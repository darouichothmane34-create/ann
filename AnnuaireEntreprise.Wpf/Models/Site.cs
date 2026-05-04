using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnnuaireEntreprise.Models;

/// <summary>
/// Représente un site géographique de l'entreprise (ex: Paris, Lyon).
/// </summary>
public class Site
{
    /// <summary>
    /// Identifiant unique du site (clé primaire EF Core).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nom du site.
    /// </summary>
    [Required]
    [MaxLength(120)]
    public string Nom { get; set; } = string.Empty;

    /// <summary>
    /// Adresse complète du site.
    /// </summary>
    [MaxLength(250)]
    public string? Adresse { get; set; }

    /// <summary>
    /// Collection des salariés rattachés à ce site.
    /// </summary>
    public ICollection<Salarie> Salaries { get; set; } = new List<Salarie>();
}
