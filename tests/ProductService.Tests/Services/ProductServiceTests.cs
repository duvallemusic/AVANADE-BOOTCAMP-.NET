using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductService.Data;
using ProductService.Services;
using Shared.DTOs;
using Xunit;

namespace ProductService.Tests.Services;

public class ProductServiceTests : IDisposable
{
    private readonly ProductDbContext _context;
    private readonly ProductService.Services.ProductService _productService;
    private readonly ILogger<ProductService.Services.ProductService> _logger;

    public ProductServiceTests()
    {
        var options = new DbContextOptionsBuilder<ProductDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ProductDbContext(options);
        _logger = Mock.Of<ILogger<ProductService.Services.ProductService>>();
        _productService = new ProductService.Services.ProductService(_context, _logger);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldCreateProduct_WhenValidData()
    {
        // Arrange
        var createProductDto = new CreateProductDto
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            StockQuantity = 10
        };

        // Act
        var result = await _productService.CreateProductAsync(createProductDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createProductDto.Name, result.Name);
        Assert.Equal(createProductDto.Description, result.Description);
        Assert.Equal(createProductDto.Price, result.Price);
        Assert.Equal(createProductDto.StockQuantity, result.StockQuantity);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var createProductDto = new CreateProductDto
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            StockQuantity = 10
        };
        var createdProduct = await _productService.CreateProductAsync(createProductDto);

        // Act
        var result = await _productService.GetProductByIdAsync(createdProduct.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdProduct.Id, result.Id);
        Assert.Equal(createdProduct.Name, result.Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Act
        var result = await _productService.GetProductByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldUpdateProduct_WhenProductExists()
    {
        // Arrange
        var createProductDto = new CreateProductDto
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            StockQuantity = 10
        };
        var createdProduct = await _productService.CreateProductAsync(createProductDto);

        var updateProductDto = new UpdateProductDto
        {
            Name = "Updated Product",
            Price = 149.99m
        };

        // Act
        var result = await _productService.UpdateProductAsync(createdProduct.Id, updateProductDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateProductDto.Name, result.Name);
        Assert.Equal(updateProductDto.Price, result.Price);
        Assert.Equal(createdProduct.Description, result.Description); // Não alterado
    }

    [Fact]
    public async Task UpdateStockAsync_ShouldUpdateStock_WhenProductExists()
    {
        // Arrange
        var createProductDto = new CreateProductDto
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            StockQuantity = 10
        };
        var createdProduct = await _productService.CreateProductAsync(createProductDto);

        // Act
        var result = await _productService.UpdateStockAsync(createdProduct.Id, 20);

        // Assert
        Assert.True(result);
        var updatedProduct = await _productService.GetProductByIdAsync(createdProduct.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equal(20, updatedProduct.StockQuantity);
    }

    [Fact]
    public async Task ReserveStockAsync_ShouldReserveStock_WhenSufficientStock()
    {
        // Arrange
        var createProductDto = new CreateProductDto
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            StockQuantity = 10
        };
        var createdProduct = await _productService.CreateProductAsync(createProductDto);

        // Act
        var result = await _productService.ReserveStockAsync(createdProduct.Id, 5);

        // Assert
        Assert.True(result);
        var updatedProduct = await _productService.GetProductByIdAsync(createdProduct.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equal(5, updatedProduct.StockQuantity);
    }

    [Fact]
    public async Task ReserveStockAsync_ShouldNotReserveStock_WhenInsufficientStock()
    {
        // Arrange
        var createProductDto = new CreateProductDto
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            StockQuantity = 10
        };
        var createdProduct = await _productService.CreateProductAsync(createProductDto);

        // Act
        var result = await _productService.ReserveStockAsync(createdProduct.Id, 15);

        // Assert
        Assert.False(result);
        var product = await _productService.GetProductByIdAsync(createdProduct.Id);
        Assert.NotNull(product);
        Assert.Equal(10, product.StockQuantity); // Stock não deve ter mudado
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldDeleteProduct_WhenProductExists()
    {
        // Arrange
        var createProductDto = new CreateProductDto
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            StockQuantity = 10
        };
        var createdProduct = await _productService.CreateProductAsync(createProductDto);

        // Act
        var result = await _productService.DeleteProductAsync(createdProduct.Id);

        // Assert
        Assert.True(result);
        var deletedProduct = await _productService.GetProductByIdAsync(createdProduct.Id);
        Assert.Null(deletedProduct);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
