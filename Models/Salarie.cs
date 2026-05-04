using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnnuaireEntreprise.Models;

/// <summary>
/// Représente un salarié dans l'annuaire d'entreprise.
/// </summary>
public class Salarie
{
    /// <summary>
    /// Identifiant unique du salarié (clé primaire EF Core).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Prénom du salarié.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Prenom { get; set; } = string.Empty;

    /// <summary>
    /// Nom de famille du salarié.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    /// <summary>
    /// Adresse e-mail professionnelle.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Numéro de téléphone professionnel.
    /// </summary>
    [MaxLength(30)]
    public string? Telephone { get; set; }

    /// <summary>
    /// Numéro de téléphone portable professionnel.
    /// </summary>
    [MaxLength(30)]
    public string? TelephonePortable { get; set; }

    /// <summary>
    /// Fonction / poste occupé dans l'entreprise.
    /// </summary>
    [MaxLength(120)]
    public string? Poste { get; set; }

    /// <summary>
    /// Date d'embauche du salarié.
    /// </summary>
    public DateTime? DateEmbauche { get; set; }

    /// <summary>
    /// Clé étrangère vers le service du salarié.
    /// </summary>
    [ForeignKey(nameof(ServiceEntreprise))]
    public int ServiceEntrepriseId { get; set; }

    /// <summary>
    /// Navigation vers le service du salarié.
    /// </summary>
    public ServiceEntreprise? ServiceEntreprise { get; set; }

    /// <summary>
    /// Clé étrangère vers le site du salarié.
    /// </summary>
    [ForeignKey(nameof(Site))]
    public int SiteId { get; set; }

    /// <summary>
    /// Navigation vers le site du salarié.
    /// </summary>
    public Site? Site { get; set; }
}
