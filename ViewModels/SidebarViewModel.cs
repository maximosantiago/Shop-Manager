using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MyAvaloniaApp.ViewModels;

public class SidebarViewModel
{
    public ObservableCollection<SidebarItem> Items { get; }

    private DashboardViewModel Dashboard { get; }

    public ICommand SelectItemCommand { get; }

    public SidebarViewModel(DashboardViewModel dashboard)
    {
        Dashboard = dashboard;

        Items = new ObservableCollection<SidebarItem>
        {
            new SidebarItem("Inicio", "home", new HomeViewModel()),
            new SidebarItem("Productos", "products", new ProductsViewModel()),
            new SidebarItem("Clientes", "clients", new ClientsViewModel()),
            new SidebarItem("Ventas", "sales", new SalesViewModel())
        };

        Items[0].Active = true;

        SelectItemCommand = new RelayCommand<SidebarItem>(SelectItem);
    }

    public void SelectItem(SidebarItem selectedItem)
    {
        foreach (var item in Items)
        {
            item.Active = item == selectedItem;
        }

        Dashboard.ChangeCurrentPage(selectedItem.ViewModel);
    }
}