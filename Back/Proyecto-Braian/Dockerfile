# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Proyecto-Braian.sln ./

COPY Domain/Domain.csproj Domain/
COPY Application/Application.csproj Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY ProyectoBraian/ProyectoBraian.csproj ProyectoBraian/

RUN dotnet restore ProyectoBraian/ProyectoBraian.csproj

COPY . .

RUN dotnet publish ProyectoBraian/ProyectoBraian.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# =========================
# Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ProyectoBraian.dll"]