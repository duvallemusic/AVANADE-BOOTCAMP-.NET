using Shared.DTOs;
using Shared.Models;

namespace OrderService.Services;

public interface IOrderService
{
    Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync();
    Task<IEnumerable<OrderResponseDto>> GetOrdersByCustomerAsync(string customerId);
    Task<OrderResponseDto?> GetOrderByIdAsync(int id);
    Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto createOrderDto);
    Task<OrderResponseDto?> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto updateOrderStatusDto);
    Task<bool> CancelOrderAsync(int id);
    Task<bool> ValidateProductAvailabilityAsync(int productId, int quantity);
    Task<decimal> GetProductPriceAsync(int productId);
    Task<string> GetProductNameAsync(int productId);
}
