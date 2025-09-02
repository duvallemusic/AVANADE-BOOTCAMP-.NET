# Script para visualizar logs dos microserviços Docker
Write-Host "📋 Logs dos E-Commerce Microservices..." -ForegroundColor Green

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

Write-Host "`n📊 Containers em execução:" -ForegroundColor Cyan
docker-compose ps

Write-Host "`n🔍 Escolha uma opção:" -ForegroundColor Yellow
Write-Host "1. Ver logs de todos os serviços" -ForegroundColor White
Write-Host "2. Ver logs do ProductService" -ForegroundColor White
Write-Host "3. Ver logs do OrderService" -ForegroundColor White
Write-Host "4. Ver logs do ApiGateway" -ForegroundColor White
Write-Host "5. Ver logs do RabbitMQ" -ForegroundColor White
Write-Host "0. Sair" -ForegroundColor White

$choice = Read-Host "`nDigite sua escolha (0-5)"

switch ($choice) {
    "1" {
        Write-Host "`n📋 Logs de todos os serviços:" -ForegroundColor Cyan
        docker-compose logs -f
    }
    "2" {
        Write-Host "`n📋 Logs do ProductService:" -ForegroundColor Cyan
        docker-compose logs -f productservice
    }
    "3" {
        Write-Host "`n📋 Logs do OrderService:" -ForegroundColor Cyan
        docker-compose logs -f orderservice
    }
    "4" {
        Write-Host "`n📋 Logs do ApiGateway:" -ForegroundColor Cyan
        docker-compose logs -f apigateway
    }
    "5" {
        Write-Host "`n📋 Logs do RabbitMQ:" -ForegroundColor Cyan
        docker-compose logs -f rabbitmq
    }
    "0" {
        Write-Host "👋 Saindo..." -ForegroundColor Yellow
        exit 0
    }
    default {
        Write-Host "❌ Opção inválida!" -ForegroundColor Red
    }
}
