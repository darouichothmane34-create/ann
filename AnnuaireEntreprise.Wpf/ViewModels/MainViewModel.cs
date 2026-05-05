using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AnnuaireEntreprise.Data;
using AnnuaireEntreprise.Helpers;
using AnnuaireEntreprise.Models;
using AnnuaireEntreprise.Repositories;
using AnnuaireEntreprise.Services;
using System.Windows;
using System.Diagnostics;

namespace AnnuaireEntreprise.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly SiteRepository _siteRepository;
    private readonly ServiceRepository _serviceRepository;
    private readonly ISalarieRepository _salarieRepository;
    private readonly PdfService _pdfService;
    private readonly LoggerService _loggerService;

    public ObservableCollection<Site> Sites { get; } = new();
    public ObservableCollection<ServiceEntreprise> Services { get; } = new();
    public ObservableCollection<Salarie> Salaries { get; } = new();

    private string _nomRecherche = string.Empty;
    public string NomRecherche { get => _nomRecherche; set => SetField(ref _nomRecherche, value); }

    private Site? _selectedSite;
    public Site? SelectedSite { get => _selectedSite; set => SetField(ref _selectedSite, value); }

    private ServiceEntreprise? _selectedService;
    public ServiceEntreprise? SelectedService { get => _selectedService; set => SetField(ref _selectedService, value); }

    private Salarie? _selectedSalarie;
    public Salarie? SelectedSalarie
    {
        get => _selectedSalarie;
        set
        {
            if (SetField(ref _selectedSalarie, value) && value is not null)
            {
                _ = LoadSalarieDetailsAsync(value.Id);
            }
        }
    }

    public ICommand RechercherCommand { get; }
    public ICommand ReinitialiserCommand { get; }
    public ICommand GenererPdfCommand { get; }

    public MainViewModel(AppDbContext dbContext, string connectionString)
    {
        _siteRepository = new SiteRepository(connectionString);
        _serviceRepository = new ServiceRepository(connectionString);
        _salarieRepository = new SalarieRepository(dbContext);
        _pdfService = new PdfService();
        _loggerService = new LoggerService();

        RechercherCommand = new RelayCommand(async () => await RechercherAsync());
        ReinitialiserCommand = new RelayCommand(async () => await ReinitialiserAsync());
        GenererPdfCommand = new RelayCommand(async () => await GenererPdfAsync());

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await LoadFiltersAsync();
        await LoadAllSalariesAsync();
    }

    public async Task RefreshAfterAdminChangesAsync()
    {
        await LoadFiltersAsync();
        await LoadAllSalariesAsync();
    }

    private async Task LoadFiltersAsync()
    {
        Sites.Clear();
        Services.Clear();

        var sites = await _siteRepository.GetAllAsync();
        var services = await _serviceRepository.GetAllAsync();

        foreach (var site in sites) Sites.Add(site);
        foreach (var service in services) Services.Add(service);
    }

    private async Task LoadAllSalariesAsync()
    {
        Salaries.Clear();
        var list = await _salarieRepository.GetAllAsync();
        foreach (var salarie in list) Salaries.Add(salarie);
    }

    private async Task RechercherAsync()
    {
        IReadOnlyList<Salarie> result;

        if (!string.IsNullOrWhiteSpace(NomRecherche))
        {
            result = await _salarieRepository.SearchByNomPartial(NomRecherche.Trim());
        }
        else if (SelectedSite is not null)
        {
            result = await _salarieRepository.SearchBySite(SelectedSite.Id);
        }
        else if (SelectedService is not null)
        {
            result = await _salarieRepository.SearchByService(SelectedService.Id);
        }
        else
        {
            result = await _salarieRepository.GetAllAsync();
        }

        Salaries.Clear();
        foreach (var salarie in result) Salaries.Add(salarie);
    }

    private async Task ReinitialiserAsync()
    {
        NomRecherche = string.Empty;
        SelectedSite = null;
        SelectedService = null;
        SelectedSalarie = null;
        await LoadAllSalariesAsync();
    }

    private async Task LoadSalarieDetailsAsync(int id)
    {
        var details = await _salarieRepository.GetSalarieDetails(id);
        if (details is null) return;
        _selectedSalarie = details;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSalarie)));
    }


    private async Task GenererPdfAsync()
    {
        if (SelectedSalarie is null)
        {
            MessageBox.Show("Sélectionnez un salarié.", "PDF", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var details = await _salarieRepository.GetSalarieDetails(SelectedSalarie.Id) ?? SelectedSalarie;
            var path = _pdfService.GenerateSalariePdf(details);
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
            MessageBox.Show($"PDF généré et ouvert : {path}", "PDF", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            await _loggerService.LogErrorAsync($"PDF generation failed: {ex.Message}");
            MessageBox.Show("Erreur lors de la génération du PDF.", "PDF", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
