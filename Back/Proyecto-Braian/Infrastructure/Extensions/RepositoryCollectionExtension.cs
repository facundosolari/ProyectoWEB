using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure.Context;
using Infrastructure.Data;
using Infrastructure.ThirdServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataBaseContext>(options =>
            {
                try
                {
                    // Intenta tomar la connection string del appsettings
                    

                    // Tomamos las variables de entorno
                    var host = Environment.GetEnvironmentVariable("MYSQL_HOST");
                    var port = Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306";
                    var user = Environment.GetEnvironmentVariable("MYSQL_USER");
                    var pass = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
                    var db = Environment.GetEnvironmentVariable("MYSQL_DATABASE");
                    var connectionString = configuration.GetConnectionString("DefaultConnection")
                        .Replace("${MYSQL_HOST}", host)
                        .Replace("${MYSQL_PORT}", port)
                        .Replace("${MYSQL_DATABASE}", db)
                        .Replace("${MYSQL_USER}", user)
                        .Replace("${MYSQL_PASSWORD}", pass);

                    // Si la connection string del appsettings es vacía, construimos desde variables de entorno
                    if (string.IsNullOrWhiteSpace(connectionString))
                    {
                        if (!string.IsNullOrWhiteSpace(host))
                        {
                            connectionString = $"Server={host};Port={port};Database={db};User={user};Password={pass};Connect Timeout=90;";
                        }
                    }
    
                    // Si sigue vacía, logueamos y lanzamos excepción
                    if (string.IsNullOrWhiteSpace(connectionString))
                    {
                        Console.WriteLine("❌ Connection string no encontrada, revisa tus variables de entorno.");
                        throw new Exception("Connection string no encontrada");
                    }

                    // Logueamos info de la DB (sin la contraseña)
                    Console.WriteLine($"✅ DB Connection: Server={host};Port={port};Database={db};User={user};Password=*****");

                    // Configuramos DbContext con AutoDetect para mayor flexibilidad
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ Error al configurar DbContext:");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            });

            // ----------------------------
            // Repositorios
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IGoogleTokenValidator, GoogleTokenValidator>();
            services.AddScoped<IOrderMessageRepository, OrderMessageRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductSizeRepository, ProductSizeRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IDescuentoRepository, DescuentoRepository>();
            services.AddScoped<IReglaDescuentoRepository, ReglaDescuentoRepository>();

            return services;
        }
    }
}