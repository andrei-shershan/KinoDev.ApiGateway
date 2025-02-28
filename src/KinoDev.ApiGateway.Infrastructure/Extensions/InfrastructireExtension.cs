using KinoDev.ApiGateway.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace KinoDev.ApiGateway.Infrastructure.Extensions
{
    public static class InfrastructireExtension
    {
        public static IServiceCollection InitializeInfrastructure(this IServiceCollection services)
        {
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            services.AddMemoryCache();
            services.AddSingleton<ICacheProvider, MemoryCacheProvider>();

            return services;
        }
    }
}
