using Microsoft.AspNetCore.Mvc;
using PressureScada.Api.Security;

namespace PressureScada.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(TokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login(LoginRequest request)
    {
        var users = new Dictionary<string, (string Password, string Role)>(StringComparer.OrdinalIgnoreCase)
        {
            ["operator"] = ("operator123", "Operator"),
            ["admin"] = ("admin123", "Admin")
        };

        if (!users.TryGetValue(request.Username, out var user) || user.Password != request.Password)
        {
            return Unauthorized(new { message = "Неверный логин или пароль." });
        }

        return Ok(new LoginResponse(tokenService.CreateToken(request.Username, user.Role), user.Role));
    }
}

public sealed record LoginRequest(string Username, string Password);
public sealed record LoginResponse(string Token, string Role);
