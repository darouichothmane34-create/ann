using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnnuaireEntreprise.Models;

/// <summary>
/// Représente un service métier de l'entreprise (ex: RH, IT, Finance).
/// </summary>
public class ServiceEntreprise
{
    /// <summary>
    /// Identifiant unique du service (clé primaire EF Core).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nom du service.
    /// </summary>
    [Required]
    [MaxLength(120)]
    public string Nom { get; set; } = string.Empty;

    /// <summary>
    /// Description optionnelle du service.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Collection des salariés appartenant à ce service.
    /// </summary>
    public ICollection<Salarie> Salaries { get; set; } = new List<Salarie>();
}
