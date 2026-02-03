using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyManager.Models
{
    public class SalesDetail
    {
        [Key]
        public int DetailId { get; set; }

        public int SalesId { get; set; }

        public int MedicineId { get; set; }

        [StringLength(50)]
        public string BatchNumber { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal LineTotal { get; set; }

        [ForeignKey("SalesId")]
        public SalesMaster? SalesMaster { get; set; }

        [ForeignKey("MedicineId")]
        public Medicine? Medicine { get; set; }
    }
}
