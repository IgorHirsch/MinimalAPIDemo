using AutoMapper;

namespace MagicVilla_CouponAPI.Models.DTO
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Coupon <-> CouponDTO (bidirektional)
            CreateMap<Coupon, CouponDTO>().ReverseMap();
            // CouponCreateDTO <-> Coupon (bidirektional)
            CreateMap<CouponCreateDTO, Coupon>().ReverseMap();
        }
    }
}