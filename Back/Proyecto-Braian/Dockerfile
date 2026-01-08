# =========================
# Etapa 1: Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar solo los proyectos y la solución para un restore más rápido
COPY *.sln ./
COPY Domain/*.csproj ./Domain/
COPY Application/*.csproj ./Application/
COPY Infrastructure/*.csproj ./Infrastructure/
COPY Web/*.csproj ./Proyecto\ Braian/

# Restaurar dependencias
RUN dotnet restore "Proyecto Braian/Proyecto Braian.csproj"

# Copiar todo el código fuente
COPY . .

# Compilar la app en modo Release
RUN dotnet publish "Proyecto Braian/Proyecto Braian.csproj" -c Release -o /app/publish

# =========================
# Etapa 2: Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copiar la app publicada desde la etapa de build
COPY --from=build /app/publish .

# Variables de entorno para MySQL
ENV MYSQL_DATABASE=railway
ENV MYSQL_USER=root
ENV MYSQL_PASSWORD=PwOjFMfsKmhsaGDCPupdgcyEknLWFhnB
ENV MYSQL_HOST=mysql.railway.internal
ENV MYSQL_PORT=3306

# Puerto expuesto (el que usa tu app)
EXPOSE 5000

# Comando de arranque
ENTRYPOINT ["dotnet", "Proyecto Braian.dll"]