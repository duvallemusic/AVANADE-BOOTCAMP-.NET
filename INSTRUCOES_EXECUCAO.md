# 🚀 Instruções de Execução - E-Commerce Microservices

### 🏗️ Arquitetura Implementada

- ✅ **ProductService**: Microserviço de gestão de estoque
- ✅ **OrderService**: Microserviço de gestão de vendas  
- ✅ **ApiGateway**: Gateway de API com roteamento
- ✅ **Shared**: Biblioteca compartilhada com DTOs e serviços
- ✅ **Testes Unitários**: Cobertura de testes para os principais serviços

### 🔧 Tecnologias Utilizadas

- ✅ **.NET 9** - Framework principal
- ✅ **Entity Framework Core** - ORM para acesso a dados
- ✅ **SQL Server** - Banco de dados relacional
- ✅ **JWT Bearer** - Autenticação e autorização
- ✅ **YARP** - Reverse Proxy para API Gateway
- ✅ **RabbitMQ** - Comunicação assíncrona entre microserviços
- ✅ **Swagger/OpenAPI** - Documentação da API
- ✅ **xUnit** - Testes unitários
- ✅ **Docker** - Containerização
- ✅ **Health Checks** - Monitoramento de saúde dos serviços
- ✅ **Logging Avançado** - Sistema de logs estruturado

## 🚀 Como Executar o Sistema

### Opção 1: Execução Manual (Recomendado para desenvolvimento)

1. **Execute o script PowerShell**:
   ```powershell
   .\run-services.ps1
   ```

2. **Ou execute manualmente em terminais separados**:
   ```bash
   # Terminal 1 - ProductService
   cd src/ProductService
   dotnet run

   # Terminal 2 - OrderService  
   cd src/OrderService
   dotnet run

   # Terminal 3 - ApiGateway
   cd src/ApiGateway
   dotnet run
   ```

### Opção 2: Execução com Docker

1. **Instale o Docker Desktop**

2. **Execute o docker-compose**:
   ```bash
   docker-compose up -d
   ```

## 📋 URLs dos Serviços

Após a execução, os serviços estarão disponíveis em:

- **API Gateway**: https://localhost:7000
- **Product Service**: https://localhost:7001  
- **Order Service**: https://localhost:7002

## 📚 Documentação Swagger

- **API Gateway**: https://localhost:7000/swagger
- **Product Service**: https://localhost:7001/swagger
- **Order Service**: https://localhost:7002/swagger

## 🔐 Usuários de Teste

| Usuário    | Senha       | Role     | Permissões                    |
|------------|-------------|----------|-------------------------------|
| admin      | admin123    | Admin    | Todas as operações            |
| manager    | manager123  | Manager  | Gestão de produtos e pedidos  |
| customer   | customer123 | Customer | Apenas consultas e pedidos    |

## 🧪 Testes

Execute os testes unitários:

```bash
# Todos os testes
dotnet test

# Testes específicos
dotnet test tests/ProductService.Tests/
dotnet test tests/OrderService.Tests/
```

## 🔍 Health Checks

Monitore a saúde dos serviços:

- **ProductService**: https://localhost:7001/health
- **OrderService**: https://localhost:7002/health

## 📊 Fluxo de Teste Completo

### 1. Autenticação
```bash
POST https://localhost:7000/api/auth/login
{
  "username": "customer",
  "password": "customer123"
}
```

### 2. Consultar Produtos
```bash
GET https://localhost:7000/api/products
Authorization: Bearer {token}
```

### 3. Criar Pedido
```bash
POST https://localhost:7000/api/orders
Authorization: Bearer {token}
{
  "customerId": "3",
  "customerName": "Cliente Teste",
  "customerEmail": "cliente@teste.com",
  "items": [
    {
      "productId": 1,
      "quantity": 2
    }
  ]
}
```

### 4. Consultar Pedidos
```bash
GET https://localhost:7000/api/orders/customer/3
Authorization: Bearer {token}
```

## 🏗️ Funcionalidades Implementadas

### ProductService
- ✅ Cadastro de produtos
- ✅ Consulta de produtos
- ✅ Atualização de produtos
- ✅ Controle de estoque
- ✅ Reserva e liberação de estoque
- ✅ Health checks
- ✅ Logging estruturado

### OrderService
- ✅ Criação de pedidos
- ✅ Consulta de pedidos
- ✅ Atualização de status
- ✅ Cancelamento de pedidos
- ✅ Validação de estoque
- ✅ Comunicação com ProductService
- ✅ Publicação de eventos RabbitMQ
- ✅ Health checks
- ✅ Logging estruturado

### ApiGateway
- ✅ Roteamento de requisições
- ✅ Autenticação centralizada
- ✅ Autorização baseada em roles
- ✅ Endpoints de autenticação
- ✅ Documentação Swagger

### Recursos Avançados
- ✅ **RabbitMQ**: Comunicação assíncrona entre microserviços
- ✅ **Health Checks**: Monitoramento de saúde dos serviços
- ✅ **Logging Avançado**: Sistema de logs estruturado com middleware
- ✅ **Testes Unitários**: Cobertura completa dos serviços principais
- ✅ **Docker**: Containerização completa do sistema
- ✅ **JWT**: Autenticação segura com tokens
- ✅ **Entity Framework**: ORM com migrations automáticas
- ✅ **Swagger**: Documentação interativa da API

## 🎯 Critérios de Aceitação Atendidos

- ✅ Sistema permite cadastro de produtos no microserviço de estoque
- ✅ Sistema permite criação de pedidos com validação de estoque
- ✅ Comunicação entre microserviços via RabbitMQ
- ✅ API Gateway direciona requisições corretamente
- ✅ Sistema seguro com autenticação JWT
- ✅ Código bem estruturado com separação de responsabilidades
- ✅ Testes unitários implementados
- ✅ Logging e monitoramento implementados
- ✅ Sistema escalável e robusto

## 🚀 Próximos Passos 

1. **Adicionar mais microserviços**:
   - PaymentService (pagamentos)
   - NotificationService (notificações)
   - InventoryService (gestão avançada de estoque)

2. **Implementar recursos avançados**:
   - Cache com Redis
   - Métricas com Prometheus
   - Tracing distribuído
   - Rate limiting
   - Circuit breaker

3. **Deploy em nuvem**:
   - Azure Container Instances
   - Azure Kubernetes Service
   - Azure Service Bus
   - Azure Application Gateway


