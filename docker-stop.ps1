# Script para parar os microserviços Docker
Write-Host "🛑 Parando E-Commerce Microservices..." -ForegroundColor Red

# Verificar se estamos no diretório correto
if (!(Test-Path "docker-compose.yml")) {
    Write-Host "❌ Execute este script no diretório raiz do projeto" -ForegroundColor Red
    exit 1
}

# Parar e remover containers
Write-Host "🛑 Parando containers..." -ForegroundColor Yellow
docker-compose down

# Remover volumes (opcional - descomente se quiser limpar dados)
# Write-Host "🗑️ Removendo volumes..." -ForegroundColor Yellow
# docker-compose down -v

# Remover imagens (opcional - descomente se quiser limpar imagens)
# Write-Host "🗑️ Removendo imagens..." -ForegroundColor Yellow
# docker-compose down --rmi all

Write-Host "✅ Serviços parados com sucesso!" -ForegroundColor Green

Write-Host "`n💡 Comandos úteis:" -ForegroundColor Cyan
Write-Host "   • Iniciar novamente: .\docker-start.ps1" -ForegroundColor White
Write-Host "   • Ver containers: docker ps -a" -ForegroundColor White
Write-Host "   • Limpar tudo: docker system prune -a" -ForegroundColor White
