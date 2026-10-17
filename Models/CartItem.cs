namespace MyAvaloniaApp.Models;

public class CartItem
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    
    // Propiedad calculada para el subtotal de esta línea
    public decimal Total => Price * Quantity;
}