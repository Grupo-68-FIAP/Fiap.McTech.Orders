using Domain.Entities.Orders;
using ExternalServices.Adapters;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;

namespace UnitTests.Adapters
{
    public class PaymentAdapterUnitTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly Mock<ILogger<PaymentAdapter>> _loggerMock;
        private readonly PaymentAdapter _paymentAdapter;

        public PaymentAdapterUnitTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _loggerMock = new Mock<ILogger<PaymentAdapter>>();
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _paymentAdapter = new PaymentAdapter(httpClient, "https://fakepaymentservice.com", _loggerMock.Object);
        }

        [Fact]
        public async Task MoveOrderToNextStatus_WhenOrderIsSuccessfullyUpdated_ShouldReturnTrue()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri != null && req.RequestUri.ToString() == $"https://fakepaymentservice.com/payment/{orderId}/checkout"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            // Act
            var result = await _paymentAdapter.MoveOrderToNextStatus(orderId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task MoveOrderToNextStatus_WhenOrderIdIsEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var orderId = Guid.Empty;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _paymentAdapter.MoveOrderToNextStatus(orderId));
            Assert.Contains("Order ID cannot be empty", exception.Message);
        }

        [Fact]
        public async Task MoveOrderToNextStatus_WhenRequestFails_ShouldThrowException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri != null && req.RequestUri.ToString() == $"https://fakepaymentservice.com/payment/{orderId}/checkout"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _paymentAdapter.MoveOrderToNextStatus(orderId));
            Assert.Contains($"Error while communicating with the payment service for order ID {orderId}", exception.Message);
        }
    }
}
