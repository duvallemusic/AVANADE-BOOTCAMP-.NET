# Use a imagem oficial do .NET 9
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 7001

# Use a imagem do SDK para build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["src/ProductService/ProductService.csproj", "src/ProductService/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]

# Restaurar dependências
RUN dotnet restore "src/ProductService/ProductService.csproj"

# Copiar todo o código fonte
COPY . .

# Build da aplicação
WORKDIR "/src/src/ProductService"
RUN dotnet build "ProductService.csproj" -c Release -o /app/build

# Publish da aplicação
FROM build AS publish
RUN dotnet publish "ProductService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagem final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configurar variáveis de ambiente
ENV ASPNETCORE_URLS=http://+:7001
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "ProductService.dll"]