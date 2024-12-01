using Domain.Exceptions;
using Domain.Interfaces.ExternalServices;
using Microsoft.Extensions.Logging;

namespace ExternalServices.Adapters
{
    public class CartAdapter : ICartAdapter
    {
        private readonly HttpClient _httpClient;
        private readonly string _cartServiceBaseUrl;
        private readonly ILogger<CartAdapter> _logger;

        public CartAdapter(HttpClient httpClient, string cartServiceBaseUrl, ILogger<CartAdapter> logger)
        {
            _httpClient = httpClient;
            _cartServiceBaseUrl = cartServiceBaseUrl;
            _logger = logger;
        }

        public async Task<bool> DeleteCartByIdAsync(Guid cartId)
        {
            try
            {
                _logger.LogInformation("Attempting to delete cart with ID {CartId}.", cartId);

                var response = await _httpClient.DeleteAsync($"{_cartServiceBaseUrl}/cart/{cartId}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to delete cart with ID {CartId}. Status code: {StatusCode}.", cartId, response.StatusCode);
                    return false;
                }

                _logger.LogInformation("Cart with ID {CartId} deleted successfully.", cartId);
                return true;
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Error occurred while sending HTTP request to delete cart with ID {CartId}.", cartId);
                throw new CartDeletionException($"Error occurred while sending HTTP request to delete cart with ID {cartId}.", httpEx);
            }
            catch (TimeoutException timeoutEx)
            {
                _logger.LogError(timeoutEx, "Request to delete cart with ID {CartId} timed out.", cartId);
                throw new CartDeletionException($"Request to delete cart with ID {cartId} timed out.", timeoutEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while trying to delete cart with ID {CartId}.", cartId);
                throw new CartDeletionException($"An unexpected error occurred while trying to delete cart with ID {cartId}.", ex);
            }
        }
    }
}
