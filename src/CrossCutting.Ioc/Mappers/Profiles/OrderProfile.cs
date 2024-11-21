using Application.Dtos.Orders;
using AutoMapper;
using Domain.Entities.Orders;

namespace CrossCutting.Ioc.Mappers.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            // Mapeamento de Order para OrderOutputDto
            CreateMap<Order, OrderOutputDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<Order.Item, OrderOutputDto.Item>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Price * src.Quantity));
        }
    }
}
