using Domain.Interfaces;
using Infrastructure.Context;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Application.Interfaces;
using Infrastructure.ThirdServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Implementacion DbContext a la interfaz
            services.AddDbContext<DataBaseContext>(options =>
            {
                var host = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost";
                var db = Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "proyecto_braian";
                var user = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root";
                var password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "solariDev135!";
                var port = Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306";

                Console.WriteLine($"MYSQL_HOST={host}");
                Console.WriteLine($"MYSQL_DATABASE={db}");
                Console.WriteLine($"MYSQL_USER={user}");
                Console.WriteLine($"MYSQL_PORT={port}");

                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(db) ||
                    string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(port))
                {
                    throw new Exception("One or more MySQL environment variables are not set!");
                }

                var connectionString = $"Server={host};Database={db};User={user};Password={password};Port={port};";

                if (connectionString.Contains("Data Source=", StringComparison.OrdinalIgnoreCase))
                {
                    // SQLite
                    options.UseSqlite(connectionString);
                }
                else
                {
                    // MySQL
                    options.UseMySql(
                        connectionString,
                        ServerVersion.AutoDetect(connectionString)
                    );
                }
            });
            // Implementacion Repositorios a la interfaz

            services.AddScoped<IAuthenticationService,AuthenticationService>();
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