using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Mail;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using AnnuaireEntreprise.Data;
using AnnuaireEntreprise.Helpers;
using AnnuaireEntreprise.Models;
using AnnuaireEntreprise.Repositories;
using AnnuaireEntreprise.Services;
using Microsoft.EntityFrameworkCore;

namespace AnnuaireEntreprise.ViewModels;

public class AdminViewModel : INotifyPropertyChanged
{
    private readonly SiteRepository _siteRepository;
    private readonly ServiceRepository _serviceRepository;
    private readonly ISalarieRepository _salarieRepository;
    private readonly LoggerService _loggerService;
    private readonly RandomUserImportService _randomUserImportService;
    private readonly HttpClient _httpClient;

    public ObservableCollection<Site> Sites { get; } = new();
    public ObservableCollection<ServiceEntreprise> Services { get; } = new();
    public ObservableCollection<Salarie> Salaries { get; } = new();

    public Site? SelectedSite { get => _selectedSite; set { if (SetField(ref _selectedSite, value) && value is not null) SiteVille = value.Nom; } }
    private Site? _selectedSite;
    public ServiceEntreprise? SelectedService { get => _selectedService; set { if (SetField(ref _selectedService, value) && value is not null) ServiceNom = value.Nom; } }
    private ServiceEntreprise? _selectedService;

    public Salarie? SelectedSalarie
    {
        get => _selectedSalarie;
        set
        {
            if (SetField(ref _selectedSalarie, value) && value is not null)
            {
                SalarieNom = value.Nom;
                SalariePrenom = value.Prenom;
                SalarieTelephoneFixe = value.Telephone ?? string.Empty;
                SalarieTelephonePortable = value.TelephonePortable ?? string.Empty;
                SalarieEmail = value.Email;
                SalarieSelectedService = Services.FirstOrDefault(x => x.Id == value.ServiceEntrepriseId);
                SalarieSelectedSite = Sites.FirstOrDefault(x => x.Id == value.SiteId);
            }
        }
    }
    private Salarie? _selectedSalarie;

    public string SiteVille { get => _siteVille; set => SetField(ref _siteVille, value); }
    private string _siteVille = string.Empty;
    public string ServiceNom { get => _serviceNom; set => SetField(ref _serviceNom, value); }
    private string _serviceNom = string.Empty;

    public string SalarieNom { get => _salarieNom; set => SetField(ref _salarieNom, value); }
    private string _salarieNom = string.Empty;
    public string SalariePrenom { get => _salariePrenom; set => SetField(ref _salariePrenom, value); }
    private string _salariePrenom = string.Empty;
    public string SalarieTelephoneFixe { get => _salarieTelephoneFixe; set => SetField(ref _salarieTelephoneFixe, value); }
    private string _salarieTelephoneFixe = string.Empty;
    public string SalarieTelephonePortable { get => _salarieTelephonePortable; set => SetField(ref _salarieTelephonePortable, value); }
    private string _salarieTelephonePortable = string.Empty;
    public string SalarieEmail { get => _salarieEmail; set => SetField(ref _salarieEmail, value); }
    private string _salarieEmail = string.Empty;
    public ServiceEntreprise? SalarieSelectedService { get => _salarieSelectedService; set => SetField(ref _salarieSelectedService, value); }
    private ServiceEntreprise? _salarieSelectedService;
    public Site? SalarieSelectedSite { get => _salarieSelectedSite; set => SetField(ref _salarieSelectedSite, value); }
    private Site? _salarieSelectedSite;

    public ICommand AddSiteCommand { get; }
    public ICommand UpdateSiteCommand { get; }
    public ICommand DeleteSiteCommand { get; }
    public ICommand RefreshSiteCommand { get; }
    public ICommand AddServiceCommand { get; }
    public ICommand UpdateServiceCommand { get; }
    public ICommand DeleteServiceCommand { get; }
    public ICommand RefreshServiceCommand { get; }
    public ICommand AddSalarieCommand { get; }
    public ICommand UpdateSalarieCommand { get; }
    public ICommand DeleteSalarieCommand { get; }
    public ICommand RefreshSalarieCommand { get; }
    public ICommand ImporterApiCommand { get; }

    public AdminViewModel(string connectionString)
    {
        _siteRepository = new SiteRepository(connectionString);
        _serviceRepository = new ServiceRepository(connectionString);
        var dbOptions = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connectionString).Options;
        _salarieRepository = new SalarieRepository(new AppDbContext(dbOptions));
        _loggerService = new LoggerService();
        _randomUserImportService = new RandomUserImportService();
        _httpClient = new HttpClient();

