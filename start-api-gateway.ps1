# Script para iniciar apenas o ApiGateway
Write-Host "🚀 Iniciando ApiGateway..." -ForegroundColor Green

# Verificar se estamos no diretório correto
if (!(Test-Path "ECommerceMicroservices.sln")) {
    Write-Host "❌ Execute este script no diretório raiz do projeto" -ForegroundColor Red
    exit 1
}

# Navegar para o diretório do ApiGateway
Set-Location "src/ApiGateway"

Write-Host "📂 Diretório atual: $(Get-Location)" -ForegroundColor Cyan
Write-Host "🔧 Iniciando ApiGateway na porta 7000..." -ForegroundColor Yellow

# Iniciar o serviço
dotnet run --urls "http://localhost:7000"
