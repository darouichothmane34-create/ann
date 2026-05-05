using System.IO;
using System.Windows;
using AnnuaireEntreprise.Data;
using AnnuaireEntreprise.Services;
using AnnuaireEntreprise.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AnnuaireEntreprise;

public partial class App : Application
{
    private readonly LoggerService _loggerService = new();
    public static string ConnectionString { get; private set; } = string.Empty;
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DispatcherUnhandledException += async (_, args) =>
        {
            await _loggerService.LogErrorAsync($"Erreur interface non gérée: {args.Exception.Message}");
            args.Handled = true;
            MessageBox.Show("Une erreur inattendue est survenue.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        };

        var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "annuaire.db");
        var connectionString = $"Data Source={dbPath}";
        ConnectionString = connectionString;

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connectionString)
            .Options;

        var dbContext = new AppDbContext(options);
        await DatabaseInitializer.InitializeAsync(dbContext);

        var mainWindow = new MainWindow
        {
            DataContext = new MainViewModel(dbContext, connectionString)
        };

        mainWindow.Show();
    }
}
