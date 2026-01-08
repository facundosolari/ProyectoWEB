# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Proyecto-Braian.sln ./
COPY Domain/Domain.csproj Domain/
COPY Application/Application.csproj Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY ProyectoBraian/ProyectoBraian.csproj ProyectoBraian/

RUN dotnet restore ProyectoBraian/ProyectoBraian.csproj

COPY . .

RUN dotnet publish ProyectoBraian/ProyectoBraian.csproj -c Release -o /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

# NO fijes ASPNETCORE_URLS ni el puerto
ENTRYPOINT ["dotnet", "ProyectoBraian.dll"]