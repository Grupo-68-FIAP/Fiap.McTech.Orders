using Application.Dtos;
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

            CreateMap<CreateOrderRequestDto, Order>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ForCtorParam("clientId", opt => opt.MapFrom(src => src.ClientId))
                .ForCtorParam("totalAmount", opt => opt.MapFrom(src => src.AllValue));

            CreateMap<CartItemRequestDto, Order.Item>()
                .ForCtorParam("productId", opt => opt.MapFrom(src => src.ProductId))
                .ForCtorParam("productName", opt => opt.MapFrom(src => src.Name))
                .ForCtorParam("price", opt => opt.MapFrom(src => src.Value))
                .ForCtorParam("quantity", opt => opt.MapFrom(src => src.Quantity))
                .ForCtorParam("orderId", opt => opt.ExplicitExpansion());
        }
    }
}
