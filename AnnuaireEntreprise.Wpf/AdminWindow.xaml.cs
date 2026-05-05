using System.Windows;
using AnnuaireEntreprise.ViewModels;

namespace AnnuaireEntreprise;

public partial class AdminWindow : Window
{
    public AdminWindow()
    {
        InitializeComponent();
        DataContext = new AdminViewModel(App.ConnectionString);
    }
}
