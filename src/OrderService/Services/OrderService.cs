using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using Shared.DTOs;
using Shared.Models;
using Shared.Services;
using Shared.Events;
using System.Text.Json;

namespace OrderService.Services;

public class OrderService : IOrderService
{
    private readonly OrderDbContext _context;
    private readonly ILogger<OrderService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IMessageService _messageService;

    public OrderService(OrderDbContext context, ILogger<OrderService> logger, HttpClient httpClient, IConfiguration configuration, IMessageService messageService)
    {
        _context = context;
        _logger = logger;
        _httpClient = httpClient;
        _configuration = configuration;
        _messageService = messageService;
    }

    public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(MapToResponseDto);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetOrdersByCustomerAsync(string customerId)
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(MapToResponseDto);
    }

    public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order != null ? MapToResponseDto(order) : null;
    }

    public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto createOrderDto)
    {
        // Validar disponibilidade dos produtos
        foreach (var item in createOrderDto.Items)
        {
            var isAvailable = await ValidateProductAvailabilityAsync(item.ProductId, item.Quantity);
            if (!isAvailable)
            {
                throw new InvalidOperationException($"Produto {item.ProductId} não está disponível na quantidade solicitada");
            }
        }

        // Criar o pedido
        var order = new Order
        {
            CustomerId = createOrderDto.CustomerId,
            CustomerName = createOrderDto.CustomerName,
            CustomerEmail = createOrderDto.CustomerEmail,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Adicionar itens ao pedido
        foreach (var itemDto in createOrderDto.Items)
        {
            var productName = await GetProductNameAsync(itemDto.ProductId);
            var unitPrice = await GetProductPriceAsync(itemDto.ProductId);

            var orderItem = new OrderItem
            {
                ProductId = itemDto.ProductId,
                ProductName = productName,
                Quantity = itemDto.Quantity,
                UnitPrice = unitPrice
            };

            order.Items.Add(orderItem);
        }

        // Calcular total
        order.TotalAmount = order.Items.Sum(i => i.TotalPrice);

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Pedido criado: {OrderId} - Cliente: {CustomerId}", order.Id, order.CustomerId);

        // Reservar estoque
        foreach (var item in order.Items)
        {
            await ReserveStockAsync(item.ProductId, item.Quantity);
        }

        // Atualizar status para confirmado
        order.Status = OrderStatus.Confirmed;
        order.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Pedido confirmado: {OrderId}", order.Id);

        // Publicar evento de pedido criado
        try
        {
            var orderEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(i => new OrderItemEvent
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                }).ToList()
            };

            await _messageService.PublishAsync(orderEvent, "ecommerce-events", "order.created");
            _logger.LogInformation("Evento de pedido criado publicado: {OrderId}", order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar evento de pedido criado: {OrderId}", order.Id);
        }

        return MapToResponseDto(order);
    }

    public async Task<OrderResponseDto?> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto updateOrderStatusDto)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return null;

        order.Status = updateOrderStatusDto.Status;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Status do pedido {OrderId} atualizado para {Status}", id, updateOrderStatusDto.Status);
        return MapToResponseDto(order);
    }

    public async Task<bool> CancelOrderAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return false;

        if (order.Status == OrderStatus.Cancelled)
            return true;

        // Liberar estoque se o pedido estava confirmado
        if (order.Status == OrderStatus.Confirmed)
        {
            foreach (var item in order.Items)
            {
                await ReleaseStockAsync(item.ProductId, item.Quantity);
            }
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Pedido {OrderId} cancelado", id);
        return true;
    }

    public async Task<bool> ValidateProductAvailabilityAsync(int productId, int quantity)
    {
        try
        {
            var productServiceUrl = _configuration["ProductService:BaseUrl"] ?? "https://localhost:7001";
            var response = await _httpClient.GetAsync($"{productServiceUrl}/api/products/{productId}");

            if (!response.IsSuccessStatusCode)
                return false;

            var content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductResponseDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return product != null && product.StockQuantity >= quantity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar disponibilidade do produto {ProductId}", productId);
            return false;
        }
    }

    public async Task<decimal> GetProductPriceAsync(int productId)
    {
        try
        {
            var productServiceUrl = _configuration["ProductService:BaseUrl"] ?? "https://localhost:7001";
            var response = await _httpClient.GetAsync($"{productServiceUrl}/api/products/{productId}");

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Produto {productId} não encontrado");

            var content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductResponseDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return product?.Price ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter preço do produto {ProductId}", productId);
            throw;
        }
    }

    public async Task<string> GetProductNameAsync(int productId)
    {
        try
        {
            var productServiceUrl = _configuration["ProductService:BaseUrl"] ?? "https://localhost:7001";
            var response = await _httpClient.GetAsync($"{productServiceUrl}/api/products/{productId}");

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Produto {productId} não encontrado");

            var content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductResponseDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return product?.Name ?? "Produto não encontrado";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter nome do produto {ProductId}", productId);
            throw;
        }
    }

    private async Task<bool> ReserveStockAsync(int productId, int quantity)
    {
        try
        {
            var productServiceUrl = _configuration["ProductService:BaseUrl"] ?? "https://localhost:7001";
            var updateStockDto = new { ProductId = productId, Quantity = quantity };
            var json = JsonSerializer.Serialize(updateStockDto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{productServiceUrl}/api/products/{productId}/reserve", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao reservar estoque do produto {ProductId}", productId);
            return false;
        }
    }

    private async Task<bool> ReleaseStockAsync(int productId, int quantity)
    {
        try
        {
            var productServiceUrl = _configuration["ProductService:BaseUrl"] ?? "https://localhost:7001";
            var updateStockDto = new { ProductId = productId, Quantity = quantity };
            var json = JsonSerializer.Serialize(updateStockDto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{productServiceUrl}/api/products/{productId}/release", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao liberar estoque do produto {ProductId}", productId);
            return false;
        }
    }

    private static OrderResponseDto MapToResponseDto(Order order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.Items.Select(i => new OrderItemResponseDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList()
        };
    }
}
