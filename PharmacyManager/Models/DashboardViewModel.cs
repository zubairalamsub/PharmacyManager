namespace PharmacyManager.Models
{
    public class DashboardViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public int TotalMedicines { get; set; }
        public int LowStockCount { get; set; }
        public int TotalInvoices { get; set; }
        public List<SalesMaster> RecentSales { get; set; } = new();
    }
}
