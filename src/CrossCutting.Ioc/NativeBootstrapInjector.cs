using Application.Interfaces;
using AppServices.Orders;
using CrossCutting.Ioc.Infra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.Ioc
{
    public static class NativeBootstrapInjector
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Infra 
            services.ConfigureSqlServer(configuration);
            services.RegisterRepositories();

            //APP Services 
            services.AddScoped<IOrderAppService, OrderAppService>(); 
        }
    }
}
