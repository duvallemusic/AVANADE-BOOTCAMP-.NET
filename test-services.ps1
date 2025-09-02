# Script para testar os servicos
Write-Host "Testando E-Commerce Microservices..." -ForegroundColor Green

# Funcao para testar uma URL
function Test-Service {
    param([string]$Url, [string]$ServiceName)
    
    Write-Host "Testando $ServiceName..." -ForegroundColor Cyan
    
    try {
        $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 10
        if ($response.StatusCode -eq 200) {
            Write-Host "$ServiceName esta funcionando em $Url" -ForegroundColor Green
            return $true
        } else {
            Write-Host "$ServiceName retornou status $($response.StatusCode)" -ForegroundColor Red
            return $false
        }
    }
    catch {
        Write-Host "$ServiceName nao esta acessivel em $Url" -ForegroundColor Red
        Write-Host "   Erro: $($_.Exception.Message)" -ForegroundColor Gray
        return $false
    }
}

# Testar os servicos
Write-Host "`nTestando servicos..." -ForegroundColor Yellow

$productServiceOk = Test-Service "http://localhost:7001/swagger" "ProductService"
$orderServiceOk = Test-Service "http://localhost:7002/swagger" "OrderService"
$apiGatewayOk = Test-Service "http://localhost:7000/swagger" "ApiGateway"

Write-Host "`nResumo dos testes:" -ForegroundColor Cyan
Write-Host "ProductService: $(if($productServiceOk) {'OK'} else {'FALHOU'})" -ForegroundColor $(if($productServiceOk) {'Green'} else {'Red'})
Write-Host "OrderService: $(if($orderServiceOk) {'OK'} else {'FALHOU'})" -ForegroundColor $(if($orderServiceOk) {'Green'} else {'Red'})
Write-Host "ApiGateway: $(if($apiGatewayOk) {'OK'} else {'FALHOU'})" -ForegroundColor $(if($apiGatewayOk) {'Green'} else {'Red'})

if ($productServiceOk -and $orderServiceOk -and $apiGatewayOk) {
    Write-Host "`nTodos os servicos estao funcionando!" -ForegroundColor Green
    Write-Host "`nURLs para testar:" -ForegroundColor Yellow
    Write-Host "   - ProductService: http://localhost:7001/swagger" -ForegroundColor White
    Write-Host "   - OrderService: http://localhost:7002/swagger" -ForegroundColor White
    Write-Host "   - ApiGateway: http://localhost:7000/swagger" -ForegroundColor White
} else {
    Write-Host "`nAlguns servicos nao estao funcionando." -ForegroundColor Yellow
    Write-Host "Para iniciar os servicos, use:" -ForegroundColor Cyan
    Write-Host "   - .\start-all-services.ps1 (todos os servicos)" -ForegroundColor White
    Write-Host "   - .\start-product-service.ps1 (apenas ProductService)" -ForegroundColor White
    Write-Host "   - .\start-order-service.ps1 (apenas OrderService)" -ForegroundColor White
    Write-Host "   - .\start-api-gateway.ps1 (apenas ApiGateway)" -ForegroundColor White
}