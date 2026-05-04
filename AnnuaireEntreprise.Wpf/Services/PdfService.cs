using System;
using System.IO;
using AnnuaireEntreprise.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AnnuaireEntreprise.Services;

public class PdfService
{
    public string GenerateSalariePdf(Salarie salarie)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents", "FichesSalaries");
        Directory.CreateDirectory(folder);

        var fileName = $"fiche_{salarie.Nom}_{salarie.Prenom}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf".Replace(" ", "_");
        var outputPath = Path.Combine(folder, fileName);

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Content().Column(column =>
                {
                    column.Spacing(8);
                    column.Item().Text("Fiche salarié").FontSize(20).Bold();
                    column.Item().Text($"Nom : {salarie.Nom}");
                    column.Item().Text($"Prénom : {salarie.Prenom}");
                    column.Item().Text($"Téléphone fixe : {salarie.Telephone}");
                    column.Item().Text($"Téléphone portable : {salarie.TelephonePortable}");
                    column.Item().Text($"Email : {salarie.Email}");
                    column.Item().Text($"Service : {salarie.ServiceEntreprise?.Nom}");
                    column.Item().Text($"Site : {salarie.Site?.Nom}");
                    column.Item().Text($"Date de génération : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                });
            });
        }).GeneratePdf(outputPath);

        return outputPath;
    }
}
