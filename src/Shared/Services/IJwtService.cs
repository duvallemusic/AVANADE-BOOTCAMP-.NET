using Shared.Models;

namespace Shared.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    bool ValidateToken(string token);
    string GetUserIdFromToken(string token);
    string GetUserRoleFromToken(string token);
}
