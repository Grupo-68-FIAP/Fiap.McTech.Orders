using Domain.Entities.Orders;
using Domain.Enums;

namespace Domain.Interfaces.Repositories.Orders
{
    public interface IOrderRepository : IRepositoryBase<Order>
    {
        Task<Order?> GetOrderByIdAsync(Guid id);
        Task<List<Order>> GetOrderByStatusAsync(OrderStatus status);
        Task<List<Order>> GetCurrrentOrders();
    }
}
