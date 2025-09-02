using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using Shared.DTOs;
using Shared.Models;

namespace ProductService.Services;

public class ProductService : IProductService
{
    private readonly ProductDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(ProductDbContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
    {
        var products = await _context.Products.ToListAsync();
        return products.Select(MapToResponseDto);
    }

    public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        return product != null ? MapToResponseDto(product) : null;
    }

    public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        var product = new Product
        {
            Name = createProductDto.Name,
            Description = createProductDto.Description,
            Price = createProductDto.Price,
            StockQuantity = createProductDto.StockQuantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Produto criado: {ProductId} - {ProductName}", product.Id, product.Name);
        return MapToResponseDto(product);
    }

    public async Task<ProductResponseDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return null;

        if (updateProductDto.Name != null)
            product.Name = updateProductDto.Name;
        
        if (updateProductDto.Description != null)
            product.Description = updateProductDto.Description;
        
        if (updateProductDto.Price.HasValue)
            product.Price = updateProductDto.Price.Value;
        
        if (updateProductDto.StockQuantity.HasValue)
            product.StockQuantity = updateProductDto.StockQuantity.Value;

        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Produto atualizado: {ProductId} - {ProductName}", product.Id, product.Name);
        return MapToResponseDto(product);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Produto removido: {ProductId} - {ProductName}", product.Id, product.Name);
        return true;
    }

    public async Task<bool> UpdateStockAsync(int productId, int quantity)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            return false;

        var previousQuantity = product.StockQuantity;
        product.StockQuantity = quantity;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Estoque atualizado para produto {ProductId}: {PreviousQuantity} -> {NewQuantity}", 
            productId, previousQuantity, quantity);

        return true;
    }

    public async Task<bool> ReserveStockAsync(int productId, int quantity)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null || product.StockQuantity < quantity)
            return false;

        product.StockQuantity -= quantity;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Estoque reservado para produto {ProductId}: {Quantity} unidades", 
            productId, quantity);

        return true;
    }

    public async Task<bool> ReleaseStockAsync(int productId, int quantity)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            return false;

        product.StockQuantity += quantity;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Estoque liberado para produto {ProductId}: {Quantity} unidades", 
            productId, quantity);

        return true;
    }

    private static ProductResponseDto MapToResponseDto(Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
