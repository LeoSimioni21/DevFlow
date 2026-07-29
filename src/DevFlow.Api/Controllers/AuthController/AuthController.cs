using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevFlow.Application.Usuarios.Commands.Usuarios;
using DevFlow.Application.Usuarios.DTOs.Usuarios;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DevFlow.Api.Controllers;

public record LoginResponse(string Token, UsuarioResponse Usuario);

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public AuthController(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var usuario = await _mediator.Send(new LoginCommand(request));
        if (usuario is null)
            return Unauthorized(new { erro = "Email ou senha inválidos" });

        var token = GerarToken(usuario);
        return Ok(new LoginResponse(token, usuario));
    }

    private string GerarToken(UsuarioResponse usuario)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
        var horas = double.Parse(_configuration["Jwt:ExpiraEmHoras"] ?? "8");

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome ?? string.Empty),
                new Claim(ClaimTypes.Email, usuario.Email ?? string.Empty),
            }),
            Expires = DateTime.UtcNow.AddHours(horas),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
