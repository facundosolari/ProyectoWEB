using Application.Extensions;
using Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine($"==> Railway asignó el puerto: {Environment.GetEnvironmentVariable("PORT")}");
// ----------------------------
// Escuchar en el puerto dinámico de Railway (ANTES de Build)
var portStr = Environment.GetEnvironmentVariable("PORT");
if (!int.TryParse(portStr, out var port))
{
    throw new Exception("PORT environment variable is not set or invalid.");
}
Console.WriteLine($"==> Railway ahora asignó el puerto: {Environment.GetEnvironmentVariable("PORT")}");

builder.WebHost.ConfigureKestrel(options =>
{
    // Solo escucha en el puerto dinámico
    options.ListenAnyIP(port);
});

// Evitar que UseUrls sobrescriba algo por defecto
builder.WebHost.UseUrls(); // dejar vacío fuerza que no haya fallback a 8080

// ----------------------------
// SERVICIOS
builder.Services.AddControllers();

// CORS para frontend (cookies necesitan AllowCredentials)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins(
                "http://localhost:5173", // desarrollo
                "https://cobacha-60edd35ba-facundosolaris-projects.vercel.app", // prod Vercel
                "https://proyectoweb.up.railway.app" // prod Railway
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials() // muy importante para cookies HttpOnly
    );
});

// ----------------------------
// Infraestructura y servicios
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddHttpContextAccessor();

// ----------------------------
// JWT Authentication
// Ahora se asegura de leer correctamente las variables de entorno
builder.Services.AddJwtAuthentication(builder.Configuration);

// ----------------------------
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----------------------------
// BUILD APP
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var app = builder.Build();
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

// ----------------------------
// PIPELINE DE MIDDLEWARE
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontend");

app.UseStaticFiles();

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// ----------------------------
// Mapear controladores

app.MapControllers();

// ----------------------------
// Ejecutar app
app.Run();