        AddSiteCommand = new RelayCommand(async () => await AddSiteAsync());
        UpdateSiteCommand = new RelayCommand(async () => await UpdateSiteAsync());
        DeleteSiteCommand = new RelayCommand(async () => await DeleteSiteAsync());
        RefreshSiteCommand = new RelayCommand(async () => await LoadSitesAsync());
        AddServiceCommand = new RelayCommand(async () => await AddServiceAsync());
        UpdateServiceCommand = new RelayCommand(async () => await UpdateServiceAsync());
        DeleteServiceCommand = new RelayCommand(async () => await DeleteServiceAsync());
        RefreshServiceCommand = new RelayCommand(async () => await LoadServicesAsync());
        AddSalarieCommand = new RelayCommand(async () => await AddSalarieAsync());
        UpdateSalarieCommand = new RelayCommand(async () => await UpdateSalarieAsync());
        DeleteSalarieCommand = new RelayCommand(async () => await DeleteSalarieAsync());
        RefreshSalarieCommand = new RelayCommand(async () => await LoadSalariesAsync());
        ImporterApiCommand = new RelayCommand(async () => await ImporterApiAsync());

        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { await LoadSitesAsync(); await LoadServicesAsync(); await LoadSalariesAsync(); }

    // Sites/Services methods kept from step 7
    private async Task LoadSitesAsync() { try { Sites.Clear(); foreach (var i in await _siteRepository.GetAllAsync()) Sites.Add(i);} catch (Exception ex){ await _loggerService.LogErrorAsync($"Sites refresh failed: {ex.Message}"); MessageBox.Show("Erreur lors du chargement des sites."); } }
    private async Task AddSiteAsync() { if (string.IsNullOrWhiteSpace(SiteVille)){ MessageBox.Show("La ville est obligatoire."); return;} try { await _siteRepository.AddAsync(new Site{Nom=SiteVille.Trim()}); SiteVille=string.Empty; await LoadSitesAsync(); MessageBox.Show("Site ajouté avec succès."); } catch (Exception ex){ await _loggerService.LogErrorAsync($"Site add failed: {ex.Message}"); MessageBox.Show("Erreur lors de l'ajout du site."); } }
    private async Task UpdateSiteAsync() { if (SelectedSite is null || string.IsNullOrWhiteSpace(SiteVille)){ MessageBox.Show("Sélectionnez un site et renseignez la ville."); return;} try { SelectedSite.Nom=SiteVille.Trim(); var ok = await _siteRepository.UpdateAsync(SelectedSite); if (!ok) { MessageBox.Show("Modification non appliquée."); return; } await LoadSitesAsync(); MessageBox.Show("Site modifié avec succès."); } catch (Exception ex){ await _loggerService.LogErrorAsync($"Site update failed: {ex.Message}"); MessageBox.Show("Erreur lors de la modification du site."); } }
    private async Task DeleteSiteAsync() { if (SelectedSite is null){ MessageBox.Show("Sélectionnez un site."); return;} try { var linked = await _salarieRepository.SearchBySite(SelectedSite.Id); if (linked.Count > 0) { MessageBox.Show("Impossible de supprimer ce site : des salariés y sont rattachés."); return; } var ok = await _siteRepository.DeleteAsync(SelectedSite.Id); if (!ok) { MessageBox.Show("Suppression non appliquée."); return; } SiteVille=string.Empty; await LoadSitesAsync(); MessageBox.Show("Site supprimé avec succès."); } catch (Exception ex){ await _loggerService.LogErrorAsync($"Site delete failed: {ex.Message}"); MessageBox.Show("Erreur lors de la suppression du site."); } }
    private async Task LoadServicesAsync() { try { Services.Clear(); foreach (var i in await _serviceRepository.GetAllAsync()) Services.Add(i);} catch (Exception ex){ await _loggerService.LogErrorAsync($"Services refresh failed: {ex.Message}"); MessageBox.Show("Erreur lors du chargement des services."); } }
    private async Task AddServiceAsync() { if (string.IsNullOrWhiteSpace(ServiceNom)){ MessageBox.Show("Le nom du service est obligatoire."); return;} try { await _serviceRepository.AddAsync(new ServiceEntreprise{Nom=ServiceNom.Trim()}); ServiceNom=string.Empty; await LoadServicesAsync(); MessageBox.Show("Service ajouté avec succès."); } catch (Exception ex){ await _loggerService.LogErrorAsync($"Service add failed: {ex.Message}"); MessageBox.Show("Erreur lors de l'ajout du service."); } }
    private async Task UpdateServiceAsync() { if (SelectedService is null || string.IsNullOrWhiteSpace(ServiceNom)){ MessageBox.Show("Sélectionnez un service et renseignez le nom."); return;} try { SelectedService.Nom=ServiceNom.Trim(); var ok = await _serviceRepository.UpdateAsync(SelectedService); if (!ok) { MessageBox.Show("Modification non appliquée."); return; } await LoadServicesAsync(); MessageBox.Show("Service modifié avec succès."); } catch (Exception ex){ await _loggerService.LogErrorAsync($"Service update failed: {ex.Message}"); MessageBox.Show("Erreur lors de la modification du service."); } }
    private async Task DeleteServiceAsync() { if (SelectedService is null){ MessageBox.Show("Sélectionnez un service."); return;} try { var linked = await _salarieRepository.SearchByService(SelectedService.Id); if (linked.Count > 0) { MessageBox.Show("Impossible de supprimer ce service : des salariés y sont rattachés."); return; } var ok = await _serviceRepository.DeleteAsync(SelectedService.Id); if (!ok) { MessageBox.Show("Suppression non appliquée."); return; } ServiceNom=string.Empty; await LoadServicesAsync(); MessageBox.Show("Service supprimé avec succès."); } catch (Exception ex){ await _loggerService.LogErrorAsync($"Service delete failed: {ex.Message}"); MessageBox.Show("Erreur lors de la suppression du service."); } }

