using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnnuaireEntreprise.Models;
using Microsoft.EntityFrameworkCore;

namespace AnnuaireEntreprise.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync("PRAGMA journal_mode=WAL;", cancellationToken);

        var sites = await dbContext.Sites.ToListAsync(cancellationToken);
        if (sites.Count == 0)
        {
            sites = new List<Site>
            {
                new() { Nom = "Paris" },
                new() { Nom = "Lyon" },
                new() { Nom = "Marseille" },
                new() { Nom = "Lille" },
                new() { Nom = "Bordeaux" }
            };
            await dbContext.Sites.AddRangeAsync(sites, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var services = await dbContext.ServicesEntreprise.ToListAsync(cancellationToken);
        if (services.Count == 0)
        {
            services = new List<ServiceEntreprise>
            {
                new() { Nom = "Comptabilité" },
                new() { Nom = "Production" },
                new() { Nom = "Accueil" },
                new() { Nom = "Informatique" },
                new() { Nom = "Ressources Humaines" },
                new() { Nom = "Logistique" }
            };
            await dbContext.ServicesEntreprise.AddRangeAsync(services, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var salaryCount = await dbContext.Salaries.CountAsync(cancellationToken);
        if (salaryCount == 0)
        {
            var salaries = new List<Salarie>
            {
                new() { Prenom = "Claire", Nom = "Durand", Email = "claire.durand@entreprise.fr", Telephone = "0102030405", TelephonePortable="0610101010", Poste = "Comptable", DateEmbauche = new DateTime(2021, 3, 15), SiteId = sites[0].Id, ServiceEntrepriseId = services[0].Id },
                new() { Prenom = "Nicolas", Nom = "Martin", Email = "nicolas.martin@entreprise.fr", Telephone = "0102030406", TelephonePortable="0620202020", Poste = "Technicien support", DateEmbauche = new DateTime(2020, 9, 1), SiteId = sites[1].Id, ServiceEntrepriseId = services[3].Id },
                new() { Prenom = "Sofia", Nom = "Bernard", Email = "sofia.bernard@entreprise.fr", Telephone = "0102030407", TelephonePortable="0630303030", Poste = "Chargée RH", DateEmbauche = new DateTime(2022, 1, 10), SiteId = sites[2].Id, ServiceEntrepriseId = services[4].Id }
            };

            await dbContext.Salaries.AddRangeAsync(salaries, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
