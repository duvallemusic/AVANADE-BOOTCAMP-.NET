# Script para iniciar todos os serviços do E-Commerce Microservices
Write-Host "🚀 Iniciando E-Commerce Microservices..." -ForegroundColor Green

# Função para iniciar um serviço
function Start-Service {
    param(
        [string]$ServiceName,
        [string]$ProjectPath,
        [string]$Port
    )
    
    Write-Host "`n📦 Iniciando $ServiceName na porta $Port..." -ForegroundColor Yellow
    
    # Verificar se o projeto existe
    if (!(Test-Path $ProjectPath)) {
        Write-Host "❌ Projeto não encontrado: $ProjectPath" -ForegroundColor Red
        return $false
    }
    
    # Iniciar o serviço em uma nova janela do PowerShell
    $command = "cd '$ProjectPath'; dotnet run --urls 'http://localhost:$Port'"
    Start-Process powershell -ArgumentList "-NoExit", "-Command", $command
    
    Write-Host "✅ $ServiceName iniciado em nova janela" -ForegroundColor Green
    return $true
}

# Verificar se estamos no diretório correto
if (!(Test-Path "ECommerceMicroservices.sln")) {
    Write-Host "❌ Execute este script no diretório raiz do projeto (onde está o ECommerceMicroservices.sln)" -ForegroundColor Red
    exit 1
}

# Iniciar os serviços
$services = @(
    @{ Name = "ProductService"; Path = "src/ProductService"; Port = "7001" },
    @{ Name = "OrderService"; Path = "src/OrderService"; Port = "7002" },
    @{ Name = "ApiGateway"; Path = "src/ApiGateway"; Port = "7000" }
)

$successCount = 0
foreach ($service in $services) {
    if (Start-Service -ServiceName $service.Name -ProjectPath $service.Path -Port $service.Port) {
        $successCount++
    }
    Start-Sleep -Seconds 2
}

Write-Host "`n📋 Resumo:" -ForegroundColor Cyan
Write-Host "✅ $successCount de $($services.Count) serviços iniciados" -ForegroundColor Green

Write-Host "`n🌐 URLs dos serviços:" -ForegroundColor Yellow
Write-Host "   • ProductService: http://localhost:7001/swagger" -ForegroundColor White
Write-Host "   • OrderService: http://localhost:7002/swagger" -ForegroundColor White
Write-Host "   • ApiGateway: http://localhost:7000/swagger" -ForegroundColor White

Write-Host "`n⏳ Aguarde 10-15 segundos para que todos os serviços estejam prontos..." -ForegroundColor Yellow
Write-Host "💡 Use o script 'test-services.ps1' para verificar se estão funcionando" -ForegroundColor Cyan

Write-Host "`nPressione qualquer tecla para continuar..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
