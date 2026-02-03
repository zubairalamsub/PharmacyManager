using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyManager.Models
{
    public class SalesMaster
    {
        [Key]
        public int SalesId { get; set; }

        [Required]
        [StringLength(20)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime InvoiceDate { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [StringLength(15)]
        public string ContactNumber { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Discount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal GrandTotal { get; set; }

        public ICollection<SalesDetail> SalesDetails { get; set; } = new List<SalesDetail>();
    }
}
