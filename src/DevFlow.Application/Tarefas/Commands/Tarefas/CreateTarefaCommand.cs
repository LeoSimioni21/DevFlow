using DevFlow.Application.Tarefas.DTOs.Tarefas;
using MediatR;

namespace DevFlow.Application.Tarefas.Commands.Tarefas;

public record CreateTarefaCommand(int ProjetoId, CreateTarefaRequest Request) : IRequest<TarefaResponse>;
