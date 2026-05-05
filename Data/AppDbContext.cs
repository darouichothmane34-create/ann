using AnnuaireEntreprise.Models;
using Microsoft.EntityFrameworkCore;

namespace AnnuaireEntreprise.Data;

/// <summary>
/// Contexte EF Core principal de l'application annuaire.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Table des sites d'entreprise.
    /// </summary>
    public DbSet<Site> Sites => Set<Site>();

    /// <summary>
    /// Table des services d'entreprise.
    /// </summary>
    public DbSet<ServiceEntreprise> ServicesEntreprise => Set<ServiceEntreprise>();

    /// <summary>
    /// Table des salariés.
    /// </summary>
    public DbSet<Salarie> Salaries => Set<Salarie>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Site>()
            .HasIndex(x => x.Nom)
            .IsUnique();

        modelBuilder.Entity<ServiceEntreprise>()
            .HasIndex(x => x.Nom)
            .IsUnique();

        modelBuilder.Entity<Salarie>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<Salarie>()
            .HasOne(x => x.Site)
            .WithMany(x => x.Salaries)
            .HasForeignKey(x => x.SiteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Salarie>()
            .HasOne(x => x.ServiceEntreprise)
            .WithMany(x => x.Salaries)
            .HasForeignKey(x => x.ServiceEntrepriseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
