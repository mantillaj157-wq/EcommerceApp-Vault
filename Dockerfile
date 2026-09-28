# Imagen base para la ejecución en .NET 10
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Imagen SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar el archivo de proyecto y restaurar dependencias
COPY ["Ecommerce_Vault.csproj", "./"]
RUN dotnet restore "Ecommerce_Vault.csproj"

# Copiar todos los archivos del proyecto y compilar
COPY . .
RUN dotnet build "Ecommerce_Vault.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ecommerce_Vault.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecommerce_Vault.dll"]