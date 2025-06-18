using MagicVilla_CouponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_CouponAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed-Daten für Coupons mit fest kodierten (statischen) Datumswerten
            modelBuilder.Entity<Coupon>().HasData(
                new Coupon
                {
                    Id = 1,
                    Name = "10% Discount",
                    Percent = 10,
                    IsActive = true,
                    Created = new DateTime(2025, 6, 18, 7, 39, 44, 716, DateTimeKind.Utc),
                    LastUpdated = new DateTime(2025, 6, 18, 7, 39, 44, 716, DateTimeKind.Utc)
                },
                new Coupon
                {
                    Id = 2,
                    Name = "20% Discount",
                    Percent = 20,
                    IsActive = true,
                    Created = new DateTime(2025, 6, 18, 7, 39, 44, 716, DateTimeKind.Utc),
                    LastUpdated = new DateTime(2025, 6, 18, 7, 39, 44, 716, DateTimeKind.Utc)
                },
                new Coupon
                {
                    Id = 3,
                    Name = "30% Discount",
                    Percent = 30,
                    IsActive = false,
                    Created = new DateTime(2025, 6, 8, 7, 39, 44, 716, DateTimeKind.Utc),
                    LastUpdated = new DateTime(2025, 6, 13, 7, 39, 44, 716, DateTimeKind.Utc)
                }
            );
        }
    }
}