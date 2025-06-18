using MagicVilla_CouponAPI.Models;

namespace MagicVilla_CouponAPI.Repository
{
    public interface ICouponRepository
    {
        Task<IEnumerable<Coupon>> GetAllAsync();
        Task<Coupon> GetByIdAsync(int id);
        Task<Coupon> AddAsync(Coupon coupon);
        Task<bool> UpdateAsync(Coupon coupon);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
    }
}