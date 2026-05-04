using System.Net.Http;
using System.Text.Json;
using AnnuaireEntreprise.Models;
using AnnuaireEntreprise.Repositories;

namespace AnnuaireEntreprise.Services;

public class RandomUserImportService
{
    private static readonly Uri ApiUri = new("https://randomuser.me/api/?results=10&nat=fr");

    public async Task<int> ImportAsync(HttpClient httpClient, ISalarieRepository salarieRepository, IReadOnlyList<Site> sites, IReadOnlyList<ServiceEntreprise> services, CancellationToken cancellationToken = default)
    {
        if (sites.Count == 0 || services.Count == 0)
            return 0;

        using var response = await httpClient.GetAsync(ApiUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        var payload = await JsonSerializer.DeserializeAsync<RandomUserResponse>(stream, cancellationToken: cancellationToken);
        if (payload?.Results is null || payload.Results.Count == 0)
            return 0;

        var existing = await salarieRepository.GetAllAsync(cancellationToken);
        var existingEmails = existing.Select(x => x.Email.ToLowerInvariant()).ToHashSet();

        var rng = new Random();
        var imported = 0;

        foreach (var person in payload.Results)
        {
            var email = person.Email?.Trim();
            if (string.IsNullOrWhiteSpace(email))
                continue;

            if (existingEmails.Contains(email.ToLowerInvariant()))
                continue;

            var salarie = new Salarie
            {
                Nom = person.Name?.Last ?? string.Empty,
                Prenom = person.Name?.First ?? string.Empty,
                Telephone = person.Phone,
                TelephonePortable = person.Cell,
                Email = email,
                SiteId = sites[rng.Next(sites.Count)].Id,
                ServiceEntrepriseId = services[rng.Next(services.Count)].Id
            };

            await salarieRepository.AddAsync(salarie, cancellationToken);
            existingEmails.Add(email.ToLowerInvariant());
            imported++;
        }

        return imported;
    }

    private class RandomUserResponse { public List<RandomUserItem> Results { get; set; } = []; }
    private class RandomUserItem { public RandomUserName? Name { get; set; } public string? Phone { get; set; } public string? Cell { get; set; } public string? Email { get; set; } }
    private class RandomUserName { public string? First { get; set; } public string? Last { get; set; } }
}
