using MediatR;
using DevFlow.Application.Tarefas.DTOs.Tarefas;
using DevFlow.Application.Interfaces;
using DevFlow.Application.Interfaces.Usuarios;

namespace DevFlow.Application.Tarefas.Queries.Tarefas;

public class GetTarefasQueryHandler : IRequestHandler<GetTarefasQuery, List<TarefaResponse>>
{
    private readonly ITarefasRepository _tarefaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public GetTarefasQueryHandler(ITarefasRepository tarefasRepository, IUsuarioRepository usuarioRepository)
    {
        _tarefaRepository = tarefasRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<List<TarefaResponse>> Handle(GetTarefasQuery getTarefasQuery, CancellationToken cancellationToken)
    {
        var tarefas = getTarefasQuery.ProjetoId.HasValue
            ? await _tarefaRepository.GetTarefasByProjetoIdAsync(getTarefasQuery.ProjetoId.Value)
            : await _tarefaRepository.GetTarefasAsync();

        if (!string.IsNullOrWhiteSpace(getTarefasQuery.Codigo))
        {
            tarefas = tarefas
                .Where(tarefa => tarefa.Codigo.Contains(getTarefasQuery.Codigo, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var usuarios = await _usuarioRepository.GetAllAsync();
        var nomesPorId = usuarios.ToDictionary(u => u.Id, u => u.Nome);

        return tarefas
            .Select(tarefa => tarefa.ToResponse(
                tarefa.ResponsavelId.HasValue ? nomesPorId.GetValueOrDefault(tarefa.ResponsavelId.Value) : null))
            .ToList();
    }
}
