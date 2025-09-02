# 🐳 Configuração Docker + WSL para E-Commerce Microservices

## 📋 Pré-requisitos

### 1. Instalar Docker Desktop
- Baixe e instale o [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Certifique-se de que o WSL 2 está habilitado
- No Docker Desktop, vá em Settings > General > "Use the WSL 2 based engine"

### 2. Instalar WSL 2 (se não estiver instalado)
```powershell
# No PowerShell como Administrador
wsl --install
# ou para instalar uma distribuição específica
wsl --install -d Ubuntu
```

### 3. Verificar instalação
```powershell
# Verificar WSL
wsl --list --verbose

# Verificar Docker
docker --version
docker-compose --version
```

## 🚀 Como Executar

### Opção 1: PowerShell (Windows)
```powershell
# Iniciar todos os serviços
.\docker-start.ps1

# Testar serviços
.\docker-test.ps1

# Ver logs
.\docker-logs.ps1

# Parar serviços
.\docker-stop.ps1
```

### Opção 2: WSL (Linux)
```bash
# Navegar para o diretório do projeto
cd /mnt/c/Users/bruno/PROJETOS\ .NET/

# Iniciar serviços
docker-compose up -d

# Testar serviços
curl -f http://localhost:7001/swagger
curl -f http://localhost:7002/swagger
curl -f http://localhost:7000/swagger

# Ver logs
docker-compose logs -f

# Parar serviços
docker-compose down
```

### Opção 3: Comandos Docker Diretos
```bash
# Construir imagens
docker-compose build

# Iniciar serviços
docker-compose up -d

# Ver status
docker-compose ps

# Ver logs de um serviço específico
docker-compose logs -f productservice

# Parar serviços
docker-compose down
```

## 🌐 URLs dos Serviços

- **ProductService**: http://localhost:7001/swagger
- **OrderService**: http://localhost:7002/swagger
- **ApiGateway**: http://localhost:7000/swagger
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)

## 🔧 Solução de Problemas

### Se o Docker não iniciar:
1. Verifique se o WSL 2 está rodando:
   ```powershell
   wsl --status
   ```

2. Reinicie o Docker Desktop

3. Verifique se o Hyper-V está habilitado (Windows Pro/Enterprise)

### Se os serviços não iniciarem:
1. Verifique os logs:
   ```bash
   docker-compose logs
   ```

2. Verifique se as portas estão livres:
   ```powershell
   netstat -an | findstr "7000\|7001\|7002\|15672"
   ```

3. Reinicie os serviços:
   ```bash
   docker-compose restart
   ```

### Se houver problemas de permissão no WSL:
```bash
# Dar permissão de execução aos scripts
chmod +x *.ps1

# Ou executar diretamente com docker-compose
docker-compose up -d
```

## 📊 Monitoramento

### Ver logs em tempo real:
```bash
# Todos os serviços
docker-compose logs -f

# Serviço específico
docker-compose logs -f productservice
```

### Ver status dos containers:
```bash
docker-compose ps
```

### Ver uso de recursos:
```bash
docker stats
```

## 🎯 Vantagens do Docker + WSL

1. **Isolamento**: Cada serviço roda em seu próprio container
2. **Consistência**: Mesmo ambiente em desenvolvimento e produção
3. **Escalabilidade**: Fácil de escalar horizontalmente
4. **Dependências**: RabbitMQ e outros serviços gerenciados automaticamente
5. **Portabilidade**: Funciona em qualquer máquina com Docker
6. **WSL Integration**: Melhor performance no Windows

## 🚀 Próximos Passos

1. **Testar APIs**: Use o Swagger UI para testar os endpoints
2. **Monitorar**: Use o RabbitMQ Management para ver mensagens
3. **Desenvolver**: Faça alterações no código e reconstrua as imagens
4. **Produção**: Use o mesmo docker-compose.yml em produção (com ajustes de configuração)
