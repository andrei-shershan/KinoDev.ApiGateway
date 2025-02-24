using KinoDev.ApiGateway.WebApi.ConfigurationSettings;
using KinoDev.ApiGateway.WebApi.SetupExtensions;

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

            var jwtSettings = builder.Configuration.GetSection("JWT").Get<JwtSettings>();
            builder.Services.SetupAuthentication(jwtSettings);

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
