using System.ComponentModel;

namespace MyAvaloniaApp.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    // Guarda el ViewModel actual.
    private object _currentViewModel;

    // Propiedad que Avalonia observa mediante PropertyChanged.
    public object CurrentViewModel
    {
        get => _currentViewModel;

        set
        {
            _currentViewModel = value;

            // Avisamos que la propiedad cambió.
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nameof(CurrentViewModel))
            );
        }
    }

    // Evento donde se registran los callbacks que quieren enterarse de cambios.
    public event PropertyChangedEventHandler? PropertyChanged;

    public MainWindowViewModel()
    {
        CurrentViewModel = new LoginViewModel(this);
    }

    public void ChangeCurrentViewModel(object newView)
    {
        CurrentViewModel = newView;
    }
}