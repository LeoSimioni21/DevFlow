using MediatR;
using DevFlow.Application.Interfaces;

namespace DevFlow.Application.Tarefas.Commands.Tarefas;

public class DeleteTarefaCommandHandler : IRequestHandler<DeleteTarefaCommand, bool>
{
    private readonly ITarefasRepository _tarefaRepository;

    public DeleteTarefaCommandHandler(ITarefasRepository tarefasRepository)
    {
        _tarefaRepository = tarefasRepository;
    }

    public async Task<bool> Handle(DeleteTarefaCommand deleteTarefaCommand, CancellationToken cancellationToken)
    {
        var tarefa = await _tarefaRepository.GetTarefaByIdAsync(deleteTarefaCommand.Id);
        if (tarefa is null)
            return false;

        await _tarefaRepository.DeleteByAsync(deleteTarefaCommand.Id);
        await _tarefaRepository.SaveChangesAsync();

        return true;
    }
}
