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
                var connectionString = configuration.GetConnectionString("DefaultConnection");

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