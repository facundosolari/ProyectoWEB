using Application.Extensions;
using Infrastructure.Context;
using Infrastructure.Extensions;

var apiUrl = Environment.GetEnvironmentVariable("API_URL");

// Mostrarla en consola
Console.WriteLine($"La API que se pasó al contenedor es: {apiUrl}");
try
{
    var builder = WebApplication.CreateBuilder(args);

    // ----------------------------
    // BLOQUE DE SERVICIOS
    try
    {
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

        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error en configuración de servicios:");
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.StackTrace);
    }

    var app = builder.Build();

    // ----------------------------
    // MIGRACIONES AUTOMÁTICAS
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
        Console.WriteLine("❌ Error al aplicar migraciones:");
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.StackTrace);
    }

    // ----------------------------
    // BLOQUE DE MIDDLEWARE
    try
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Proyecto API v1");
            c.RoutePrefix = "api-docs";
        });

        app.UseCors("AllowFrontend");
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error en configuración de middleware:");
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.StackTrace);
    }

    // ----------------------------
    // RUTAS
    app.MapGet("/health", (ILogger<Program> log) =>
    {
        log.LogInformation("Healthcheck requested at {time}", DateTime.UtcNow);
        return Results.Ok("Healthy");
    });

    app.MapControllers();

    // ----------------------------
    try
    {
        app.Run();
        Console.WriteLine("✅ App lista, esperando requests...");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error al iniciar la app:");
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.StackTrace);
    }
}
catch (Exception ex)
{
    Console.WriteLine("💀 ERROR CRÍTICO EN ARRANQUE:");
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);
}