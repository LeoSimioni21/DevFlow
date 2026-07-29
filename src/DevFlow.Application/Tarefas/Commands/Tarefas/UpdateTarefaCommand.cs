using DevFlow.Application.Tarefas.DTOs.Tarefas;
using MediatR;

namespace DevFlow.Application.Tarefas.Commands.Tarefas;

public record UpdateTarefaCommand(int Id, UpdateTarefaRequest Request) : IRequest<TarefaResponse?>;
