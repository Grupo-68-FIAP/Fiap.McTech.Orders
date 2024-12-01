using Application.Dtos;
using Application.Dtos.Orders;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities.Orders;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces.ExternalServices;
using Domain.Interfaces.Repositories.Orders;
using Microsoft.Extensions.Logging;

namespace AppServices.Orders
{
    public class OrderAppService : IOrderAppService
    {
        private readonly ILogger<OrderAppService> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartAdapter _cartAdapter;
        private readonly IPaymentAdapter _paymentAdapter;
        
        private readonly IMapper _mapper;

        public OrderAppService(
            IOrderRepository orderRepository,
            IMapper mapper,
            ICartAdapter cartAdapter,
            IPaymentAdapter paymentAdapter,
            ILogger<OrderAppService> logger)
        {
            _orderRepository = orderRepository;
            _cartAdapter = cartAdapter;
            _paymentAdapter = paymentAdapter;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderOutputDto> GetOrderByIdAsync(Guid id)
        {
            _logger.LogInformation("Retrieving order with ID {OrderId}.", id);

            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid Order ID provided: {OrderId}.", id);
                throw new ArgumentException("Order ID cannot be empty.", nameof(id));
            }

            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found.", id);
                throw new EntityNotFoundException($"Order with ID {id} not found.");
            }

            _logger.LogInformation("Order with ID {OrderId} retrieved successfully.", id);

            return _mapper.Map<OrderOutputDto>(order);
        }

        public async Task<OrderOutputDto> CreateOrderByCartAsync(CreateOrderRequestDto request)
        {
            if (request.CartId == Guid.Empty)
            {
                _logger.LogWarning("Invalid Cart ID provided: {CartId}.", request.CartId);
                throw new InvalidCartIdException("Cart ID cannot be empty."); 
            }


            _logger.LogInformation("Creating order for cart with ID {CartId}.", request.CartId);

            try
            {
                var order = _mapper.Map<Order>(request);

                var createdOrder = await _orderRepository.AddAsync(order);
                _logger.LogInformation("Order created successfully with ID {OrderId}.", createdOrder.Id);

                _ = _cartAdapter.DeleteCartByIdAsync(request.CartId);

                _logger.LogInformation("Cart with ID {CartId} deleted after order creation.", request.CartId);

                return _mapper.Map<OrderOutputDto>(createdOrder);
            }
            catch (InvalidCartIdException ex)
            {
                _logger.LogError(ex, "Error creating order for cart with ID {CartId}.", request.CartId);
                throw;
            }
        }

        public async Task DeleteOrderAsync(Guid orderId)
        {
            _logger.LogInformation("Attempting to delete order with ID: {OrderId}.", orderId);

            if (orderId == Guid.Empty)
            {
                _logger.LogWarning("Invalid Order ID provided: {OrderId}.", orderId);
                throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));
            }

            var existingOrder = await _orderRepository.GetByIdAsync(orderId)
                ?? throw new EntityNotFoundException($"Order with ID {orderId} not found.");

            await _orderRepository.RemoveAsync(existingOrder);

            _logger.LogInformation("Order with ID {OrderId} deleted successfully.", orderId);
        }

        public async Task<List<OrderOutputDto>> GetOrderByStatusAsync(OrderStatus status)
        {
            _logger.LogInformation("Retrieving orders with status {Status}.", status.ToString().Replace(Environment.NewLine, ""));

            if (!Enum.IsDefined(typeof(OrderStatus), status))
            {
                _logger.LogWarning("Invalid order status provided: {Status}.", status.ToString().Replace(Environment.NewLine, ""));
                throw new ArgumentException("Invalid order status.", nameof(status));
            }

            var orders = await _orderRepository.GetOrderByStatusAsync(status);
            if (!orders.Any())
            {
                _logger.LogInformation("No orders found with status {Status}.", status.ToString().Replace(Environment.NewLine, ""));
                return new List<OrderOutputDto>();
            }

            _logger.LogInformation("Orders with status {Status} retrieved successfully.", status.ToString().Replace(Environment.NewLine, ""));

            return _mapper.Map<List<OrderOutputDto>>(orders);
        }

        public async Task<OrderOutputDto> MoveOrderToNextStatus(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid Order ID provided: {OrderId}.", id);
                throw new ArgumentException("Order ID cannot be empty.", nameof(id));
            }

            _logger.LogInformation("Attempting to move order with ID {OrderId} to next status.", id);


            var originalOrder = await _orderRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException($"Order with ID {id} not found. Update aborted.");

            try
            {
                originalOrder.SendToNextStatus();
                await _orderRepository.UpdateAsync(originalOrder);

                _logger.LogInformation("Order with ID {OrderId} moved to next status successfully.", id);

                var paymentStatusUpdate = await _paymentAdapter.MoveOrderToNextStatus(id);
                if (!paymentStatusUpdate)
                {
                    _logger.LogWarning("Failed to update payment status for order ID {OrderId}.", id);
                    throw new PaymentStatusUpdateException($"Payment status update failed for order ID {id}. Status transition incomplete.");
                }

                var updatedOrder = await _orderRepository.GetOrderByIdAsync(id);
                _logger.LogInformation("Order with ID {OrderId} retrieved after status update.", id);

                return _mapper.Map<OrderOutputDto>(updatedOrder);
            }
            catch (PaymentStatusUpdateException ex)
            {
                _logger.LogError(ex, "Error moving order with ID {OrderId} to next status.", id);
                throw;
            }
        }

        public async Task<List<OrderOutputDto>> GetCurrrentOrders()
        {
            _logger.LogInformation("Retrieving all current orders.");

            var orders = await _orderRepository.GetCurrrentOrders();

            if (orders == null || !orders.Any())
            {
                _logger.LogInformation("No current orders found.");
                return new List<OrderOutputDto>();
            }

            _logger.LogInformation("All current orders retrieved successfully.");

            return _mapper.Map<List<OrderOutputDto>>(orders);
        }
    }
}
