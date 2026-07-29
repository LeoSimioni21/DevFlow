using MediatR;
using DevFlow.Application.Interfaces;

namespace DevFlow.Application.Projetos.Commands.Projetos;

public class DeleteProjetoCommandHandler : IRequestHandler<DeleteProjetoCommand, bool>
{
    private readonly IProjetosRepository _projetoRepository;
    private readonly ITarefasRepository _tarefaRepository;

    public DeleteProjetoCommandHandler(IProjetosRepository projetosRepository, ITarefasRepository tarefasRepository)
    {
        _projetoRepository = projetosRepository;
        _tarefaRepository = tarefasRepository;
    }

    public async Task<bool> Handle(DeleteProjetoCommand deleteProjetoCommand, CancellationToken cancellationToken)
    {
        var projeto = await _projetoRepository.GetProjetoByIdAsync(deleteProjetoCommand.Id);
        if (projeto is null)
            return false;

        // Delete em cascata: tarefas primeiro, depois o projeto
        var tarefas = await _tarefaRepository.GetTarefasByProjetoIdAsync(deleteProjetoCommand.Id);
        foreach (var tarefa in tarefas)
            await _tarefaRepository.DeleteByAsync(tarefa.Id);

        await _projetoRepository.DeleteByAsync(deleteProjetoCommand.Id);
        await _projetoRepository.SaveChangesAsync();

        return true;
    }
}
