using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyManager.Data;
using PharmacyManager.Models;

namespace PharmacyManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly PharmacyDbContext _context;

        public HomeController(PharmacyDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Account");

            var fullName = HttpContext.Session.GetString("FullName") ?? "User";

            var medicineStats = await _context.Medicines
                .GroupBy(m => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    LowStock = g.Count(m => m.StockQuantity < 50)
                })
                .FirstOrDefaultAsync();

            var totalInvoices = await _context.SalesMasters.CountAsync();

            var recentSales = await _context.SalesMasters
                .OrderByDescending(s => s.InvoiceDate)
                .Take(5)
                .ToListAsync();

            var model = new DashboardViewModel
            {
                FullName = fullName,
                TotalMedicines = medicineStats?.Total ?? 0,
                LowStockCount = medicineStats?.LowStock ?? 0,
                TotalInvoices = totalInvoices,
                RecentSales = recentSales
            };

            return View(model);
        }



        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
