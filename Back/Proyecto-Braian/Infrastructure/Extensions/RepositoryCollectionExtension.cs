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
                var connectionString =
                    configuration.GetConnectionString("DefaultConnection");

                // Railway MySQL
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    var host = Environment.GetEnvironmentVariable("MYSQLHOST");
                    var db = Environment.GetEnvironmentVariable("MYSQLDATABASE");
                    var user = Environment.GetEnvironmentVariable("MYSQLUSER");
                    var pass = Environment.GetEnvironmentVariable("MYSQLPASSWORD");
                    var port = Environment.GetEnvironmentVariable("MYSQLPORT") ?? "3306";

                    if (!string.IsNullOrWhiteSpace(host))
                    {
                        connectionString =
                            $"Server={host};Port={port};Database={db};User={user};Password={pass};";
                    }
                }

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new Exception("Connection string no encontrada");

                options.UseMySql(
                    connectionString,
                    new MySqlServerVersion(new Version(8, 0, 36))
                );
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