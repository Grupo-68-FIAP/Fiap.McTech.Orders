using Application.Dtos;
using Application.Dtos.Orders;
using Application.Interfaces;
using AppServices.Orders;
using AutoMapper;
using Domain.Entities.Orders;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces.ExternalServices;
using Domain.Interfaces.Repositories.Orders;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppServices.UnitTests
{
    public class OrderAppServiceUnitTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<ICartAdapter> _cartAdapterMock;
        private readonly Mock<IPaymentAdapter> _paymentAdapterMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<OrderAppService>> _loggerMock;

        private readonly OrderAppService _orderAppService;

        public OrderAppServiceUnitTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _cartAdapterMock = new Mock<ICartAdapter>();
            _paymentAdapterMock = new Mock<IPaymentAdapter>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<OrderAppService>>();

            _orderAppService = new OrderAppService(
                _orderRepositoryMock.Object,
                _mapperMock.Object,
                _cartAdapterMock.Object,
                _paymentAdapterMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetOrderByIdAsync_WhenOrderExists_ShouldReturnOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(Guid.NewGuid(), 100m);  
            var orderDto = new OrderOutputDto { Id = orderId };

            _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);
            _mapperMock.Setup(mapper => mapper.Map<OrderOutputDto>(order))
                .Returns(orderDto);

            // Act
            var result = await _orderAppService.GetOrderByIdAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result?.Id);
        }

        [Fact]
        public async Task GetOrderByIdAsync_WhenOrderDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(orderId))
                .ReturnsAsync((Order?) null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _orderAppService.GetOrderByIdAsync(orderId)
            );
            Assert.Equal($"Order with ID {orderId} not found.", exception.Message);
        }

        [Fact]
        public async Task CreateOrderByCartAsync_WhenCartIdIsValid_ShouldCreateOrder()
        {
            // Arrange
            var request = new CreateOrderRequestDto { CartId = Guid.NewGuid() };
            var order = new Order(Guid.NewGuid(), 100m);  // Instanciando com clientId e totalAmount
            var orderDto = new OrderOutputDto();

            _mapperMock.Setup(mapper => mapper.Map<Order>(request))
                .Returns(order);
            _orderRepositoryMock.Setup(repo => repo.AddAsync(order))
                .ReturnsAsync(order);
            _mapperMock.Setup(mapper => mapper.Map<OrderOutputDto>(order))
                .Returns(orderDto);

            // Act
            var result = await _orderAppService.CreateOrderByCartAsync(request);

            // Assert
            Assert.NotNull(result);
            _cartAdapterMock.Verify(cart => cart.DeleteCartByIdAsync(request.CartId), Times.Once);
        }

        [Fact]
        public async Task CreateOrderByCartAsync_WhenCartIdIsEmpty_ShouldThrowException()
        {
            // Arrange
            var request = new CreateOrderRequestDto { CartId = Guid.Empty };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _orderAppService.CreateOrderByCartAsync(request)
            );
            Assert.Equal("Cart ID cannot be empty. (Parameter 'CartId')", exception.Message);
        }

        [Fact]
        public async Task MoveOrderToNextStatus_WhenOrderExists_ShouldUpdateStatus()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(Guid.NewGuid(), 100m);

            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(orderId))
                .ReturnsAsync(order);
            _paymentAdapterMock.Setup(adapter => adapter.MoveOrderToNextStatus(orderId))
                .ReturnsAsync(true);

            // Act
            await _orderAppService.MoveOrderToNextStatus(orderId);

            // Assert
            _orderRepositoryMock.Verify(repo => repo.UpdateAsync(order), Times.Once);
            Assert.Equal(OrderStatus.Received, order.Status);  
        }

        [Fact]
        public async Task MoveOrderToNextStatus_WhenOrderDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(orderId))
                .ReturnsAsync((Order?) null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _orderAppService.MoveOrderToNextStatus(orderId)
            );
            Assert.Equal($"Order with ID {orderId} not found. Update aborted.", exception.Message);
        }
    }
}
