using System.Windows.Input;

namespace MyAvaloniaApp.ViewModels;

public class LoginViewModel
{
    private MainWindowViewModel _mainWindowViewModel { get; }
    public string Usuario { get; set; } = "";
    public string Contraseña { get; set; } = "";

    public ICommand LoginCommand { get; }

    public LoginViewModel(MainWindowViewModel mainWindowViewModel)
    {
        this._mainWindowViewModel = mainWindowViewModel;
        LoginCommand = new RelayCommand(Login);
    }

    private void Login()
    {
        if (Usuario == "admin" && Contraseña == "1234")
        {
            this._mainWindowViewModel.ChangeCurrentViewModel(new DashboardViewModel(this._mainWindowViewModel));
            // Más adelante navegaremos al Dashboard
        }
    }
}