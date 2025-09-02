# Script para iniciar apenas o ProductService
Write-Host "🚀 Iniciando ProductService..." -ForegroundColor Green

# Verificar se estamos no diretório correto
if (!(Test-Path "ECommerceMicroservices.sln")) {
    Write-Host "❌ Execute este script no diretório raiz do projeto" -ForegroundColor Red
    exit 1
}

# Navegar para o diretório do ProductService
Set-Location "src/ProductService"

Write-Host "📂 Diretório atual: $(Get-Location)" -ForegroundColor Cyan
Write-Host "🔧 Iniciando ProductService na porta 7001..." -ForegroundColor Yellow

# Iniciar o serviço
dotnet run --urls "http://localhost:7001"
