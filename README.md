# E-Commerce Microservices

Sistema de e-commerce desenvolvido com arquitetura de microserviços usando .NET 9, Entity Framework, RabbitMQ e JWT para autenticação.

## 🏗️ Arquitetura

O sistema é composto por três microserviços principais:

- **ProductService**: Gerencia o catálogo de produtos e controle de estoque
- **OrderService**: Gerencia pedidos e vendas
- **ApiGateway**: Gateway de API com roteamento e autenticação centralizada

## 🚀 Tecnologias Utilizadas

- **.NET 9** - Framework principal
- **Entity Framework Core** - ORM para acesso a dados
- **SQL Server** - Banco de dados relacional
- **JWT Bearer** - Autenticação e autorização
- **YARP** - Reverse Proxy para API Gateway
- **RabbitMQ** - Comunicação assíncrona entre microserviços
- **Swagger/OpenAPI** - Documentação da API
- **xUnit** - Testes unitários

## 📁 Estrutura do Projeto

```
ECommerceMicroservices/
├── src/
│   ├── Shared/                    # Classes e DTOs compartilhados
│   ├── ProductService/            # Microserviço de produtos
│   ├── OrderService/              # Microserviço de pedidos
│   └── ApiGateway/                # Gateway de API
├── tests/
│   ├── ProductService.Tests/      # Testes do ProductService
│   └── OrderService.Tests/        # Testes do OrderService
└── ECommerceMicroservices.sln     # Solution file
```

## 🛠️ Configuração e Execução

### Pré-requisitos

- .NET 9 SDK
- SQL Server (LocalDB ou SQL Server Express)
- RabbitMQ (opcional para funcionalidades avançadas)

### 1. Clone o repositório

```bash
git clone <repository-url>
cd ECommerceMicroservices
```

### 2. Restaure as dependências

```bash
dotnet restore
```

### 3. Configure as strings de conexão

Edite os arquivos `appsettings.json` de cada serviço para configurar as strings de conexão do banco de dados:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ProductServiceDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 4. Execute os serviços

Execute cada serviço em um terminal separado:

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

### 5. Acesse a documentação

- **ApiGateway**: https://localhost:7000/swagger
- **ProductService**: https://localhost:7001/swagger
- **OrderService**: https://localhost:7002/swagger

## 🔐 Autenticação

O sistema utiliza JWT para autenticação. Usuários pré-configurados:

| Usuário    | Senha       | Role     |
|------------|-------------|----------|
| admin      | admin123    | Admin    |
| manager    | manager123  | Manager  |
| customer   | customer123 | Customer |

### Endpoints de Autenticação

- `POST /api/auth/login` - Login
- `POST /api/auth/register` - Registro de novo usuário
- `GET /api/auth/me` - Informações do usuário logado

## 📋 Funcionalidades

### ProductService

- ✅ Cadastro de produtos
- ✅ Consulta de produtos
- ✅ Atualização de produtos
- ✅ Controle de estoque
- ✅ Reserva e liberação de estoque

### OrderService

- ✅ Criação de pedidos
- ✅ Consulta de pedidos
- ✅ Atualização de status
- ✅ Cancelamento de pedidos
- ✅ Validação de estoque

### ApiGateway

- ✅ Roteamento de requisições
- ✅ Autenticação centralizada
- ✅ Autorização baseada em roles

## 🧪 Testes

Execute os testes unitários:

```bash
# Todos os testes
dotnet test

# Testes específicos
dotnet test tests/ProductService.Tests/
dotnet test tests/OrderService.Tests/
```

## 📊 Endpoints Principais

### Produtos (via ApiGateway)

```
GET    /api/products              # Listar produtos
GET    /api/products/{id}         # Buscar produto por ID
POST   /api/products              # Criar produto (Admin/Manager)
PUT    /api/products/{id}         # Atualizar produto (Admin/Manager)
DELETE /api/products/{id}         # Deletar produto (Admin)
POST   /api/products/{id}/stock   # Atualizar estoque (Admin/Manager)
```

### Pedidos (via ApiGateway)

```
GET    /api/orders                    # Listar pedidos (Admin/Manager)
GET    /api/orders/customer/{id}      # Pedidos do cliente
GET    /api/orders/{id}               # Buscar pedido por ID
POST   /api/orders                    # Criar pedido
PUT    /api/orders/{id}/status        # Atualizar status (Admin/Manager)
POST   /api/orders/{id}/cancel        # Cancelar pedido
```

## 🔄 Fluxo de Venda

1. Cliente autentica via `/api/auth/login`
2. Cliente consulta produtos via `/api/products`
3. Cliente cria pedido via `/api/orders`
4. Sistema valida estoque no ProductService
5. Sistema reserva estoque automaticamente
6. Pedido é confirmado e estoque é reduzido

## 🏗️ Padrões de Arquitetura

- **Domain-Driven Design (DDD)**
- **Repository Pattern**
- **Service Layer Pattern**
- **CQRS** (Command Query Responsibility Segregation)
- **Event-Driven Architecture**

## 🔧 Configurações Avançadas

### RabbitMQ (Opcional)

Para habilitar comunicação assíncrona via RabbitMQ, configure:

```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest"
  }
}
```

### Logs

O sistema utiliza o sistema de logging do .NET com diferentes níveis:

- **Information**: Operações normais
- **Warning**: Situações de atenção
- **Error**: Erros que não impedem o funcionamento
- **Critical**: Erros críticos

## 🚀 Deploy

### Docker (Futuro)

```bash
# Build das imagens
docker build -t productservice ./src/ProductService
docker build -t orderservice ./src/OrderService
docker build -t apigateway ./src/ApiGateway

# Executar containers
docker-compose up -d
```

### Azure (Futuro)

- Azure Container Instances
- Azure SQL Database
- Azure Service Bus
- Azure Application Gateway

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.



---

**Desenvolvido com ❤️ usando .NET 9**
