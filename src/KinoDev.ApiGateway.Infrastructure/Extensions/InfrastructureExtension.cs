using KinoDev.ApiGateway.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace KinoDev.ApiGateway.Infrastructure.Extensions
{
    public static class InfrastructureExtension
    {
        public static IServiceCollection InitializeInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            services.AddMemoryCache();
            services.AddSingleton<ICacheProvider, MemoryCacheProvider>();

            services.AddSerilogServices(configuration);

            return services;
        }
    }
}
