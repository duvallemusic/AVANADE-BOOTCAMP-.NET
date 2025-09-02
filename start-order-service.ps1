# Script para iniciar apenas o OrderService
Write-Host "🚀 Iniciando OrderService..." -ForegroundColor Green

# Verificar se estamos no diretório correto
if (!(Test-Path "ECommerceMicroservices.sln")) {
    Write-Host "❌ Execute este script no diretório raiz do projeto" -ForegroundColor Red
    exit 1
}

# Navegar para o diretório do OrderService
Set-Location "src/OrderService"

Write-Host "📂 Diretório atual: $(Get-Location)" -ForegroundColor Cyan
Write-Host "🔧 Iniciando OrderService na porta 7002..." -ForegroundColor Yellow

# Iniciar o serviço
dotnet run --urls "http://localhost:7002"
