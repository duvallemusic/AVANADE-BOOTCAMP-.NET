# 🚀 Como Executar os Microserviços

## 📋 Pré-requisitos
- .NET 9 SDK instalado
- PowerShell (Windows)

## 🎯 Opções para Iniciar os Serviços

### Opção 1: Iniciar Todos os Serviços (Recomendado)
```powershell
.\start-all-services.ps1
```
Este script irá:
- Abrir 3 janelas do PowerShell
- Cada janela executará um serviço
- Aguardar alguns segundos para inicialização

### Opção 2: Iniciar Serviços Individualmente
```powershell
# Terminal 1 - ProductService
.\start-product-service.ps1

# Terminal 2 - OrderService  
.\start-order-service.ps1

# Terminal 3 - ApiGateway
.\start-api-gateway.ps1
```

### Opção 3: Iniciar Manualmente (Comandos Diretos)
```powershell
# Terminal 1 - ProductService
cd src/ProductService
dotnet run --urls "http://localhost:7001"

# Terminal 2 - OrderService
cd src/OrderService  
dotnet run --urls "http://localhost:7002"

# Terminal 3 - ApiGateway
cd src/ApiGateway
dotnet run --urls "http://localhost:7000"
```

## 🧪 Testar os Serviços

### Verificar se Estão Funcionando
```powershell
.\test-services.ps1
```

### URLs dos Serviços
- **ProductService**: http://localhost:7001/swagger
- **OrderService**: http://localhost:7002/swagger
- **ApiGateway**: http://localhost:7000/swagger

## 🔧 Solução de Problemas

### Se os Serviços Não Iniciarem:
1. Verifique se o .NET 9 está instalado:
   ```powershell
   dotnet --version
   ```

2. Restaure as dependências:
   ```powershell
   dotnet restore ECommerceMicroservices.sln
   ```

3. Execute os testes para verificar se tudo está funcionando:
   ```powershell
   dotnet test ECommerceMicroservices.sln
   ```

### Se as Portas Estiverem Ocupadas:
```powershell
# Verificar portas em uso
netstat -an | findstr "7000\|7001\|7002"

# Parar processos dotnet
Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force
```

## 📊 Funcionalidades dos Serviços

### ProductService (Porta 7001)
- ✅ Cadastrar produtos
- ✅ Listar produtos
- ✅ Atualizar estoque
- ✅ Reservar estoque

### OrderService (Porta 7002)
- ✅ Criar pedidos
- ✅ Listar pedidos
- ✅ Atualizar status
- ✅ Cancelar pedidos

### ApiGateway (Porta 7000)
- ✅ Roteamento para outros serviços
- ✅ Autenticação JWT
- ✅ Documentação Swagger

## 🎉 Pronto!
Após iniciar os serviços, você pode:
1. Acessar as URLs do Swagger para testar as APIs
2. Usar o Postman ou similar para fazer requisições
3. Verificar os logs nos terminais dos serviços
