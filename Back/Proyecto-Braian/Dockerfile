# =========================
# Etapa 1: Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar solo la solución y los proyectos para un restore rápido
COPY *.sln ./
COPY Domain/*.csproj ./Domain/
COPY Application/*.csproj ./Application/
COPY Infrastructure/*.csproj ./Infrastructure/
COPY ProyectoBraian/*.csproj ./ProyectoBraian/

# Restaurar dependencias
RUN dotnet restore "ProyectoBraian/ProyectoBraian.csproj"

# Copiar todo el código fuente
COPY . .

# Compilar la app en modo Release
RUN dotnet publish "ProyectoBraian/ProyectoBraian.csproj" -c Release -o /app/publish

# =========================
# Etapa 2: Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copiar la app compilada desde la etapa de build
COPY --from=build /app/publish .

# Variables de entorno para MySQL (Railway)
ENV MYSQL_DATABASE=railway
ENV MYSQL_USER=root
ENV MYSQL_PASSWORD=PwOjFMfsKmhsaGDCPupdgcyEknLWFhnB
ENV MYSQL_HOST=mysql.railway.internal
ENV MYSQL_PORT=3306

# Puerto que usa la app
EXPOSE 5000

# Comando de arranque
ENTRYPOINT ["dotnet", "ProyectoBraian.dll"]