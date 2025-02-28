using KinoDev.ApiGateway.WebApi.SetupExtensions;
using Microsoft.IdentityModel.Protocols.Configuration;
using KinoDev.ApiGateway.Infrastructure.Extensions;
using KinoDev.ApiGateway.Infrastructure.HttpClients;
using KinoDev.ApiGateway.Infrastructure.Models.ConfigurationSettings;
using Microsoft.Extensions.Options;

namespace KinoDev.ApiGateway.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

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

            var authenticationSettings = builder.Configuration.GetSection("AuthenticationSettings").Get<AuthenticationSettings>();
            var apiClients = builder.Configuration.GetSection("ApiClients").Get<ApiClients>();
            if (authenticationSettings == null || apiClients == null)
            {
                throw new InvalidConfigurationException("Cannot obtain AuthenticationSettings from settings!");
            }

            builder.Services.Configure<AuthenticationSettings>(builder.Configuration.GetSection("AuthenticationSettings"));

            builder.Services.SetupAuthentication(authenticationSettings);

            builder.Services.InitializeInfrastructure();

            builder.Services.AddTransient<InternalAuthenticationDelegationHandler>();

            builder.Services
                .AddHttpClient<IAuthenticationClient, AuthenticationClient>(options =>
                {
                    options.BaseAddress = new Uri(apiClients.IdentityServiceUri);
                })
                .ConfigurePrimaryHttpMessageHandler(() => HttpClientHandlerFactory.CreateHandler(true));

            builder.Services
                .AddHttpClient<IDomainServiceClient, DomainServiceClient>(options =>
                {
                    options.BaseAddress = new Uri(apiClients.DomainServiceUri);
                })
                .ConfigurePrimaryHttpMessageHandler(() => HttpClientHandlerFactory.CreateHandler(true))
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

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
