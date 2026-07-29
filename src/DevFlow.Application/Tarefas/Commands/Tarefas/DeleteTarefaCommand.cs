using MediatR;

namespace DevFlow.Application.Tarefas.Commands.Tarefas;

public record DeleteTarefaCommand(int Id) : IRequest<bool>;