    private async Task LoadSalariesAsync() { try { Salaries.Clear(); foreach (var i in await _salarieRepository.GetAllAsync()) Salaries.Add(i);} catch (Exception ex){ await _loggerService.LogErrorAsync($"Salaries refresh failed: {ex.Message}"); MessageBox.Show("Erreur lors du chargement des salariés."); } }

    private bool ValidateSalarie()
    {
        if (string.IsNullOrWhiteSpace(SalarieNom) || string.IsNullOrWhiteSpace(SalariePrenom) || string.IsNullOrWhiteSpace(SalarieEmail) || SalarieSelectedService is null || SalarieSelectedSite is null)
        { MessageBox.Show("Nom, prénom, email, service et site sont obligatoires."); return false; }
        try { _ = new MailAddress(SalarieEmail); }
        catch { MessageBox.Show("Email invalide."); return false; }
        return true;
    }

    private async Task AddSalarieAsync()
    {
        if (!ValidateSalarie()) return;
        try
        {
            await _salarieRepository.AddAsync(new Salarie { Nom = SalarieNom.Trim(), Prenom = SalariePrenom.Trim(), Telephone = SalarieTelephoneFixe.Trim(), TelephonePortable = SalarieTelephonePortable.Trim(), Email = SalarieEmail.Trim(), ServiceEntrepriseId = SalarieSelectedService!.Id, SiteId = SalarieSelectedSite!.Id });
            await LoadSalariesAsync();
            MessageBox.Show("Salarié ajouté avec succès.");
        }
        catch (Exception ex) { await _loggerService.LogErrorAsync($"Salarie add failed: {ex.Message}"); MessageBox.Show("Erreur lors de l'ajout du salarié."); }
    }

    private async Task UpdateSalarieAsync()
    {
        if (SelectedSalarie is null || !ValidateSalarie()) return;
        try
        {
            SelectedSalarie.Nom = SalarieNom.Trim(); SelectedSalarie.Prenom = SalariePrenom.Trim(); SelectedSalarie.Telephone = SalarieTelephoneFixe.Trim(); SelectedSalarie.TelephonePortable = SalarieTelephonePortable.Trim(); SelectedSalarie.Email = SalarieEmail.Trim(); SelectedSalarie.ServiceEntrepriseId = SalarieSelectedService!.Id; SelectedSalarie.SiteId = SalarieSelectedSite!.Id;
            await _salarieRepository.UpdateAsync(SelectedSalarie);
            await LoadSalariesAsync();
            MessageBox.Show("Salarié modifié avec succès.");
        }
        catch (Exception ex) { await _loggerService.LogErrorAsync($"Salarie update failed: {ex.Message}"); MessageBox.Show("Erreur lors de la modification du salarié."); }
    }

    private async Task DeleteSalarieAsync()
    {
        if (SelectedSalarie is null) { MessageBox.Show("Sélectionnez un salarié."); return; }
        try { await _salarieRepository.DeleteAsync(SelectedSalarie.Id); await LoadSalariesAsync(); MessageBox.Show("Salarié supprimé avec succès."); }
        catch (Exception ex) { await _loggerService.LogErrorAsync($"Salarie delete failed: {ex.Message}"); MessageBox.Show("Erreur lors de la suppression du salarié."); }
    }


    private async Task ImporterApiAsync()
    {
        try
        {
            var imported = await _randomUserImportService.ImportAsync(_httpClient, _salarieRepository, Sites.ToList(), Services.ToList());
            await LoadSalariesAsync();
            MessageBox.Show($"Import terminé : {imported} salarié(s) importé(s).", "Import API", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            await _loggerService.LogErrorAsync($"API import failed: {ex.Message}");
            MessageBox.Show("Erreur lors de l'import API.", "Import API", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? name = null) { if (EqualityComparer<T>.Default.Equals(field, value)) return false; field=value; PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(name)); return true; }
}
