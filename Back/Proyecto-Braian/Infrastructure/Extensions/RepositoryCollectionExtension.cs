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
                // 1. Intentamos obtener las variables de entorno (Prioridad para Railway)
                // Usamos el operador ?? para buscar nombres con y sin guion bajo
                var host = Environment.GetEnvironmentVariable("MYSQLHOST") ?? Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "mysql";
                var port = Environment.GetEnvironmentVariable("MYSQLPORT") ?? Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306";
                var user = Environment.GetEnvironmentVariable("MYSQLUSER") ?? Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root";
                var pass = Environment.GetEnvironmentVariable("MYSQLPASSWORD") ?? Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
                var db = Environment.GetEnvironmentVariable("MYSQLDATABASE") ?? Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "railway";

                string connectionString;

                // 2. Si existe password (estamos en la nube o configurado), construimos la cadena
                if (!string.IsNullOrWhiteSpace(pass))
                {
                    connectionString = $"Server={host};Port={port};Database={db};User={user};Password={pass};Connect Timeout=120;";

                    // LOG CRÍTICO: Esto nos dirá en Railway qué valores está leyendo realmente
                    Console.WriteLine($"🚀 INTENTO CONEXIÓN: Host={host} | Port={port} | DB={db} | User={user}");
                }
                else
                {
                    // 3. Fallback al appsettings.json si no hay variables de entorno (Local)
                    connectionString = configuration.GetConnectionString("DefaultConnection");
                    Console.WriteLine("ℹ️ MODO LOCAL: Usando ConnectionString de appsettings.json");
                }

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new Exception("❌ Error: No se encontró una cadena de conexión válida.");
                }

                var serverVersion = ServerVersion.AutoDetect(connectionString);

                options.UseMySql(connectionString, serverVersion, b =>
                {
                    b.MigrationsAssembly("Infrastructure");
                    b.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null
                    );
                });
            });

            // ----------------------------
            // Repositorios
            // ----------------------------
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