# Script para testar os microserviços Docker
Write-Host "🧪 Testando E-Commerce Microservices Docker..." -ForegroundColor Green

# Verificar se estamos no diretório correto
if (!(Test-Path "docker-compose.yml")) {
    Write-Host "❌ Execute este script no diretório raiz do projeto" -ForegroundColor Red
    exit 1
}

# Verificar se os containers estão rodando
$containers = docker-compose ps -q
if (!$containers) {
    Write-Host "❌ Nenhum container está rodando. Execute .\docker-start.ps1 primeiro." -ForegroundColor Red
    exit 1
}

# Função para testar uma URL
function Test-Service {
    param([string]$Url, [string]$ServiceName)
    
    Write-Host "🔍 Testando $ServiceName..." -ForegroundColor Cyan
    
    try {
        $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 10
        if ($response.StatusCode -eq 200) {
            Write-Host "✅ $ServiceName está funcionando em $Url" -ForegroundColor Green
            return $true
        } else {
            Write-Host "❌ $ServiceName retornou status $($response.StatusCode)" -ForegroundColor Red
            return $false
        }
    }
    catch {
        Write-Host "❌ $ServiceName não está acessível em $Url" -ForegroundColor Red
        Write-Host "   Erro: $($_.Exception.Message)" -ForegroundColor Gray
        return $false
    }
}

# Testar os serviços
Write-Host "`n🔍 Testando serviços..." -ForegroundColor Yellow

$productServiceOk = Test-Service "http://localhost:7001/swagger" "ProductService"
$orderServiceOk = Test-Service "http://localhost:7002/swagger" "OrderService"
$apiGatewayOk = Test-Service "http://localhost:7000/swagger" "ApiGateway"
$rabbitmqOk = Test-Service "http://localhost:15672" "RabbitMQ Management"

Write-Host "`n📊 Resumo dos testes:" -ForegroundColor Cyan
Write-Host "ProductService: $(if($productServiceOk) {'✅ OK'} else {'❌ FALHOU'})" -ForegroundColor $(if($productServiceOk) {'Green'} else {'Red'})
Write-Host "OrderService: $(if($orderServiceOk) {'✅ OK'} else {'❌ FALHOU'})" -ForegroundColor $(if($orderServiceOk) {'Green'} else {'Red'})
Write-Host "ApiGateway: $(if($apiGatewayOk) {'✅ OK'} else {'❌ FALHOU'})" -ForegroundColor $(if($apiGatewayOk) {'Green'} else {'Red'})
Write-Host "RabbitMQ: $(if($rabbitmqOk) {'✅ OK'} else {'❌ FALHOU'})" -ForegroundColor $(if($rabbitmqOk) {'Green'} else {'Red'})

if ($productServiceOk -and $orderServiceOk -and $apiGatewayOk -and $rabbitmqOk) {
    Write-Host "`n🎉 Todos os serviços estão funcionando!" -ForegroundColor Green
    Write-Host "`n🌐 URLs para testar:" -ForegroundColor Yellow
    Write-Host "   • ProductService: http://localhost:7001/swagger" -ForegroundColor White
    Write-Host "   • OrderService: http://localhost:7002/swagger" -ForegroundColor White
    Write-Host "   • ApiGateway: http://localhost:7000/swagger" -ForegroundColor White
    Write-Host "   • RabbitMQ Management: http://localhost:15672 (guest/guest)" -ForegroundColor White
} else {
    Write-Host "`n⚠️ Alguns serviços não estão funcionando." -ForegroundColor Yellow
    Write-Host "💡 Para verificar logs, use: .\docker-logs.ps1" -ForegroundColor Cyan
    Write-Host "💡 Para reiniciar, use: .\docker-stop.ps1 e depois .\docker-start.ps1" -ForegroundColor Cyan
}

# Verificar status dos containers
Write-Host "`n📋 Status dos containers:" -ForegroundColor Cyan
docker-compose ps
