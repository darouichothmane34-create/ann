using AnnuaireEntreprise.Models;

namespace AnnuaireEntreprise.Repositories;

/// <summary>
/// Contrat spécifique pour les recherches salariées.
/// </summary>
public interface ISalarieRepository : IRepository<Salarie>
{
    Task<IReadOnlyList<Salarie>> SearchByNomPartial(string nom, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Salarie>> SearchBySite(int siteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Salarie>> SearchByService(int serviceId, CancellationToken cancellationToken = default);
    Task<Salarie?> GetSalarieDetails(int id, CancellationToken cancellationToken = default);
}
