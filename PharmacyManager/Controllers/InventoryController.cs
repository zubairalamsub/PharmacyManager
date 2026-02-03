using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyManager.Data;
using PharmacyManager.Models;
using ClosedXML.Excel;

namespace PharmacyManager.Controllers
{
    public class InventoryController : Controller
    {
        private readonly PharmacyDbContext _context;

        public InventoryController(PharmacyDbContext context)
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

            var medicines = await _context.Medicines.OrderBy(m => m.Name).ToListAsync();
            return View(medicines);
        }

        public IActionResult Create()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Medicine medicine)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                _context.Medicines.Add(medicine);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Medicine added successfully.";
                return RedirectToAction("Index");
            }
            return View(medicine);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null) return NotFound();
            return View(medicine);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null) return NotFound();
            return View(medicine);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Medicine medicine)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            if (id != medicine.MedicineId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(medicine);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Medicine updated successfully.";
                return RedirectToAction("Index");
            }
            return View(medicine);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null) return NotFound();
            return View(medicine);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine != null)
            {
                _context.Medicines.Remove(medicine);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Medicine deleted successfully.";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ExportToExcel()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var medicines = await _context.Medicines.OrderBy(m => m.Name).ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Inventory");

   
            worksheet.Cell(1, 1).Value = "Medicine Name";
            worksheet.Cell(1, 2).Value = "Category";
            worksheet.Cell(1, 3).Value = "Manufacturer";
            worksheet.Cell(1, 4).Value = "Batch No";
            worksheet.Cell(1, 5).Value = "Expiry Date";
            worksheet.Cell(1, 6).Value = "Stock Qty";
            worksheet.Cell(1, 7).Value = "Unit Price";

            var headerRange = worksheet.Range(1, 1, 1, 7);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            int row = 2;
            foreach (var m in medicines)
            {
                worksheet.Cell(row, 1).Value = m.Name;
                worksheet.Cell(row, 2).Value = m.Category;
                worksheet.Cell(row, 3).Value = m.Manufacturer;
                worksheet.Cell(row, 4).Value = m.BatchNumber;
                worksheet.Cell(row, 5).Value = m.ExpiryDate.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 6).Value = m.StockQuantity;
                worksheet.Cell(row, 7).Value = m.UnitPrice;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "InventoryReport_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
        }
    }
}
