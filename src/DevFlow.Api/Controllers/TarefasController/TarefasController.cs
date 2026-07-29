using DevFlow.Application;
using DevFlow.Application.Tarefas.Commands.Tarefas;
using DevFlow.Application.Tarefas.DTOs.Tarefas;
using DevFlow.Application.Tarefas.Queries.Tarefas;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevFlow.Api.Controllers;

[ApiController]
[Route("api/tarefas")]
public class TarefasController : ControllerBase
{
    private readonly IMediator _mediator;

    public TarefasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<TarefaResponse>>> GetAll([FromQuery] int? projetoId)
    {
        var tarefas = await _mediator.Send(new GetTarefasQuery(projetoId));
        return Ok(tarefas);
    }

    [HttpGet("~/api/projetos/{projetoId:int}/tarefas")]
    public async Task<ActionResult<List<TarefaResponse>>> GetByProjeto(int projetoId)
    {
        var tarefas = await _mediator.Send(new GetTarefasQuery(projetoId));
        return Ok(tarefas);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TarefaResponse>> GetById(int id)
    {
        var tarefa = await _mediator.Send(new GetTarefaByIdQuery(id));
        return tarefa is null ? NotFound() : Ok(tarefa);
    }

    [HttpPost("~/api/projetos/{projetoId:int}/tarefas")]
    public async Task<ActionResult<TarefaResponse>> Create(int projetoId, [FromBody] CreateTarefaRequest request)
    {
        try
        {
            var tarefa = await _mediator.Send(new CreateTarefaCommand(projetoId, request));
            return CreatedAtAction(nameof(GetById), new { id = tarefa.Id }, tarefa);
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TarefaResponse>> Update(int id, [FromBody] UpdateTarefaRequest request)
    {
        try
        {
            var tarefa = await _mediator.Send(new UpdateTarefaCommand(id, request));
            return tarefa is null ? NotFound() : Ok(tarefa);
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deletado = await _mediator.Send(new DeleteTarefaCommand(id));
        return deletado ? NoContent() : NotFound();
    }
}
