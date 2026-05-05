using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using AnnuaireEntreprise.Services;
using AnnuaireEntreprise.ViewModels;

namespace AnnuaireEntreprise;

public partial class MainWindow : Window
{
    private readonly AuthService _authService = new();
    private readonly LoggerService _loggerService = new();

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.A)
        {
            await OpenAdminLoginAsync();
            e.Handled = true;
        }
    }

    private async Task OpenAdminLoginAsync()
    {
        var loginWindow = new AdminLoginWindow(_authService, _loggerService)
        {
            Owner = this
        };

        var result = loginWindow.ShowDialog();
        if (result == true)
        {
            _ = _loggerService.LogAdminPanelOpenedAsync("Ouverture du panneau administrateur");
            var adminWindow = new AdminWindow
            {
                Owner = this
            };

            adminWindow.ShowDialog();

            if (DataContext is MainViewModel vm)
            {
                                await vm.RefreshAfterAdminChangesAsync();
            }
        }
    }
}
