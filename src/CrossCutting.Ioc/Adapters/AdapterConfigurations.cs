using Domain.Interfaces.ExternalServices;
using ExternalServices.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.Ioc.Adapters
{
    public static class AdapterConfigurations
    {
        public static void AddAdapterServices(this IServiceCollection services, IConfiguration configuration)
        {
            var cartService= configuration.GetSection("CartService:BaseUrl");
            services.AddHttpClient<ICartAdapter, CartAdapter>(client =>
            {
                client.BaseAddress = new Uri(cartService.Value); 
            });

            var paymentService = configuration.GetSection("PaymentService:BaseUrl");
            services.AddHttpClient<IPaymentAdapter, PaymentAdapter>(client =>
            {
                client.BaseAddress = new Uri(paymentService.Value); 
            });
        }
    }
}
