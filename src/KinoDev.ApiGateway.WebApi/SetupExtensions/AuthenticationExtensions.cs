using KinoDev.ApiGateway.Infrastructure.Models.ConfigurationSettings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace KinoDev.ApiGateway.WebApi.SetupExtensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection SetupAuthentication(
            this IServiceCollection services,
            AuthenticationSettings authenticationSettings
            )
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = authenticationSettings.Issuer,
                        ValidAudience = authenticationSettings.Audiences.Gateway,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.Secret)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            return services;
        }
    }
}
