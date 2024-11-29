using ExternalServices.Adapters;
using Microsoft.Extensions.Logging;
using Moq.Protected;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Adapters
{
    public class CartAdapterUnitTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly Mock<ILogger<CartAdapter>> _loggerMock;
        private readonly CartAdapter _cartAdapter;

        public CartAdapterUnitTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _loggerMock = new Mock<ILogger<CartAdapter>>();
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _cartAdapter = new CartAdapter(httpClient, "https://fakecartservice.com", _loggerMock.Object);
        }

        [Fact]
        public async Task DeleteCartByIdAsync_WhenDeletionIsSuccessful_ShouldReturnTrue()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Delete && req.RequestUri.ToString() == $"https://fakecartservice.com/cart/{cartId}"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            // Act
            var result = await _cartAdapter.DeleteCartByIdAsync(cartId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteCartByIdAsync_WhenDeletionFails_ShouldReturnFalse()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Delete && req.RequestUri.ToString() == $"https://fakecartservice.com/cart/{cartId}"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            // Act
            var result = await _cartAdapter.DeleteCartByIdAsync(cartId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteCartByIdAsync_WhenHttpRequestExceptionOccurs_ShouldThrowException()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Request failed"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _cartAdapter.DeleteCartByIdAsync(cartId));
            Assert.Contains("Error occurred while sending HTTP request", exception.Message);
        }

        [Fact]
        public async Task DeleteCartByIdAsync_WhenTimeoutExceptionOccurs_ShouldThrowException()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new TimeoutException("Request timed out"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _cartAdapter.DeleteCartByIdAsync(cartId));
            Assert.Contains("Request to delete cart with ID", exception.Message);
        }

        [Fact]
        public async Task DeleteCartByIdAsync_WhenGenericExceptionOccurs_ShouldThrowException()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _cartAdapter.DeleteCartByIdAsync(cartId));
            Assert.Contains("An unexpected error occurred", exception.Message);
        }
    }
}
