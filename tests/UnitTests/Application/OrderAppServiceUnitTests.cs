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
            var order = new Order(orderId, 150m); // Supondo que exista uma classe `Order`.
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
        public async Task GetOrderByIdAsync_WhenOrderDoesNotExist_ShouldThrowEntityNotFoundException()
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
        public async Task GetOrderByIdAsync_WhenOrderIdIsEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var emptyId = Guid.Empty;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _orderAppService.GetOrderByIdAsync(emptyId)
            );

            Assert.Equal("Order ID cannot be empty. (Parameter 'id')", exception.Message);
        }

        [Fact]
        public async Task CreateOrderByCartAsync_WhenRequestIsValid_ShouldCreateOrder()
        {
            // Arrange
            var request = new CreateOrderRequestDto
            {
                CartId = Guid.NewGuid(),
                ClientId = Guid.NewGuid(),
                Items = new List<CartItemRequestDto>
        {
            new CartItemRequestDto { ProductId = Guid.NewGuid(), Quantity = 2, Value = 50m }
        }
            };
            request.CalculateAllValue();

            var order = new Order(request.CartId, request.AllValue); // Supondo que exista uma classe `Order`.
            var createdOrder = new Order(Guid.NewGuid(), request.AllValue);
            var orderDto = new OrderOutputDto { Id = createdOrder.Id };

            _mapperMock.Setup(mapper => mapper.Map<Order>(request))
                .Returns(order);
            _orderRepositoryMock.Setup(repo => repo.AddAsync(order))
                .ReturnsAsync(createdOrder);
            _mapperMock.Setup(mapper => mapper.Map<OrderOutputDto>(createdOrder))
                .Returns(orderDto);

            // Act
            var result = await _orderAppService.CreateOrderByCartAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderDto.Id, result.Id);
            _cartAdapterMock.Verify(adapter => adapter.DeleteCartByIdAsync(request.CartId), Times.Once);
        }

        [Fact]
        public async Task CreateOrderByCartAsync_WhenCartIdIsEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var request = new CreateOrderRequestDto
            {
                CartId = Guid.Empty,
                ClientId = Guid.NewGuid()
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _orderAppService.CreateOrderByCartAsync(request)
            );

            Assert.Equal("Cart ID cannot be empty. (Parameter 'CartId')", exception.Message);
        }

        [Fact]
        public async Task DeleteOrderAsync_WhenOrderExists_ShouldDeleteOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId, 150m);

            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(orderId))
                .ReturnsAsync(order);
            _orderRepositoryMock.Setup(repo => repo.RemoveAsync(order))
                .Returns(Task.CompletedTask);

            // Act
            await _orderAppService.DeleteOrderAsync(orderId);

            // Assert
            _orderRepositoryMock.Verify(repo => repo.RemoveAsync(order), Times.Once);
        }

        [Fact]
        public async Task DeleteOrderAsync_WhenOrderIdIsEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var emptyId = Guid.Empty;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _orderAppService.DeleteOrderAsync(emptyId)
            );

            Assert.Equal("Order ID cannot be empty. (Parameter 'orderId')", exception.Message);
        }

        [Fact]
        public async Task DeleteOrderAsync_WhenOrderDoesNotExist_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(orderId))
                .ReturnsAsync((Order?) null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _orderAppService.DeleteOrderAsync(orderId)
            );

            Assert.Equal($"Order with ID {orderId} not found.", exception.Message);
        }

        [Fact]
        public async Task GetOrderByStatusAsync_WhenOrdersExist_ShouldReturnOrders()
        {
            // Arrange
            var status = OrderStatus.Received;
            var orders = new List<Order> { new Order(Guid.NewGuid(), 150m) };
            var ordersDto = new List<OrderOutputDto>
    {
        new OrderOutputDto { Id = orders[0].Id, Status = status }
    };

            _orderRepositoryMock.Setup(repo => repo.GetOrderByStatusAsync(status))
                .ReturnsAsync(orders);
            _mapperMock.Setup(mapper => mapper.Map<List<OrderOutputDto>>(orders))
                .Returns(ordersDto);

            // Act
            var result = await _orderAppService.GetOrderByStatusAsync(status);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(status, result[0].Status);
        }

        [Fact]
        public async Task GetOrderByStatusAsync_WhenNoOrdersExist_ShouldReturnEmptyList()
        {
            // Arrange
            var status = OrderStatus.Received;

            _orderRepositoryMock.Setup(repo => repo.GetOrderByStatusAsync(status))
                .ReturnsAsync(new List<Order>());

            // Act
            var result = await _orderAppService.GetOrderByStatusAsync(status);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task MoveOrderToNextStatus_WhenOrderExists_ShouldMoveToNextStatus()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId, 150m);

            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(orderId))
                .ReturnsAsync(order);
            _orderRepositoryMock.Setup(repo => repo.UpdateAsync(order))
                .Returns(Task.CompletedTask);
            _paymentAdapterMock.Setup(adapter => adapter.MoveOrderToNextStatus(orderId))
                .ReturnsAsync(true);
            _orderRepositoryMock.Setup(repo => repo.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);
            var orderDto = new OrderOutputDto { Id = orderId };

            _mapperMock.Setup(mapper => mapper.Map<OrderOutputDto>(order))
                .Returns(orderDto);

            // Act
            var result = await _orderAppService.MoveOrderToNextStatus(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.Id);
        }

        [Fact]
        public async Task MoveOrderToNextStatus_WhenPaymentFails_ShouldThrowApplicationException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId, 150m);

            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(orderId))
                .ReturnsAsync(order);
            _paymentAdapterMock.Setup(adapter => adapter.MoveOrderToNextStatus(orderId))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApplicationException>(
                () => _orderAppService.MoveOrderToNextStatus(orderId)
            );

            Assert.Contains("Payment status update failed", exception.Message);
        }
    }
}
