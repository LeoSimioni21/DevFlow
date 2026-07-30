using MediatR;
using DevFlow.Application.Tarefas.DTOs.Tarefas;
using DevFlow.Application.Interfaces;
using DevFlow.Application.Interfaces.Usuarios;

namespace DevFlow.Application.Tarefas.Queries.Tarefas;

public class GetTarefaByIdQueryHandler : IRequestHandler<GetTarefaByIdQuery, TarefaResponse?>
{
    private readonly ITarefasRepository _tarefaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public GetTarefaByIdQueryHandler(ITarefasRepository tarefasRepository, IUsuarioRepository usuarioRepository)
    {
        _tarefaRepository = tarefasRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<TarefaResponse?> Handle(GetTarefaByIdQuery getTarefaByIdQuery, CancellationToken cancellationToken)
    {
        var tarefa = await _tarefaRepository.GetTarefaByIdAsync(getTarefaByIdQuery.Id);
        if (tarefa is null)
            return null;

        var responsavelNome = tarefa.ResponsavelId.HasValue
            ? (await _usuarioRepository.GetByIdAsync(tarefa.ResponsavelId.Value))?.Nome
            : null;

        return tarefa.ToResponse(responsavelNome);
    }
}
