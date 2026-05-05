using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnnuaireEntreprise.Models;
using Microsoft.Data.Sqlite;

namespace AnnuaireEntreprise.Repositories;

/// <summary>
/// Repository SQL primaire pour l'entité Site.
/// </summary>
public class SiteRepository : IRepository<Site>
{
    private readonly string _connectionString;

    public SiteRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IReadOnlyList<Site>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT Id, Nom, Adresse FROM Sites ORDER BY Nom;";
        var results = new List<Site>();

        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqliteCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                results.Add(MapSite(reader));
            }

            return results;
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Erreur lors de la lecture des sites.", ex);
        }
    }

    public async Task<Site?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT Id, Nom, Adresse FROM Sites WHERE Id = $id;";

        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("$id", id);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            return await reader.ReadAsync(cancellationToken) ? MapSite(reader) : null;
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Erreur lors de la lecture du site #{id}.", ex);
        }
    }

    public async Task<int> AddAsync(Site entity, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT INTO Sites (Nom, Adresse) VALUES ($nom, $adresse); SELECT last_insert_rowid();";

        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("$nom", entity.Nom);
            command.Parameters.AddWithValue("$adresse", (object?)entity.Adresse ?? DBNull.Value);

            var insertedId = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt32(insertedId);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Erreur lors de l'ajout d'un site.", ex);
        }
    }

    public async Task<bool> UpdateAsync(Site entity, CancellationToken cancellationToken = default)
    {
        const string sql = "UPDATE Sites SET Nom = $nom, Adresse = $adresse WHERE Id = $id;";

        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("$id", entity.Id);
            command.Parameters.AddWithValue("$nom", entity.Nom);
            command.Parameters.AddWithValue("$adresse", (object?)entity.Adresse ?? DBNull.Value);

            var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Erreur lors de la mise à jour du site #{entity.Id}.", ex);
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM Sites WHERE Id = $id;";

        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("$id", id);

            var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Erreur lors de la suppression du site #{id}.", ex);
        }
    }

    private static Site MapSite(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        Nom = reader.GetString(1),
        Adresse = reader.IsDBNull(2) ? null : reader.GetString(2)
    };
}
