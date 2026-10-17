using Microsoft.EntityFrameworkCore;
using MyAvaloniaApp.Models;

namespace MyAvaloniaApp.Data;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<SaleRecord> SaleRecords { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // 🔑 REEMPLAZA ESTA CADENA CON LA DE TU PROYECTO DE SUPABASE (Settings -> Database -> Connection string -> URI/Session)
        // Ejemplo formato: Host=db.xxxx.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=tu_password
        optionsBuilder.UseNpgsql("Host=aws-1-us-west-2.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.czygjunzcbldhjnuxnpg;Password=0v3PhQIXZggU5KIh;SSL Mode=Require;Timeout=15");
    }
}