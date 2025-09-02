using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Services;
using Shared.DTOs;
using System.Security.Claims;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAllOrders()
    {
        try
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar todos os pedidos");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrdersByCustomer(string customerId)
    {
        try
        {
            // Verificar se o usuário pode acessar os pedidos deste cliente
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserRole != "Admin" && currentUserRole != "Manager" && currentUserId != customerId)
            {
                return Forbid("Você só pode visualizar seus próprios pedidos");
            }

            var orders = await _orderService.GetOrdersByCustomerAsync(customerId);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar pedidos do cliente {CustomerId}", customerId);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseDto>> GetOrder(int id)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound($"Pedido com ID {id} não encontrado");

            // Verificar se o usuário pode acessar este pedido
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserRole != "Admin" && currentUserRole != "Manager" && currentUserId != order.CustomerId)
            {
                return Forbid("Você só pode visualizar seus próprios pedidos");
            }

            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar pedido {OrderId}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> CreateOrder(CreateOrderDto createOrderDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verificar se o usuário pode criar pedido para este cliente
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserRole != "Admin" && currentUserRole != "Manager" && currentUserId != createOrderDto.CustomerId)
            {
                return Forbid("Você só pode criar pedidos para si mesmo");
            }

            var order = await _orderService.CreateOrderAsync(createOrderDto);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao criar pedido");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<OrderResponseDto>> UpdateOrderStatus(int id, UpdateOrderStatusDto updateOrderStatusDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _orderService.UpdateOrderStatusAsync(id, updateOrderStatusDto);
            if (order == null)
                return NotFound($"Pedido com ID {id} não encontrado");

            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar status do pedido {OrderId}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult> CancelOrder(int id)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound($"Pedido com ID {id} não encontrado");

            // Verificar se o usuário pode cancelar este pedido
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserRole != "Admin" && currentUserRole != "Manager" && currentUserId != order.CustomerId)
            {
                return Forbid("Você só pode cancelar seus próprios pedidos");
            }

            var result = await _orderService.CancelOrderAsync(id);
            if (!result)
                return NotFound($"Pedido com ID {id} não encontrado");

            return Ok(new { message = "Pedido cancelado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cancelar pedido {OrderId}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}
