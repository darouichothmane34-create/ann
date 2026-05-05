using System.Windows;
using AnnuaireEntreprise.Services;

namespace AnnuaireEntreprise;

public partial class AdminLoginWindow : Window
{
    private readonly AuthService _authService;
    private readonly LoggerService _loggerService;

    public AdminLoginWindow(AuthService authService, LoggerService loggerService)
    {
        InitializeComponent();
        _authService = authService;
        _loggerService = loggerService;
    }

    private async void OnValidate(object sender, RoutedEventArgs e)
    {
        if (_authService.VerifyAdminPassword(PasswordInput.Password))
        {
            await _loggerService.LogAdminAccessSuccessAsync("Tentative admin réussie (AdminLoginWindow)");
            DialogResult = true;
            Close();
            return;
        }

        await _loggerService.LogAdminAccessDeniedAsync("Tentative admin refusée (AdminLoginWindow)");
        MessageBox.Show("Mot de passe incorrect.", "Accès refusé", MessageBoxButton.OK, MessageBoxImage.Error);
        PasswordInput.Clear();
        PasswordInput.Focus();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
