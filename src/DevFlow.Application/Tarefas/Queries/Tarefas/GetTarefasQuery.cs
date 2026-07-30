using DevFlow.Application.Tarefas.DTOs.Tarefas;
using MediatR;

namespace DevFlow.Application.Tarefas.Queries.Tarefas;

public record GetTarefasQuery(int? ProjetoId, string? Codigo = null) : IRequest<List<TarefaResponse>>;
