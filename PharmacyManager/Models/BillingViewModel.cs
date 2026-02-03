namespace PharmacyManager.Models
{
    public class BillingViewModel
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        public string CustomerName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public decimal Discount { get; set; }
        public List<SalesDetailItem> Items { get; set; } = new List<SalesDetailItem>();
        public List<Medicine> Medicines { get; set; } = new List<Medicine>();
    }

    public class SalesDetailItem
    {
        public int MedicineId { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class InvoiceDetailViewModel
    {
        public SalesMaster Invoice { get; set; } = null!;
        public List<SalesDetail> Details { get; set; } = new List<SalesDetail>();
    }
}
