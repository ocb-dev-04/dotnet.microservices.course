using AutoMapper;
using Discount.Grpc.Protos;
using Discount.GRPC.Entities;

namespace Discount.GRPC.Mapper
{
    public class DiscountMapper : Profile
    {
        public DiscountMapper()
        {
            CreateMap<Coupon, CouponModel>().ReverseMap();
        }
    }
}
