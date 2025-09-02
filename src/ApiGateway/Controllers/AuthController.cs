using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Models;
using Shared.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;

    // Simulação de usuários em memória (em produção, usar banco de dados)
    private static readonly List<User> _users = new()
    {
        new User
        {
            Id = 1,
            Username = "admin",
            Email = "admin@ecommerce.com",
            PasswordHash = HashPassword("admin123"),
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow
        },
        new User
        {
            Id = 2,
            Username = "manager",
            Email = "manager@ecommerce.com",
            PasswordHash = HashPassword("manager123"),
            Role = UserRole.Manager,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow
        },
        new User
        {
            Id = 3,
            Username = "customer",
            Email = "customer@ecommerce.com",
            PasswordHash = HashPassword("customer123"),
            Role = UserRole.Customer,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow
        }
    };

    public AuthController(IJwtService jwtService, ILogger<AuthController> logger, IConfiguration configuration)
    {
        _jwtService = jwtService;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _users.FirstOrDefault(u => u.Username == loginDto.Username);
            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Tentativa de login falhada para usuário: {Username}", loginDto.Username);
                return Unauthorized("Usuário ou senha inválidos");
            }

            var token = _jwtService.GenerateToken(user);

            // Atualizar último login
            user.LastLoginAt = DateTime.UtcNow;

            _logger.LogInformation("Login realizado com sucesso para usuário: {Username}", user.Username);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o login");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verificar se usuário já existe
            if (_users.Any(u => u.Username == registerDto.Username || u.Email == registerDto.Email))
            {
                return BadRequest("Usuário ou email já existem");
            }

            var user = new User
            {
                Id = _users.Count + 1,
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                Role = UserRole.Customer,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow
            };

            _users.Add(user);

            var token = _jwtService.GenerateToken(user);

            _logger.LogInformation("Usuário registrado com sucesso: {Username}", user.Username);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o registro");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult<object> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new
        {
            Id = userId,
            Username = username,
            Email = email,
            Role = role
        });
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}
