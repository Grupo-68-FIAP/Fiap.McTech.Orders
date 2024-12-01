using AutoMapper;
using CrossCutting.Ioc.Mappers.Profiles;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace CrossCutting.Ioc.Mappers
{
    [ExcludeFromCodeCoverage]
    public static class AutoMapperConfiguration
    {
        public static void RegisterMappings(this IServiceCollection services)
        {
            MapperConfiguration config = new(cfg =>
            {
                // Register Profiles
                cfg.AddProfile<OrderProfile>();
            });

            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
