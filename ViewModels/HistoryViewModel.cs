using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyAvaloniaApp.Data;
using MyAvaloniaApp.Models;

namespace MyAvaloniaApp.ViewModels;

public class HistoryViewModel : ViewModelBase
{
    public ObservableCollection<SaleRecord> SalesHistory { get; } = new();

    public decimal TotalRevenue => SalesHistory.Sum(s => s.TotalAmount);
    public int TotalTransactions => SalesHistory.Count;
    public decimal AverageTicket => TotalTransactions > 0 ? TotalRevenue / TotalTransactions : 0;

    public HistoryViewModel()
    {
        _ = LoadSalesHistoryAsync();
    }

    public async Task LoadSalesHistoryAsync()
    {
        try
        {
            using var db = new AppDbContext();

            var sales = await db.SaleRecords
                .AsNoTracking()
                .Include(s => s.Items)
                .OrderByDescending(s => s.Date)
                .ToListAsync();

            SalesHistory.Clear();
            foreach (var sale in sales)
            {
                SalesHistory.Add(sale);
            }

            OnPropertyChanged(nameof(TotalRevenue));
            OnPropertyChanged(nameof(TotalTransactions));
            OnPropertyChanged(nameof(AverageTicket));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("ERROR cargando historial: " + ex);
        }
    }
}