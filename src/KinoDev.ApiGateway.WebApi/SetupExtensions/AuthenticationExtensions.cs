using KinoDev.ApiGateway.WebApi.ConfigurationSettings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace KinoDev.ApiGateway.WebApi.SetupExtensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection SetupAuthentication(
            this IServiceCollection services,
            JwtSettings jwtSettings
            )
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // TODO: To settigns / variables
                    options.Authority = jwtSettings.Authority;
                    options.Audience = jwtSettings.Audience;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidAudience = jwtSettings.Audience,
                        ValidIssuer = jwtSettings.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                    };

                    // TODO: remove this when everything is OK
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            // Log the error message to see what exactly is failing
                            Console.WriteLine("Authentication failed in GATEWAY: " + context.Exception.Message + context.Exception.StackTrace);
                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }
    }
}
