
namespace MyAvaloniaApp.ViewModels;

public class SidebarItem
{
    public string Name { get; set; }
    public string Value { get; set; }
    public bool Active { get; set; }

    public object ViewModel { get; set; }

    public SidebarItem(
        string name,
        string value,
        object viewModel)
    {
        Name = name;
        Value = value;
        ViewModel = viewModel;
        Active = false;
    }
}