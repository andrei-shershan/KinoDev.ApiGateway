using KinoDev.ApiGateway.Infrastructure.Services;
using KinoDev.ApiGateway.Infrastructure.Services.Abstractions;
using KinoDev.Shared.Services;
using KinoDev.Shared.Services.Abstractions;
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
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            services.AddMemoryCache();
            services.AddSingleton<ICacheProvider, MemoryCacheProvider>();

            services.AddTransient<ICookieResponseService, CookieResponseService>();

            services.AddSingleton<IMessageBrokerService, RabbitMQService>();

            services.AddScoped<ICacheKeyService, CacheKeyService>();

            return services;
        }
    }
}
