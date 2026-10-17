using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyAvaloniaApp.ViewModels;

public class DashboardViewModel : INotifyPropertyChanged
{
    public MainWindowViewModel MainViewModel { get; }
    
    private object _currentPageViewModel = new HomeViewModel();
    public object CurrentPageViewModel
    {
        get => _currentPageViewModel;
        private set
        {
            _currentPageViewModel = value;
            OnPropertyChanged();
        }
    }

    public SidebarViewModel Sidebar { get; }

    public DashboardViewModel(MainWindowViewModel mainWindowViewModel)
    {
        MainViewModel = mainWindowViewModel;
        Sidebar = new SidebarViewModel(this);
    }

    public void ChangeCurrentPage(object viewModel)
    {
        // Al navegar, actualizamos de forma asincrónica sin congelar la UI
        if (viewModel is SalesViewModel salesVm)
        {
            _ = salesVm.LoadAvailableProductsAsync();
        }
        else if (viewModel is HistoryViewModel historyVm)
        {
            _ = historyVm.LoadSalesHistoryAsync();
        }
        else if (viewModel is ProductsViewModel productsVm)
        {
            _ = productsVm.LoadProductsAsync();
        }

        CurrentPageViewModel = viewModel;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}