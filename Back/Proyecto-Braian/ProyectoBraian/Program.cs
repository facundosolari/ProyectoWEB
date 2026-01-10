using Application.Extensions;
using Infrastructure.Context;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURACIÓN DEL PUERTO (Crucial para Railway)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var apiUrl = Environment.GetEnvironmentVariable("API_URL");
Console.WriteLine($"La API que se pasó al contenedor es: {apiUrl}");

try
{
    // ----------------------------
    // BLOQUE DE SERVICIOS
    // ----------------------------
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend",
            policy => policy
                .WithOrigins(
                    "http://localhost:5173",
                    "https://cobacha-60edd35ba-facundosolaris-projects.vercel.app",
                    "https://proyectoweb.up.railway.app"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
        );
    });

    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddApplicationServices();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddJwtAuthentication(builder.Configuration);

    var app = builder.Build();

    // ----------------------------
    // MIGRACIONES AUTOMÁTICAS
    // ----------------------------
    // Nota: Si esto falla, la app no debería morir, por eso el try-catch interno
    /*
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
            db.Database.Migrate();
            Console.WriteLine("✅ Migraciones aplicadas correctamente");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("⚠️ Error no fatal al aplicar migraciones (revisa la conexión):");
        Console.WriteLine(ex.Message);
    }
    */

    // ----------------------------
    // BLOQUE DE MIDDLEWARE
    // ----------------------------
    if (app.Environment.IsDevelopment() || true) // Forzamos Swagger para ver si funciona en Railway
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Proyecto API v1");
            c.RoutePrefix = "api-docs"; // Accederás vía proyectoweb.up.railway.app/api-docs
        });
    }

    app.UseCors("AllowFrontend");
    app.UseStaticFiles();

    app.UseAuthentication();
    app.UseAuthorization();

    // ----------------------------
    // RUTAS
    // ----------------------------
    app.MapGet("/health", (ILogger<Program> log) =>
    {
        log.LogInformation("Healthcheck requested at {time}", DateTime.UtcNow);
        return Results.Ok(new { Status = "Healthy", Port = port });
    });

    app.MapControllers();

    Console.WriteLine($"🚀 App iniciando en el puerto: {port}...");
    app.Run();
}
catch (Exception ex)
{
    // Esto captura errores de compilación del builder o crash al arrancar
    Console.WriteLine("💀 ERROR CRÍTICO EN ARRANQUE:");
    Console.WriteLine(ex.Message);
    if (ex.InnerException != null)
        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");

    Console.WriteLine(ex.StackTrace);
    throw; // Re-lanzar para que Railway sepa que el deploy falló
}