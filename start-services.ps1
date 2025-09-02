# Script simplificado para iniciar os serviços
Write-Host "Iniciando E-Commerce Microservices..." -ForegroundColor Green

# Iniciar ProductService
Write-Host "Iniciando ProductService na porta 7001..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'src/ProductService'; dotnet run --urls 'http://localhost:7001'"

# Aguardar um pouco
Start-Sleep -Seconds 3

# Iniciar OrderService
Write-Host "Iniciando OrderService na porta 7002..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'src/OrderService'; dotnet run --urls 'http://localhost:7002'"

# Aguardar um pouco
Start-Sleep -Seconds 3

# Iniciar ApiGateway
Write-Host "Iniciando ApiGateway na porta 7000..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'src/ApiGateway'; dotnet run --urls 'http://localhost:7000'"

Write-Host "`nServicos iniciados! Aguarde alguns segundos para que todos estejam prontos." -ForegroundColor Green
Write-Host "`nURLs dos servicos:" -ForegroundColor Cyan
Write-Host "  - ProductService: http://localhost:7001/swagger" -ForegroundColor White
Write-Host "  - OrderService: http://localhost:7002/swagger" -ForegroundColor White
Write-Host "  - ApiGateway: http://localhost:7000/swagger" -ForegroundColor White

Write-Host "`nPressione qualquer tecla para continuar..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
