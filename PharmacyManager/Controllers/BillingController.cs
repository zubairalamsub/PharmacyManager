using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyManager.Data;
using PharmacyManager.Models;

namespace PharmacyManager.Controllers
{
    public class BillingController : Controller
    {
        private readonly PharmacyDbContext _context;

        public BillingController(PharmacyDbContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetString("Username") != null;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var invoices = await _context.SalesMasters
                .OrderByDescending(s => s.InvoiceDate)
                .ToListAsync();

            return View(invoices);
        }

        public async Task<IActionResult> Create()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var lastInvoice = await _context.SalesMasters
                .OrderByDescending(s => s.SalesId)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastInvoice != null)
            {
                string numPart = lastInvoice.InvoiceNumber.Replace("INV-", "");
                if (int.TryParse(numPart, out int parsed))
                    nextNumber = parsed + 1;
            }

            var model = new BillingViewModel
            {
                InvoiceNumber = "INV-" + nextNumber.ToString("D5"),
                InvoiceDate = DateTime.Now,
                Medicines = await _context.Medicines
                    .Where(m => m.StockQuantity > 0)
                    .OrderBy(m => m.Name)
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BillingViewModel model)
        {
            if (!IsLoggedIn())
                return Json(new { success = false, message = "Session expired. Please login again." });

            if (string.IsNullOrWhiteSpace(model.CustomerName))
                return Json(new { success = false, message = "Customer name is required." });

            if (model.Items == null || model.Items.Count == 0)
                return Json(new { success = false, message = "Please add at least one item to the invoice." });


            foreach (var item in model.Items)
            {
                var medicine = await _context.Medicines.FindAsync(item.MedicineId);
                if (medicine == null)
                    return Json(new { success = false, message = $"Medicine not found for ID {item.MedicineId}." });

                if (medicine.StockQuantity < item.Quantity)
                    return Json(new { success = false, message = $"Not enough stock for {medicine.Name}. Available: {medicine.StockQuantity}, Requested: {item.Quantity}" });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                decimal subTotal = model.Items.Sum(i => i.Quantity * i.UnitPrice);
                decimal grandTotal = subTotal - model.Discount;

                var salesMaster = new SalesMaster
                {
                    InvoiceNumber = model.InvoiceNumber,
                    InvoiceDate = model.InvoiceDate,
                    CustomerName = model.CustomerName,
                    ContactNumber = model.ContactNumber ?? "",
                    SubTotal = subTotal,
                    Discount = model.Discount,
                    GrandTotal = grandTotal
                };

                _context.SalesMasters.Add(salesMaster);
                await _context.SaveChangesAsync();

                foreach (var item in model.Items)
                {
                    var detail = new SalesDetail
                    {
                        SalesId = salesMaster.SalesId,
                        MedicineId = item.MedicineId,
                        BatchNumber = item.BatchNumber ?? "",
                        ExpiryDate = item.ExpiryDate,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        LineTotal = item.Quantity * item.UnitPrice
                    };
                    _context.SalesDetails.Add(detail);

                    // deduct stock
                    var medicine = await _context.Medicines.FindAsync(item.MedicineId);
                    if (medicine != null)
                    {
                        medicine.StockQuantity -= item.Quantity;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "Invoice saved successfully!", invoiceId = salesMaster.SalesId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Error saving invoice: " + ex.Message });
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var invoice = await _context.SalesMasters.FindAsync(id);
            if (invoice == null) return NotFound();

            var details = await _context.SalesDetails
                .Include(d => d.Medicine)
                .Where(d => d.SalesId == id)
                .ToListAsync();

            var viewModel = new InvoiceDetailViewModel
            {
                Invoice = invoice,
                Details = details
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetMedicineInfo(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null)
                return Json(new { success = false });

            return Json(new
            {
                success = true,
                batchNumber = medicine.BatchNumber,
                expiryDate = medicine.ExpiryDate.ToString("yyyy-MM-dd"),
                unitPrice = medicine.UnitPrice,
                stockQuantity = medicine.StockQuantity
            });
        }

        [HttpGet]
        public async Task<IActionResult> CheckStock(int medicineId, int quantity)
        {
            var medicine = await _context.Medicines.FindAsync(medicineId);
            if (medicine == null)
                return Json(new { available = false, message = "Medicine not found." });

            if (medicine.StockQuantity < quantity)
                return Json(new { available = false, message = $"Only {medicine.StockQuantity} units available." });

            return Json(new { available = true, message = "Stock available." });
        }
    }
}
