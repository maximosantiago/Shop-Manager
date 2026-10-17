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

public class SalesViewModel : ViewModelBase
{
    public ObservableCollection<Product> AvailableProducts { get; } = new();
    public ObservableCollection<CartItem> CartItems { get; } = new();

    public ICommand AddToCartCommand { get; }
    public ICommand RemoveFromCartCommand { get; }
    public ICommand CheckoutCommand { get; }

    public decimal GrandTotal => CartItems.Sum(x => x.Total);

    // Mensaje que se muestra debajo del botón de cobrar
    private string _statusMessage = string.Empty;
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public SalesViewModel()
    {
        AddToCartCommand = new RelayCommand<Product>(AddToCart);
        RemoveFromCartCommand = new RelayCommand<CartItem>(RemoveFromCart);
        CheckoutCommand = new AsyncRelayCommand(CheckoutAsync);

        // La carga de productos se dispara desde DashboardViewModel.ChangeCurrentPage
    }

    public async Task LoadAvailableProductsAsync()
    {
        try
        {
            using var db = new AppDbContext();
            var list = await db.Products.AsNoTracking().ToListAsync();

            AvailableProducts.Clear();
            foreach (var p in list)
            {
                AvailableProducts.Add(p);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("ERROR cargando productos para venta: " + ex);
            StatusMessage = "Error al cargar productos: " + (ex.InnerException?.Message ?? ex.Message);
        }
    }

    private void AddToCart(Product product)
    {
        if (product == null || product.Stock <= 0) return;

        var existingItem = CartItems.FirstOrDefault(x => x.ProductId == product.Id);

        if (existingItem != null)
        {
            if (existingItem.Quantity < product.Stock)
            {
                existingItem.Quantity++;
                var index = CartItems.IndexOf(existingItem);
                CartItems[index] = new CartItem
                {
                    ProductId = existingItem.ProductId,
                    Name = existingItem.Name,
                    Price = existingItem.Price,
                    Quantity = existingItem.Quantity
                };
            }
        }
        else
        {
            CartItems.Add(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = 1
            });
        }

        OnPropertyChanged(nameof(GrandTotal));
    }

    private void RemoveFromCart(CartItem item)
    {
        if (item != null)
        {
            CartItems.Remove(item);
            OnPropertyChanged(nameof(GrandTotal));
        }
    }

    private async Task CheckoutAsync()
    {
        if (CartItems.Count == 0)
        {
            StatusMessage = "El carrito está vacío.";
            return;
        }

        StatusMessage = "Procesando venta...";

        try
        {
            using var db = new AppDbContext();

            // 1. Crear el registro de venta con sus ítems (fecha en UTC)
            var newSale = new SaleRecord
            {
                Date = DateTime.UtcNow,
                TotalAmount = GrandTotal,
                Items = CartItems.Select(ci => new SaleItem
                {
                    ProductId = ci.ProductId,
                    Name = ci.Name,
                    Price = ci.Price,
                    Quantity = ci.Quantity
                }).ToList()
            };

            db.SaleRecords.Add(newSale);

            // 2. Descontar stock en la base de datos
            foreach (var cartItem in CartItems)
            {
                var productInDb = await db.Products.FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);
                if (productInDb != null)
                {
                    productInDb.Stock -= cartItem.Quantity;
                    if (productInDb.Stock < 0) productInDb.Stock = 0;
                }
            }

            await db.SaveChangesAsync();

            // 3. Limpiar carrito y refrescar stock disponible
            CartItems.Clear();
            OnPropertyChanged(nameof(GrandTotal));
            await LoadAvailableProductsAsync();

            StatusMessage = "Venta registrada ✅";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("ERROR al cobrar: " + ex);
            StatusMessage = "Error al cobrar: " + (ex.InnerException?.Message ?? ex.Message);
        }
    }
}