using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnnuaireEntreprise.Models;
using Microsoft.Data.Sqlite;

namespace AnnuaireEntreprise.Repositories;

/// <summary>
/// Repository SQL primaire pour l'entité ServiceEntreprise.
/// </summary>
public class ServiceRepository : IRepository<ServiceEntreprise>
{
    private readonly string _connectionString;

    public ServiceRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IReadOnlyList<ServiceEntreprise>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT Id, Nom, Description FROM ServicesEntreprise ORDER BY Nom;";
        var results = new List<ServiceEntreprise>();

        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqliteCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                results.Add(MapService(reader));
            }

            return results;
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Erreur lors de la lecture des services.", ex);
        }
    }

    public async Task<ServiceEntreprise?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT Id, Nom, Description FROM ServicesEntreprise WHERE Id = $id;";

        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("$id", id);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            return await reader.ReadAsync(cancellationToken) ? MapService(reader) : null;
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Erreur lors de la lecture du service #{id}.", ex);
        }
    }

    public async Task<int> AddAsync(ServiceEntreprise entity, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT INTO ServicesEntreprise (Nom, Description) VALUES ($nom, $description); SELECT last_insert_rowid();";

        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("$nom", entity.Nom);
            command.Parameters.AddWithValue("$description", (object?)entity.Description ?? DBNull.Value);

            var insertedId = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt32(insertedId);
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Erreur lors de l'ajout d'un service.", ex);
        }
    }

    public async Task<bool> UpdateAsync(ServiceEntreprise entity, CancellationToken cancellationToken = default)
    {
        const string sql = "UPDATE ServicesEntreprise SET Nom = $nom, Description = $description WHERE Id = $id;";

        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("$id", entity.Id);
            command.Parameters.AddWithValue("$nom", entity.Nom);
            command.Parameters.AddWithValue("$description", (object?)entity.Description ?? DBNull.Value);

            var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Erreur lors de la mise à jour du service #{entity.Id}.", ex);
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM ServicesEntreprise WHERE Id = $id;";

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
            throw new RepositoryException($"Erreur lors de la suppression du service #{id}.", ex);
        }
    }

    private static ServiceEntreprise MapService(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        Nom = reader.GetString(1),
        Description = reader.IsDBNull(2) ? null : reader.GetString(2)
    };
}
