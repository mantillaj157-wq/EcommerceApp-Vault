# Usa la imagen SDK de .NET 10 para compilar
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copiar el archivo del proyecto y restaurar paquetes
COPY ["EcommerceApp.csproj", "./"]
RUN dotnet restore "EcommerceApp.csproj"

# Copiar todo el código fuente y publicar en modo Release
COPY . .
RUN dotnet publish "EcommerceApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Usa la imagen Runtime de .NET 10 para ejecutar la app
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
COPY --from=build /app/publish .

# Variables de entorno para estabilidad de la vista previa de .NET
ENV DOTNET_EnableDiagnostics=0
ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80
ENTRYPOINT ["dotnet", "EcommerceApp.dll"]