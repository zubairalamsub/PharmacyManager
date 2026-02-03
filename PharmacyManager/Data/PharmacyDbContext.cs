using Microsoft.EntityFrameworkCore;
using PharmacyManager.Models;

namespace PharmacyManager.Data
{
    public class PharmacyDbContext : DbContext
    {
        public PharmacyDbContext(DbContextOptions<PharmacyDbContext> options) : base(options)
        {
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<SalesMaster> SalesMasters { get; set; }
        public DbSet<SalesDetail> SalesDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SalesMaster>()
                .HasMany(s => s.SalesDetails)
                .WithOne(d => d.SalesMaster)
                .HasForeignKey(d => d.SalesId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SalesDetail>()
                .HasOne(d => d.Medicine)
                .WithMany()
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
