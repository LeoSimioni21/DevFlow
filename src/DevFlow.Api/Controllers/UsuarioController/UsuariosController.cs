using DevFlow.Application;
using DevFlow.Application.Usuarios.Commands.Usuarios;
using DevFlow.Application.Usuarios.DTOs.Usuarios;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevFlow.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsuariosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioResponse>> Create([FromBody] CreateUserRequest request)
    {
        try
        {
            var usuario = await _mediator.Send(new CreateUserCommand(request));
            return StatusCode(201, usuario);
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UsuarioResponse>> Update(int id, [FromBody] UpdateUsuarioRequest request)
    {
        try
        {
            var usuario = await _mediator.Send(new UpdateUsuarioCommand(id, request));
            return usuario is null ? NotFound() : Ok(usuario);
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }
}
