using Application.Dtos.Orders;
using Application.Interfaces;
using AutoMapper;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces.Repositories.Orders;
using Microsoft.Extensions.Logging;

namespace AppServices.Orders
{
    public class OrderAppService : IOrderAppService
    {
        private readonly ILogger<OrderAppService> _logger; 
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderAppService(
            IOrderRepository orderRepository,  
            IMapper mapper,
            ILogger<OrderAppService> logger)
        {
            _orderRepository = orderRepository;  
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderOutputDto?> GetOrderByIdAsync(Guid id)
        {
            _logger.LogInformation("Retrieving order with ID {OrderId}.", id);

            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                _logger.LogInformation("Order with ID {OrderId} not found.", id);
                throw new EntityNotFoundException(string.Format("Order with ID {0} not found.", id));
            }

            _logger.LogInformation("Order with ID {OrderId} retrieved successfully.", id);

            return _mapper.Map<OrderOutputDto>(order);
        }

        public Task<OrderOutputDto> CreateOrderByCartAsync(Guid cartId)
        {
            //TODO - RECEBER CARRINHO VIA REQUEST

            //TODO - SALVAR NO BANCO DE ORDER

            //TODO - ADICIONAR CHAMADA HTTP PARA DELETAR O CARRINHO 

            throw new NotImplementedException();
        }

        public async Task DeleteOrderAsync(Guid orderId)
        {
            _logger.LogInformation("Attempting to delete order with ID: {OrderId}.", orderId);

            var existingOrder = await _orderRepository.GetByIdAsync(orderId)
                ?? throw new EntityNotFoundException(string.Format("Order with ID {0} not found.", orderId));

            await _orderRepository.RemoveAsync(existingOrder);

            _logger.LogInformation("Order with ID {OrderId} deleted successfully.", orderId);
        }

        public async Task<List<OrderOutputDto>> GetOrderByStatusAsync(OrderStatus status)
        {
            _logger.LogInformation("Retrieving order with status code {Status}.", status.ToString());

            var orders = await _orderRepository.GetOrderByStatusAsync(status);
            if (!orders.Any())
                return new List<OrderOutputDto>();

            _logger.LogInformation("Order with status code {Status} retrieved successfully.", status.ToString());

            return _mapper.Map<List<OrderOutputDto>>(orders);
        }

        public async Task<OrderOutputDto> MoveOrderToNextStatus(Guid id)
        {
            var originalOrder = await _orderRepository.GetByIdAsync(id)
                ?? throw new EntityNotFoundException(string.Format("Order with ID {0} not found. Update aborted.", id));

            //TODO - REMOVER E ADICIONAR CHAMADA AO SERVIÇO EXTERNO DE PAGAMENTOS PARA ATUALIZAR STATUS
             
            originalOrder.SendToNextStatus();

            await _orderRepository.UpdateAsync(originalOrder);

            return _mapper.Map<OrderOutputDto>(await _orderRepository.GetOrderByIdAsync(id));
        }

        public async Task<List<OrderOutputDto>> GetCurrrentOrders()
        {
            _logger.LogInformation("Retrieving all orders.");

            var orders = await _orderRepository.GetCurrrentOrders();

            if (orders == null || !orders.Any())
                return new List<OrderOutputDto>();

            _logger.LogInformation("All orders retrieved successfully.");

            return _mapper.Map<List<OrderOutputDto>>(orders);
        }
    }
}
