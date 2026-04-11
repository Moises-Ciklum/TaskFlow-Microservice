using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoupleCalendar.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CoupleCalendar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto request)
    {
        // 1. Simular la validación en base de datos
        if ((request.Username == "Yo" && request.Password == "1234") ||
            (request.Username == "Mi Pareja" && request.Password == "1234"))
        {
            // 2. Si es válido, generamos el Token
            var token = GenerateJwtToken(request.Username);
            return Ok(new { Token = token });
        }

        return Unauthorized(new { Error = "Credenciales inválidas" });
    }

    private string GenerateJwtToken(string username)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // En el Payload guardamos el nombre del usuario
        var claims = new[] { new Claim(ClaimTypes.Name, username) };

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2), // El token caduca en 2 horas
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}