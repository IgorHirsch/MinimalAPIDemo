using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_CouponAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _db;

        public CouponRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Coupon>> GetAllAsync()
            => await _db.Coupons.ToListAsync();

        public async Task<Coupon> GetByIdAsync(int id)
            => await _db.Coupons.FindAsync(id);

        public async Task<Coupon> AddAsync(Coupon coupon)
        {
            _db.Coupons.Add(coupon);
            await _db.SaveChangesAsync();
            return coupon;
        }

        public async Task<bool> UpdateAsync(Coupon coupon)
        {
            _db.Coupons.Update(coupon);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var coupon = await _db.Coupons.FindAsync(id);
            if (coupon == null) return false;
            _db.Coupons.Remove(coupon);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsByNameAsync(string name)
            => await _db.Coupons.AnyAsync(c => c.Name.ToLower() == name.ToLower());
    }
}