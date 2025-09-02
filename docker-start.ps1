# Script para iniciar os microserviços com Docker
Write-Host "🐳 Iniciando E-Commerce Microservices com Docker..." -ForegroundColor Green

# Verificar se o Docker está rodando
try {
    docker version | Out-Null
    Write-Host "✅ Docker está rodando" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker não está rodando. Por favor, inicie o Docker Desktop primeiro." -ForegroundColor Red
    exit 1
}

# Verificar se estamos no diretório correto
if (!(Test-Path "docker-compose.yml")) {
    Write-Host "❌ Execute este script no diretório raiz do projeto (onde está o docker-compose.yml)" -ForegroundColor Red
    exit 1
}

Write-Host "`n🔧 Construindo e iniciando os serviços..." -ForegroundColor Yellow

# Parar containers existentes
Write-Host "🛑 Parando containers existentes..." -ForegroundColor Cyan
docker-compose down

# Construir e iniciar os serviços
Write-Host "🏗️ Construindo imagens..." -ForegroundColor Cyan
docker-compose build

Write-Host "🚀 Iniciando serviços..." -ForegroundColor Cyan
docker-compose up -d

# Aguardar os serviços iniciarem
Write-Host "`n⏳ Aguardando serviços iniciarem..." -ForegroundColor Yellow
Start-Sleep -Seconds 15

# Verificar status dos serviços
Write-Host "`n📊 Status dos serviços:" -ForegroundColor Cyan
docker-compose ps

Write-Host "`n🌐 URLs dos serviços:" -ForegroundColor Yellow
Write-Host "   • ProductService: http://localhost:7001/swagger" -ForegroundColor White
Write-Host "   • OrderService: http://localhost:7002/swagger" -ForegroundColor White
Write-Host "   • ApiGateway: http://localhost:7000/swagger" -ForegroundColor White
Write-Host "   • RabbitMQ Management: http://localhost:15672 (guest/guest)" -ForegroundColor White

Write-Host "`n💡 Comandos úteis:" -ForegroundColor Cyan
Write-Host "   • Ver logs: docker-compose logs -f [servico]" -ForegroundColor White
Write-Host "   • Parar serviços: docker-compose down" -ForegroundColor White
Write-Host "   • Reiniciar serviço: docker-compose restart [servico]" -ForegroundColor White
Write-Host "   • Ver status: docker-compose ps" -ForegroundColor White

Write-Host "`nPressione qualquer tecla para continuar..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
