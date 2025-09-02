# Use a imagem oficial do .NET 9
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 7002

# Use a imagem do SDK para build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["src/OrderService/OrderService.csproj", "src/OrderService/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]

# Restaurar dependências
RUN dotnet restore "src/OrderService/OrderService.csproj"

# Copiar todo o código fonte
COPY . .

# Build da aplicação
WORKDIR "/src/src/OrderService"
RUN dotnet build "OrderService.csproj" -c Release -o /app/build

# Publish da aplicação
FROM build AS publish
RUN dotnet publish "OrderService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagem final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configurar variáveis de ambiente
ENV ASPNETCORE_URLS=http://+:7002
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "OrderService.dll"]