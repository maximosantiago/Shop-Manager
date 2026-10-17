using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MyAvaloniaApp.Data;
using MyAvaloniaApp.Models;

namespace MyAvaloniaApp.ViewModels;

public class ProductsViewModel : ViewModelBase
{
    public ObservableCollection<Product> Products { get; } = new();

    private string _inputName = string.Empty;
    public string InputName
    {
        get => _inputName;
        set => SetProperty(ref _inputName, value);
    }

    private decimal _inputPrice;
    public decimal InputPrice
    {
        get => _inputPrice;
        set => SetProperty(ref _inputPrice, value);
    }

    private int _inputStock;
    public int InputStock
    {
        get => _inputStock;
        set => SetProperty(ref _inputStock, value);
    }

    private int? _editingProductId = null;

    public ICommand SaveProductCommand { get; }
    public ICommand EditProductCommand { get; }
    public ICommand DeleteProductCommand { get; }

    public ProductsViewModel()
    {
        SaveProductCommand = new AsyncRelayCommand(SaveProductAsync);
        EditProductCommand = new AsyncRelayCommand<int>(LoadProductIntoFormAsync);
        DeleteProductCommand = new AsyncRelayCommand<int>(DeleteProductAsync);

        // Disparamos la carga inicial sin bloquear el constructor
        _ = LoadProductsAsync();
    }

    public async Task LoadProductsAsync()
    {
        try
        {
            using var db = new AppDbContext();
            var list = await db.Products.AsNoTracking().ToListAsync();

            Products.Clear();
            foreach (var p in list)
            {
                Products.Add(p);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("ERROR cargando productos: " + ex);
        }
    }

    private async Task SaveProductAsync()
    {
        if (string.IsNullOrWhiteSpace(InputName)) return;

        using var db = new AppDbContext();

        if (_editingProductId == null || _editingProductId == 0)
        {
            var newProduct = new Product
            {
                Name = InputName,
                Price = InputPrice,
                Stock = InputStock
            };
            db.Products.Add(newProduct);
            await db.SaveChangesAsync();
            Products.Add(newProduct);
        }
        else
        {
            var productToUpdate = await db.Products.FirstOrDefaultAsync(p => p.Id == _editingProductId);
            if (productToUpdate != null)
            {
                productToUpdate.Name = InputName;
                productToUpdate.Price = InputPrice;
                productToUpdate.Stock = InputStock;
                await db.SaveChangesAsync();
            }

            await LoadProductsAsync();
            _editingProductId = null;
        }

        ClearForm();
    }

    private async Task LoadProductIntoFormAsync(int id)
    {
        using var db = new AppDbContext();
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product != null)
        {
            InputName = product.Name;
            InputPrice = product.Price;
            InputStock = product.Stock;
            _editingProductId = product.Id;
        }
    }

    private async Task DeleteProductAsync(int id)
    {
        using var db = new AppDbContext();
        var productToRemove = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (productToRemove != null)
        {
            db.Products.Remove(productToRemove);
            await db.SaveChangesAsync();
            
            var local = Products.FirstOrDefault(p => p.Id == id);
            if (local != null) Products.Remove(local);

            if (_editingProductId == id)
            {
                ClearForm();
            }
        }
    }

    private void ClearForm()
    {
        InputName = string.Empty;
        InputPrice = 0;
        InputStock = 0;
        _editingProductId = null;
    }
}