using MediatR;
using DevFlow.Application.Tarefas.DTOs.Tarefas;
using DevFlow.Application.Interfaces;
using DevFlow.Application.Interfaces.Usuarios;
using DevFlow.Domain.Tarefas;

namespace DevFlow.Application.Tarefas.Commands.Tarefas;

public class CreateTarefaCommandHandler : IRequestHandler<CreateTarefaCommand, TarefaResponse>
{
    private readonly ITarefasRepository _tarefaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public CreateTarefaCommandHandler(ITarefasRepository tarefasRepository, IUsuarioRepository usuarioRepository)
    {
        _tarefaRepository = tarefasRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<TarefaResponse> Handle(CreateTarefaCommand createTarefaCommand, CancellationToken cancellationToken)
    {
        var dto = createTarefaCommand.Request;

        if (string.IsNullOrWhiteSpace(dto.Titulo))
            throw new BusinessException("O titulo é obrigatório");

        var agora = DateTime.UtcNow;

        var tarefa = new Tarefa
        {
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            Nivel = dto.Nivel,
            ProjetoId = createTarefaCommand.ProjetoId,
            ResponsavelId = dto.ResponsavelId,
            Status = dto.Status,
            Prioridade = dto.Prioridade,
            HoraInicio = dto.HoraInicio,
            HoraFim = dto.HoraFim,
            Codigo = "PENDENTE",
            CriadoEm = agora,
            AtualizadoEm = agora
        };

        await _tarefaRepository.AddAsync(tarefa);
        await _tarefaRepository.SaveChangesAsync();

        // O código só pode ser gerado depois que o Id sequencial existe.
        tarefa.Codigo = tarefa.Id.ToString("D6");
        await _tarefaRepository.SaveChangesAsync();

        var responsavelNome = tarefa.ResponsavelId.HasValue
            ? (await _usuarioRepository.GetByIdAsync(tarefa.ResponsavelId.Value))?.Nome
            : null;

        return tarefa.ToResponse(responsavelNome);
    }
}
