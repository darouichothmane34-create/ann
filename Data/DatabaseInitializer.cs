using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnnuaireEntreprise.Models;
using Microsoft.EntityFrameworkCore;

namespace AnnuaireEntreprise.Data;

/// <summary>
/// Initialise la base SQLite au démarrage de l'application.
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Crée automatiquement la base, active WAL et insère des données de test.
    /// </summary>
    public static async Task InitializeAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        // Active le mode WAL pour permettre plusieurs lecteurs/instances.
        await dbContext.Database.ExecuteSqlRawAsync("PRAGMA journal_mode=WAL;", cancellationToken);

        if (await dbContext.Sites.AnyAsync(cancellationToken))
        {
            return;
        }

        var sites = new List<Site>
        {
            new() { Nom = "Paris", Adresse = "10 Rue de Rivoli, 75001 Paris" },
            new() { Nom = "Lyon", Adresse = "20 Rue de la République, 69002 Lyon" },
            new() { Nom = "Marseille", Adresse = "5 La Canebière, 13001 Marseille" },
            new() { Nom = "Lille", Adresse = "12 Place Rihour, 59800 Lille" },
            new() { Nom = "Bordeaux", Adresse = "8 Cours de l'Intendance, 33000 Bordeaux" }
        };

        var services = new List<ServiceEntreprise>
        {
            new() { Nom = "Comptabilité", Description = "Gestion comptable et financière." },
            new() { Nom = "Production", Description = "Pilotage des opérations de production." },
            new() { Nom = "Accueil", Description = "Accueil physique et téléphonique." },
            new() { Nom = "Informatique", Description = "Support, infrastructure et développement." },
            new() { Nom = "Ressources Humaines", Description = "Gestion RH, paie et recrutement." },
            new() { Nom = "Logistique", Description = "Gestion des flux et approvisionnements." }
        };

        await dbContext.Sites.AddRangeAsync(sites, cancellationToken);
        await dbContext.ServicesEntreprise.AddRangeAsync(services, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var salaries = new List<Salarie>
        {
            new()
            {
                Prenom = "Claire", Nom = "Durand", Email = "claire.durand@entreprise.fr", Telephone = "0102030405",
                Poste = "Comptable", DateEmbauche = new DateTime(2021, 3, 15), SiteId = sites[0].Id, ServiceEntrepriseId = services[0].Id
            },
            new()
            {
                Prenom = "Nicolas", Nom = "Martin", Email = "nicolas.martin@entreprise.fr", Telephone = "0102030406",
                Poste = "Technicien support", DateEmbauche = new DateTime(2020, 9, 1), SiteId = sites[1].Id, ServiceEntrepriseId = services[3].Id
            },
            new()
            {
                Prenom = "Sofia", Nom = "Bernard", Email = "sofia.bernard@entreprise.fr", Telephone = "0102030407",
                Poste = "Chargée RH", DateEmbauche = new DateTime(2022, 1, 10), SiteId = sites[2].Id, ServiceEntrepriseId = services[4].Id
            },
            new()
            {
                Prenom = "Julien", Nom = "Petit", Email = "julien.petit@entreprise.fr", Telephone = "0102030408",
                Poste = "Responsable logistique", DateEmbauche = new DateTime(2019, 11, 20), SiteId = sites[3].Id, ServiceEntrepriseId = services[5].Id
            },
            new()
            {
                Prenom = "Emma", Nom = "Roux", Email = "emma.roux@entreprise.fr", Telephone = "0102030409",
                Poste = "Hôtesse d'accueil", DateEmbauche = new DateTime(2023, 5, 2), SiteId = sites[4].Id, ServiceEntrepriseId = services[2].Id
            }
        };

        await dbContext.Salaries.AddRangeAsync(salaries, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
