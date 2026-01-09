using Application.Extensions;
using Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------
// Puerto dinámico de Railway o fallback local
int port;
var portStr = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(portStr) && int.TryParse(portStr, out var parsedPort))
{
    port = parsedPort;
}
else
{
    port = 5000; // Puerto local de fallback
    Console.WriteLine("No PORT env variable found. Using local port 5000.");
}

// Configurar Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(port);
});

// ----------------------------
// Servicios
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

// ----------------------------
// Infrastructure Services (usa tu extensión tal cual)
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddApplicationServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddJwtAuthentication(builder.Configuration);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ----------------------------
// Middleware
var app = builder.Build();

// Swagger solo para desarrollo
if (port == 5000)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Proyecto API v1");
        c.RoutePrefix = "api-docs"; // URL: /api-docs
    });
}

// CORS
app.UseCors("AllowFrontend");

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

// ----------------------------
// Endpoints básicos
app.MapGet("/health", (ILogger<Program> log) =>
{
    log.LogInformation("Healthcheck requested at {time}", DateTime.UtcNow);
    return Results.Ok("Healthy");
});

app.MapGet("/", (ILogger<Program> log) =>
{
    log.LogInformation("Root endpoint requested at {time}", DateTime.UtcNow);
    return Results.Ok("API is running");
});

// Mapear controladores
app.MapControllers();

// ----------------------------
// Abrir Swagger automáticamente solo en local
/*
if (port == 5000)
{
    var url = $"http://localhost:{port}/api-docs";
    try
    {
        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        };
        System.Diagnostics.Process.Start(psi);
    }
    catch (Exception ex)
    {
        Console.WriteLine("No se pudo abrir el navegador automáticamente: " + ex.Message);
    }
}
*/

// ----------------------------
// Ejecutar
app.Run();