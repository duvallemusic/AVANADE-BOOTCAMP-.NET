# Script para executar todos os servicos do E-Commerce Microservices
# Execute este script no PowerShell

Write-Host "Iniciando E-Commerce Microservices..." -ForegroundColor Green

# Verificar se o .NET 9 esta instalado
try {
    $dotnetVersion = dotnet --version
    Write-Host ".NET versao: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host ".NET 9 nao encontrado. Instale o .NET 9 SDK primeiro." -ForegroundColor Red
    exit 1
}

# Restaurar dependencias
Write-Host "Restaurando dependencias..." -ForegroundColor Yellow
dotnet restore ECommerceMicroservices.sln

# Executar testes
Write-Host "Executando testes..." -ForegroundColor Yellow
dotnet test ECommerceMicroservices.sln

if ($LASTEXITCODE -ne 0) {
    Write-Host "Testes falharam. Verifique os erros acima." -ForegroundColor Red
    exit 1
}

Write-Host "Testes passaram!" -ForegroundColor Green

# Criar diretorios de logs se nao existirem
if (!(Test-Path "logs")) {
    New-Item -ItemType Directory -Path "logs"
}

# Funcao para executar um servico em background
function Start-Service {
    param(
        [string]$ServiceName,
        [string]$ProjectPath,
        [int]$Port
    )
    
    Write-Host "Iniciando $ServiceName na porta $Port..." -ForegroundColor Cyan
    
    $process = Start-Process -FilePath "dotnet" -ArgumentList "run", "--project", $ProjectPath -PassThru -WindowStyle Hidden
    
    # Aguardar um pouco para o servico inicializar
    Start-Sleep -Seconds 3
    
    if ($process.HasExited) {
        Write-Host "Falha ao iniciar $ServiceName" -ForegroundColor Red
        return $null
    }
    
    Write-Host "$ServiceName iniciado com sucesso (PID: $($process.Id))" -ForegroundColor Green
    return $process
}

# Iniciar servicos
Write-Host "`nIniciando microservicos..." -ForegroundColor Yellow

$productService = Start-Service -ServiceName "ProductService" -ProjectPath "src/ProductService/ProductService.csproj" -Port 7001
$orderService = Start-Service -ServiceName "OrderService" -ProjectPath "src/OrderService/OrderService.csproj" -Port 7002
$apiGateway = Start-Service -ServiceName "ApiGateway" -ProjectPath "src/ApiGateway/ApiGateway.csproj" -Port 7000

# Verificar se todos os servicos foram iniciados
if ($productService -and $orderService -and $apiGateway) {
    Write-Host "`nTodos os servicos foram iniciados com sucesso!" -ForegroundColor Green
    Write-Host "`nURLs dos servicos:" -ForegroundColor Yellow
    Write-Host "   - API Gateway: https://localhost:7000" -ForegroundColor White
    Write-Host "   - Product Service: https://localhost:7001" -ForegroundColor White
    Write-Host "   - Order Service: https://localhost:7002" -ForegroundColor White
    Write-Host "`nDocumentacao Swagger:" -ForegroundColor Yellow
    Write-Host "   - API Gateway: https://localhost:7000/swagger" -ForegroundColor White
    Write-Host "   - Product Service: https://localhost:7001/swagger" -ForegroundColor White
    Write-Host "   - Order Service: https://localhost:7002/swagger" -ForegroundColor White
    
    Write-Host "`nUsuarios de teste:" -ForegroundColor Yellow
    Write-Host "   - admin / admin123 (Admin)" -ForegroundColor White
    Write-Host "   - manager / manager123 (Manager)" -ForegroundColor White
    Write-Host "   - customer / customer123 (Customer)" -ForegroundColor White
    
    Write-Host "`nPressione Ctrl+C para parar todos os servicos" -ForegroundColor Red
    
    # Aguardar interrupcao do usuario
    try {
        while ($true) {
            Start-Sleep -Seconds 1
        }
    } catch {
        Write-Host "`nParando servicos..." -ForegroundColor Yellow
        
        if ($productService -and !$productService.HasExited) {
            $productService.Kill()
            Write-Host "ProductService parado" -ForegroundColor Green
        }
        
        if ($orderService -and !$orderService.HasExited) {
            $orderService.Kill()
            Write-Host "OrderService parado" -ForegroundColor Green
        }
        
        if ($apiGateway -and !$apiGateway.HasExited) {
            $apiGateway.Kill()
            Write-Host "ApiGateway parado" -ForegroundColor Green
        }
        
        Write-Host "`nTodos os servicos foram parados. Ate logo!" -ForegroundColor Green
    }
} else {
    Write-Host "`nFalha ao iniciar alguns servicos. Verifique os logs acima." -ForegroundColor Red
    exit 1
}
