using Domain.Interfaces.ExternalServices;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace ExternalServices.Adapters
{
    [ExcludeFromCodeCoverage]
    public class PaymentAdapter : IPaymentAdapter
    {
        private readonly HttpClient _httpClient;
        private readonly string _paymentServiceBaseUrl;
        private readonly ILogger<PaymentAdapter> _logger;

        public PaymentAdapter(HttpClient httpClient, string paymentServiceBaseUrl, ILogger<PaymentAdapter> logger)
        {
            _httpClient = httpClient;
            _paymentServiceBaseUrl = paymentServiceBaseUrl;
            _logger = logger;
        }

        public async Task<bool> MoveOrderToNextStatus(Guid orderId)
        {
            _logger.LogInformation("Attempting to move order with ID {OrderId} to next payment status.", orderId);

            if (orderId == Guid.Empty)
            {
                _logger.LogWarning("Invalid Order ID provided: {OrderId}.", orderId);
                throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));
            }

            try
            {
                var response = await _httpClient.PostAsync($"{_paymentServiceBaseUrl}/payment/{orderId}/next-status", null);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to move order with ID {OrderId} to next payment status. Status code: {StatusCode}.", orderId, response.StatusCode);
                    throw new HttpRequestException($"Failed to move order with ID {orderId} to next payment status. Status code: {response.StatusCode}");
                }

                _logger.LogInformation("Order with ID {OrderId} moved to next payment status successfully.", orderId);

                return true; 
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Error while communicating with the payment service for order ID {OrderId}.", orderId);
                throw new HttpRequestException($"Error while communicating with the payment service for order ID {orderId}.", httpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while processing order ID {OrderId} for payment status change.", orderId);
                throw new Exception($"Unexpected error occurred while processing order ID {orderId} for payment status change.", ex);
            }
        }
    }
}
