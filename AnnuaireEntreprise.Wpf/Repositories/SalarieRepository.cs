using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnnuaireEntreprise.Data;
using AnnuaireEntreprise.Models;
using Microsoft.EntityFrameworkCore;

namespace AnnuaireEntreprise.Repositories;

/// <summary>
/// Repository EF Core pour l'entité Salarie.
/// </summary>
public class SalarieRepository : ISalarieRepository
{
    private readonly AppDbContext _dbContext;

    public SalarieRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Salarie>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbContext.Salaries
                .AsNoTracking()
                .OrderBy(x => x.Nom)
                .ThenBy(x => x.Prenom)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Erreur lors de la lecture des salariés.", ex);
        }
    }

    public async Task<Salarie?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbContext.Salaries
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Erreur lors de la lecture du salarié #{id}.", ex);
        }
    }

    public async Task<int> AddAsync(Salarie entity, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Salaries.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Erreur lors de l'ajout d'un salarié.", ex);
        }
    }

    public async Task<bool> UpdateAsync(Salarie entity, CancellationToken cancellationToken = default)
    {
        try
        {
            var existing = await _dbContext.Salaries.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);
            if (existing is null)
            {
                return false;
            }

            existing.Nom = entity.Nom;
            existing.Prenom = entity.Prenom;
            existing.Email = entity.Email;
            existing.Telephone = entity.Telephone;
            existing.TelephonePortable = entity.TelephonePortable;
            existing.Poste = entity.Poste;
            existing.DateEmbauche = entity.DateEmbauche;
            existing.ServiceEntrepriseId = entity.ServiceEntrepriseId;
            existing.SiteId = entity.SiteId;

            var affectedRows = await _dbContext.SaveChangesAsync(cancellationToken);
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Erreur lors de la mise à jour du salarié #{entity.Id}.", ex);
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _dbContext.Salaries.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (entity is null)
            {
                return false;
            }

            _dbContext.Salaries.Remove(entity);
            var affectedRows = await _dbContext.SaveChangesAsync(cancellationToken);
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Erreur lors de la suppression du salarié #{id}.", ex);
        }
    }

    public async Task<IReadOnlyList<Salarie>> SearchByNomPartial(string nom, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbContext.Salaries
                .AsNoTracking()
                .Where(x => EF.Functions.Like(x.Nom, $"%{nom}%"))
                .OrderBy(x => x.Nom)
                .ThenBy(x => x.Prenom)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Erreur lors de la recherche par nom.", ex);
        }
    }

    public async Task<IReadOnlyList<Salarie>> SearchBySite(int siteId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbContext.Salaries
                .AsNoTracking()
                .Where(x => x.SiteId == siteId)
                .OrderBy(x => x.Nom)
                .ThenBy(x => x.Prenom)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Erreur lors de la recherche des salariés du site #{siteId}.", ex);
        }
    }

    public async Task<IReadOnlyList<Salarie>> SearchByService(int serviceId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbContext.Salaries
                .AsNoTracking()
                .Where(x => x.ServiceEntrepriseId == serviceId)
                .OrderBy(x => x.Nom)
                .ThenBy(x => x.Prenom)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Erreur lors de la recherche des salariés du service #{serviceId}.", ex);
        }
    }

    public async Task<Salarie?> GetSalarieDetails(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbContext.Salaries
                .AsNoTracking()
                .Include(x => x.Site)
                .Include(x => x.ServiceEntreprise)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Erreur lors de la lecture détaillée du salarié #{id}.", ex);
        }
    }
}
