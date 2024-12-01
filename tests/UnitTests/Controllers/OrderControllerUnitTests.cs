using Application.Dtos.Orders;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Controllers;
using Xunit;

namespace UnitTests.Tests
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderAppService> _mockOrderAppService;
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            _mockOrderAppService = new Mock<IOrderAppService>();
            _controller = new OrderController(_mockOrderAppService.Object);
        }

        [Fact]
        public async Task GetOrders_ShouldReturnOk_WhenOrdersExist()
        {
            // Arrange
            var orders = new List<OrderOutputDto>
            {
                new OrderOutputDto { Id = Guid.NewGuid(), Status = OrderStatus.WaitPayment },
                new OrderOutputDto { Id = Guid.NewGuid(), Status = OrderStatus.InPreparation }
            };
            _mockOrderAppService.Setup(service => service.GetCurrrentOrders()).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetOrders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrders = Assert.IsType<List<OrderOutputDto>>(okResult.Value);
            Assert.Equal(2, returnedOrders.Count);
        }

        [Fact]
        public async Task GetOrders_ShouldReturnNoContent_WhenNoOrdersExist()
        {
            // Arrange
            _mockOrderAppService.Setup(service => service.GetCurrrentOrders()).ReturnsAsync(new List<OrderOutputDto>());

            // Act
            var result = await _controller.GetOrders();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetOrderById_ShouldReturnOk_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new OrderOutputDto { Id = orderId, Status = OrderStatus.WaitPayment };
            _mockOrderAppService.Setup(service => service.GetOrderByIdAsync(orderId)).ReturnsAsync(order);

            // Act
            var result = await _controller.GetOrderById(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrder = Assert.IsType<OrderOutputDto>(okResult.Value);
            Assert.Equal(orderId, returnedOrder.Id);
        }

        [Fact]
        public async Task MoveOrderToNextStatus_ShouldReturnOk_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var updatedOrder = new OrderOutputDto { Id = orderId, Status = OrderStatus.InPreparation };
            _mockOrderAppService.Setup(service => service.MoveOrderToNextStatus(orderId)).ReturnsAsync(updatedOrder);

            // Act
            var result = await _controller.MoveOrderToNextStatus(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrder = Assert.IsType<OrderOutputDto>(okResult.Value);
            Assert.Equal(OrderStatus.InPreparation, returnedOrder.Status);
        }

        [Fact]
        public async Task DeleteOrder_ShouldReturnNoContent_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _mockOrderAppService.Setup(service => service.DeleteOrderAsync(orderId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteOrder(orderId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetOrderByStatus_ShouldReturnOk_WhenOrdersExist()
        {
            // Arrange
            var status = OrderStatus.WaitPayment;
            var orders = new List<OrderOutputDto>
            {
                new OrderOutputDto { Id = Guid.NewGuid(), Status = status },
                new OrderOutputDto { Id = Guid.NewGuid(), Status = status }
            };
            _mockOrderAppService.Setup(service => service.GetOrderByStatusAsync(status)).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetOrderByStatus(status);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrders = Assert.IsType<List<OrderOutputDto>>(okResult.Value);
            Assert.Equal(2, returnedOrders.Count);
        }

        [Fact]
        public async Task GetOrderByStatus_ShouldReturnNoContent_WhenNoOrdersExist()
        {
            // Arrange
            var status = OrderStatus.WaitPayment;
            _mockOrderAppService.Setup(service => service.GetOrderByStatusAsync(status)).ReturnsAsync(new List<OrderOutputDto>());

            // Act
            var result = await _controller.GetOrderByStatus(status);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
