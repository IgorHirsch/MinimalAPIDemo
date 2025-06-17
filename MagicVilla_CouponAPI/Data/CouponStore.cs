using MagicVilla_CouponAPI.Models;

namespace MagicVilla_CouponAPI.Data
{
    public static class CouponStore
    {
        public static List<Coupon> couponList = new List<Coupon>
        {
            new Coupon
            {
                Id = 1,
                Name = "10% Discount",
                Percent = 10,
                IsActive = true,
                Created = DateTime.Now,
                LastUpdated = DateTime.Now
            },
            new Coupon
            {
                Id = 2,
                Name = "20% Discount",
                Percent = 20,
                IsActive = true,
                Created = DateTime.Now,
                LastUpdated = DateTime.Now
            },
            new Coupon
            {
                Id = 3,
                Name = "30% Discount",
                Percent = 30,
                IsActive = false,
                Created = DateTime.Now.AddDays(-10),
                LastUpdated = DateTime.Now.AddDays(-5)
            }
        };
    }
}
