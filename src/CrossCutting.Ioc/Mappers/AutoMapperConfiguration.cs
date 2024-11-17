using AutoMapper;
using CrossCutting.Ioc.Mappers.Profiles;
using Microsoft.Extensions.DependencyInjection;

namespace CrossCutting.Ioc.Mappers
{
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
