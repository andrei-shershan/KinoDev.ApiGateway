using KinoDev.ApiGateway.WebApi.SetupExtensions;
using Microsoft.IdentityModel.Protocols.Configuration;
using KinoDev.ApiGateway.Infrastructure.Extensions;
using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.ApiGateway.Infrastructure.Models.ConfigurationSettings;
using KinoDev.Shared.Models;
using KinoDev.ApiGateway.Infrastructure.HttpClients.Abstractions;
using KinoDev.ApiGateway.WebApi.Models;

namespace KinoDev.ApiGateway.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddOutputCache();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var corsAllowedOrigins = builder.Configuration.GetValue<string>("CORSAllowedOrigins");
            if (string.IsNullOrWhiteSpace(corsAllowedOrigins))
            {
                throw new InvalidConfigurationException("CORSAllowedOrigins is not set in the configuration!");
            }

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(corsAllowedOrigins.Split(","))
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            var authenticationSettings = builder.Configuration.GetSection("Authentication").Get<AuthenticationSettings>();
            var apiClients = builder.Configuration.GetSection("ApiClients").Get<ApiClientsSettings>();
            var appBuilderSettigns = builder.Configuration.GetSection("AppBuilder").Get<AppBuilderSettigns>();
            var cookieResponseSettings = builder.Configuration.GetSection("CookieResponse").Get<CookieResponseSettings>();
            var rabbitMqSettings = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqSettings>();
            var messageBrokerSettings = builder.Configuration.GetSection("MessageBroker").Get<MessageBrokerSettings>();
            var portalSettings = builder.Configuration.GetSection("PortalSettings").Get<PortalSettings>();
            if (
                authenticationSettings == null
                || apiClients == null
                || appBuilderSettigns == null
                || cookieResponseSettings == null
                || rabbitMqSettings == null
                || messageBrokerSettings == null
                || portalSettings == null)
            {
                throw new InvalidConfigurationException("Cannot obtain from settings!");
            }

            builder.Services.Configure<AuthenticationSettings>(builder.Configuration.GetSection("Authentication"));
            builder.Services.Configure<CookieResponseSettings>(builder.Configuration.GetSection("CookieResponse"));
            builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
            builder.Services.Configure<MessageBrokerSettings>(builder.Configuration.GetSection("MessageBroker"));
            builder.Services.Configure<PortalSettings>(builder.Configuration.GetSection("PortalSettings"));

            builder.Services.SetupAuthentication(authenticationSettings);

            builder.Services.InitializeInfrastructure(builder.Configuration);

            builder.Services.AddTransient<InternalAuthenticationDelegationHandler>();

            builder.Services
                .AddHttpClient<IAuthenticationClient, AuthenticationClient>(options =>
                {
                    options.BaseAddress = new Uri(apiClients.IdentityServiceUri);
                })
                .ConfigurePrimaryHttpMessageHandler(() => HttpClientHandlerFactory.CreateHandler(appBuilderSettigns.IgnoreSslErrors));

            builder.Services
                .AddHttpClient<IDomainServiceClient, DomainServiceClient>(options =>
                {
                    options.BaseAddress = new Uri(apiClients.DomainServiceUri);
                })
                .ConfigurePrimaryHttpMessageHandler(() => HttpClientHandlerFactory.CreateHandler(appBuilderSettigns.IgnoreSslErrors))
                .AddHttpMessageHandler<InternalAuthenticationDelegationHandler>();

            builder.Services
                .AddHttpClient<IPaymentClient, PaymentClient>(options =>
                {
                    options.BaseAddress = new Uri(apiClients.PaymentServiceUri);
                })
                .ConfigurePrimaryHttpMessageHandler(() => HttpClientHandlerFactory.CreateHandler(appBuilderSettigns.IgnoreSslErrors))
                .AddHttpMessageHandler<InternalAuthenticationDelegationHandler>();

            builder.Services
                .AddHttpClient<IEmailServiceClient, EmailServiceClient>(options =>
                {
                    options.BaseAddress = new Uri(apiClients.EmailServiceUri);
                })
                .ConfigurePrimaryHttpMessageHandler(() => HttpClientHandlerFactory.CreateHandler(appBuilderSettigns.IgnoreSslErrors))
                .AddHttpMessageHandler<InternalAuthenticationDelegationHandler>();

            builder.Services
                .AddHttpClient<IStorageServiceClient, StorageServiceClient>(options =>
                {
                    options.BaseAddress = new Uri(apiClients.StorageServiceUri);
                })
                .ConfigurePrimaryHttpMessageHandler(() => HttpClientHandlerFactory.CreateHandler(appBuilderSettigns.IgnoreSslErrors));
            // .AddHttpMessageHandler<InternalAuthenticationDelegationHandler>();

            builder.Services.AddMemoryCache();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            var disableHttpsRedirection = builder.Configuration.GetValue<bool>("DisableHttpsRedirection");
            if (!disableHttpsRedirection)
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseCors(); // Ensure CORS middleware is used

            app.UseAuthentication();
            app.UseOutputCache();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
