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
                // 1. Intentamos obtener la cadena base del appsettings.json
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                // 2. Tomamos las variables de entorno (útil para Railway/Producción)
                var host = Environment.GetEnvironmentVariable("MYSQLHOST");
                var port = Environment.GetEnvironmentVariable("MYSQLPORT") ?? "3306";
                var user = Environment.GetEnvironmentVariable("MYSQLUSER");
                var pass = Environment.GetEnvironmentVariable("MYSQLPASSWORD");
                var db = Environment.GetEnvironmentVariable("MYSQLDATABASE");

                // 3. Si las variables de entorno existen, construimos la cadena con ellas
                if (!string.IsNullOrWhiteSpace(host) && !string.IsNullOrWhiteSpace(user))
                {
                    connectionString = $"Server={host};Port={port};Database={db};User={user};Password={pass};Connect Timeout=120;";
                    Console.WriteLine($"✅ Usando variables de entorno - DB Connection: Server={host};Port={port};Database={db};User={user};Password=*****");
                }
                else
                {
                    Console.WriteLine("ℹ️ Usando ConnectionString de appsettings.json");
                }

                // 4. Verificación de seguridad
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new Exception("❌ Error: No se encontró una cadena de conexión válida.");
                }

                // 5. Configuración de versión manual
                var serverVersion = new MySqlServerVersion(new Version(8, 0, 30));

                options.UseMySql(connectionString, serverVersion, b =>
                {
                    b.MigrationsAssembly("Infrastructure");

                    // --- NUEVOS CAMBIOS PARA RESILIENCIA ---
                    b.EnableRetryOnFailure(
                        maxRetryCount: 5,               // Reintenta hasta 5 veces
                        maxRetryDelay: TimeSpan.FromSeconds(10), // Espera entre reintentos
                        errorNumbersToAdd: null
                    );
                });
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