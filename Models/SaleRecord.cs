using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyAvaloniaApp.Models;

public class SaleRecord
{
    [Key]
    public int Id { get; set; }

    // UTC: Npgsql rechaza fechas con hora local en columnas timestamptz
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }
    public List<SaleItem> Items { get; set; } = new();
}

public class SaleItem
{
    [Key]
    public int Id { get; set; }
    public int SaleRecordId { get; set; }
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Total => Price * Quantity;
}