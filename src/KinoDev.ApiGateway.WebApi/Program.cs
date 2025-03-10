using KinoDev.ApiGateway.WebApi.SetupExtensions;
using Microsoft.IdentityModel.Protocols.Configuration;
using KinoDev.ApiGateway.Infrastructure.Extensions;
using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.ApiGateway.Infrastructure.Models.ConfigurationSettings;

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

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    // .AllowCredentials();
                });
            });

            var authenticationSettings = builder.Configuration.GetSection("Authentication").Get<AuthenticationSettings>();
            var apiClients = builder.Configuration.GetSection("ApiClients").Get<ApiClientsSettings>();
            var appBuilderSettigns = builder.Configuration.GetSection("AppBuilder").Get<AppBuilderSettigns>();
            if (authenticationSettings == null || apiClients == null || appBuilderSettigns == null)
            {
                throw new InvalidConfigurationException("Cannot obtain AuthenticationSettings from settings!");
            }

            builder.Services.Configure<AuthenticationSettings>(builder.Configuration.GetSection("Authentication"));

            builder.Services.SetupAuthentication(authenticationSettings);

            builder.Services.InitializeInfrastructure();

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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();
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
