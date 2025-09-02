using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Data;
using OrderService.Services;
using Shared.DTOs;
using Shared.Models;
using RichardSzalay.MockHttp;
using System.Text.Json;
using Xunit;

namespace OrderService.Tests.Services;

public class OrderServiceTests : IDisposable
{
    private readonly OrderDbContext _context;
    private readonly OrderService.Services.OrderService _orderService;
    private readonly ILogger<OrderService.Services.OrderService> _logger;
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OrderServiceTests()
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new OrderDbContext(options);
        _logger = Mock.Of<ILogger<OrderService.Services.OrderService>>();
        _mockHttp = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_mockHttp);
        _configuration = Mock.Of<IConfiguration>();

        Mock.Get(_configuration)
            .Setup(x => x["ProductService:BaseUrl"])
            .Returns("https://localhost:7001");

        var messageService = Mock.Of<Shared.Services.IMessageService>();
        _orderService = new OrderService.Services.OrderService(_context, _logger, _httpClient, _configuration, messageService);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldCreateOrder_WhenValidData()
    {
        // Arrange
        var productId = 1;
        var productName = "Test Product";
        var productPrice = 99.99m;
        var stockQuantity = 10;

        // Mock HTTP responses
        _mockHttp.When($"https://localhost:7001/api/products/{productId}")
            .Respond("application/json", JsonSerializer.Serialize(new ProductResponseDto
            {
                Id = productId,
                Name = productName,
                Price = productPrice,
                StockQuantity = stockQuantity
            }));

        _mockHttp.When($"https://localhost:7001/api/products/{productId}/reserve")
            .Respond("application/json", "{}");

        var createOrderDto = new CreateOrderDto
        {
            CustomerId = "customer1",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            Items = new List<CreateOrderItemDto>
            {
                new() { ProductId = productId, Quantity = 2 }
            }
        };

        // Act
        var result = await _orderService.CreateOrderAsync(createOrderDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createOrderDto.CustomerId, result.CustomerId);
        Assert.Equal(createOrderDto.CustomerName, result.CustomerName);
        Assert.Equal(createOrderDto.CustomerEmail, result.CustomerEmail);
        Assert.Equal(OrderStatus.Confirmed, result.Status);
        Assert.Equal(productPrice * 2, result.TotalAmount);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldThrowException_WhenProductNotAvailable()
    {
        // Arrange
        var productId = 1;
        var stockQuantity = 5;

        // Mock HTTP response with insufficient stock
        _mockHttp.When($"https://localhost:7001/api/products/{productId}")
            .Respond("application/json", JsonSerializer.Serialize(new ProductResponseDto
            {
                Id = productId,
                Name = "Test Product",
                Price = 99.99m,
                StockQuantity = stockQuantity
            }));

        var createOrderDto = new CreateOrderDto
        {
            CustomerId = "customer1",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            Items = new List<CreateOrderItemDto>
            {
                new() { ProductId = productId, Quantity = 10 } // Mais que o disponível
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _orderService.CreateOrderAsync(createOrderDto));
    }

    [Fact]
    public async Task GetOrderByIdAsync_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        var order = new Order
        {
            CustomerId = "customer1",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            Status = OrderStatus.Confirmed,
            TotalAmount = 199.98m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Items = new List<OrderItem>
            {
                new()
                {
                    ProductId = 1,
                    ProductName = "Test Product",
                    Quantity = 2,
                    UnitPrice = 99.99m
                }
            }
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Act
        var result = await _orderService.GetOrderByIdAsync(order.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
        Assert.Equal(order.CustomerId, result.CustomerId);
        Assert.Equal(order.Status, result.Status);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_ShouldUpdateStatus_WhenOrderExists()
    {
        // Arrange
        var order = new Order
        {
            CustomerId = "customer1",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            Status = OrderStatus.Pending,
            TotalAmount = 199.98m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var updateStatusDto = new UpdateOrderStatusDto
        {
            Status = OrderStatus.Processing
        };

        // Act
        var result = await _orderService.UpdateOrderStatusAsync(order.Id, updateStatusDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OrderStatus.Processing, result.Status);
    }

    [Fact]
    public async Task CancelOrderAsync_ShouldCancelOrder_WhenOrderExists()
    {
        // Arrange
        var order = new Order
        {
            CustomerId = "customer1",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            Status = OrderStatus.Confirmed,
            TotalAmount = 199.98m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Items = new List<OrderItem>
            {
                new()
                {
                    ProductId = 1,
                    ProductName = "Test Product",
                    Quantity = 2,
                    UnitPrice = 99.99m
                }
            }
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Mock HTTP response for stock release
        _mockHttp.When($"https://localhost:7001/api/products/1/release")
            .Respond("application/json", "{}");

        // Act
        var result = await _orderService.CancelOrderAsync(order.Id);

        // Assert
        Assert.True(result);
        var cancelledOrder = await _orderService.GetOrderByIdAsync(order.Id);
        Assert.NotNull(cancelledOrder);
        Assert.Equal(OrderStatus.Cancelled, cancelledOrder.Status);
    }

    public void Dispose()
    {
        _context.Dispose();
        _httpClient.Dispose();
        _mockHttp.Dispose();
    }
}
