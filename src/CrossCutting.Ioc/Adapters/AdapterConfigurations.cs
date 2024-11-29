using Domain.Interfaces.ExternalServices;
using ExternalServices.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using System.Diagnostics.CodeAnalysis;

namespace CrossCutting.Ioc.Adapters
{
    [ExcludeFromCodeCoverage]
    public static class AdapterConfigurations
    {
        public static void AddAdapterServices(this IServiceCollection services, IConfiguration configuration)
        {
            var cartServiceBaseUrl = Environment.GetEnvironmentVariable("MCTECH_CART_SERVICE") 
                ?? GetBaseUrlFromConfiguration(configuration, "CartService:BaseUrl");

            var paymentServiceBaseUrl = Environment.GetEnvironmentVariable("MCTECH_PAYMENT_SERVICE") 
                ?? GetBaseUrlFromConfiguration(configuration, "PaymentService:BaseUrl");

            ConfigureHttpClientWithPolicies<ICartAdapter, CartAdapter>(services, cartServiceBaseUrl);
            ConfigureHttpClientWithPolicies<IPaymentAdapter, PaymentAdapter>(services, paymentServiceBaseUrl);
        }

        private static void ConfigureHttpClientWithPolicies<TInterface, TImplementation>(
            IServiceCollection services, string baseUrl)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("The base URL for the HTTP client cannot be null or empty.", nameof(baseUrl));

            services.AddHttpClient<TInterface, TImplementation>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler())
            .AddHttpMessageHandler(() => new PolicyHttpMessageHandler(GetRetryPolicy()))
            .AddHttpMessageHandler(() => new PolicyHttpMessageHandler(GetTimeoutPolicy()));
        }

        private static string GetBaseUrlFromConfiguration(IConfiguration configuration, string sectionKey)
        {
            var baseUrl = configuration[sectionKey];
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException($"Configuration for '{sectionKey}' is missing or empty.");
            return baseUrl;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );
        }

        private static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(10);
        }
    }
}
