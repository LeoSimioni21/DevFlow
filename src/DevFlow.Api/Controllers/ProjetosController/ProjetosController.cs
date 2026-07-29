using DevFlow.Application;
using DevFlow.Application.Projetos.Commands.Projetos;
using DevFlow.Application.Projetos.DTOs.Projetos;
using DevFlow.Application.Projetos.Queries.Projetos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevFlow.Api.Controllers;

[ApiController]
[Route("api/projetos")]
public class ProjetosController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjetosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProjetoResponse>>> GetAll()
    {
        var projetos = await _mediator.Send(new GetProjetosQuery());
        return Ok(projetos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjetoResponse>> GetById(int id)
    {
        var projeto = await _mediator.Send(new GetProjetoByIdQuery(id));
        return projeto is null ? NotFound() : Ok(projeto);
    }

    [HttpPost]
    public async Task<ActionResult<ProjetoResponse>> Create([FromBody] CreateProjetoRequest request)
    {
        try
        {
            var projeto = await _mediator.Send(new CreateProjetoCommand(request));
            return CreatedAtAction(nameof(GetById), new { id = projeto.Id }, projeto);
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deletado = await _mediator.Send(new DeleteProjetoCommand(id));
        return deletado ? NoContent() : NotFound();
    }
}
