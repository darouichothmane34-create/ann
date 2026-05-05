using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnnuaireEntreprise.Repositories;

/// <summary>
/// Contrat générique CRUD asynchrone pour les repositories.
/// </summary>
public interface IRepository<T>
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
