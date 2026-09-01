using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyAvaloniaApp.ViewModels;

public class DashboardViewModel : INotifyPropertyChanged
{
    public MainWindowViewModel MainViewModel { get; }
    
    private object _currentPageViewModel;

    
    public object CurrentPageViewModel
    {
        get => _currentPageViewModel;
        private set
        {
            _currentPageViewModel = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public SidebarViewModel Sidebar { get; }

    public DashboardViewModel(MainWindowViewModel mainWindowViewModel)
    {
        MainViewModel = mainWindowViewModel;

        Sidebar = new SidebarViewModel(this);

        CurrentPageViewModel = new HomeViewModel();
    }
    public void ChangeCurrentPage(object viewModel)
    {
        CurrentPageViewModel = viewModel;
    }

    protected void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}