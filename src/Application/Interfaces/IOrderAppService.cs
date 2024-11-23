using Application.Dtos.Create;
using Application.Dtos.Orders; 
using Domain.Enums;

namespace Application.Interfaces
{
    public interface IOrderAppService
    {
        Task<OrderOutputDto?> GetOrderByIdAsync(Guid id);
        Task<List<OrderOutputDto>> GetOrderByStatusAsync(OrderStatus status);
        Task<OrderOutputDto> CreateOrderByCartAsync(CreateOrderRequestDto request);
        Task DeleteOrderAsync(Guid orderId);
        Task<OrderOutputDto> MoveOrderToNextStatus(Guid id);
        Task<List<OrderOutputDto>> GetCurrrentOrders();
    }
}
