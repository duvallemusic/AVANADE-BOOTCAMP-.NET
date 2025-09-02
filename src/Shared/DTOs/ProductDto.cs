using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;

public class CreateProductDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
    public decimal Price { get; set; }
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "A quantidade deve ser maior ou igual a zero")]
    public int StockQuantity { get; set; }
}

public class UpdateProductDto
{
    [StringLength(100)]
    public string? Name { get; set; }
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
    public decimal? Price { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "A quantidade deve ser maior ou igual a zero")]
    public int? StockQuantity { get; set; }
}

public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UpdateStockDto
{
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "A quantidade deve ser maior ou igual a zero")]
    public int Quantity { get; set; }
}
