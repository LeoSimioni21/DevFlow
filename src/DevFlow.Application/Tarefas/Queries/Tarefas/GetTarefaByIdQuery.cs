using DevFlow.Application.Tarefas.DTOs.Tarefas;
using MediatR;

namespace DevFlow.Application.Tarefas.Queries.Tarefas;

public record GetTarefaByIdQuery(int Id) : IRequest<TarefaResponse?>;
