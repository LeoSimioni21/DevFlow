using MediatR;
using DevFlow.Application.Tarefas.DTOs.Tarefas;
using DevFlow.Application.Interfaces;
using DevFlow.Application.Interfaces.Usuarios;

namespace DevFlow.Application.Tarefas.Commands.Tarefas;

public class UpdateTarefaCommandHandler : IRequestHandler<UpdateTarefaCommand, TarefaResponse?>
{
    private readonly ITarefasRepository _tarefaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public UpdateTarefaCommandHandler(ITarefasRepository tarefasRepository, IUsuarioRepository usuarioRepository)
    {
        _tarefaRepository = tarefasRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<TarefaResponse?> Handle(UpdateTarefaCommand updateTarefaCommand, CancellationToken cancellationToken)
    {
        var tarefa = await _tarefaRepository.GetTarefaByIdAsync(updateTarefaCommand.Id);
        if (tarefa is null)
            return null;

        var dto = updateTarefaCommand.Request;

        if (string.IsNullOrWhiteSpace(dto.Titulo))
            throw new BusinessException("O titulo é obrigatório");

        tarefa.Titulo = dto.Titulo;
        tarefa.Descricao = dto.Descricao;
        tarefa.Nivel = dto.Nivel;
        tarefa.Status = dto.Status;
        tarefa.Prioridade = dto.Prioridade;
        tarefa.HoraInicio = dto.HoraInicio;
        tarefa.HoraFim = dto.HoraFim;
        tarefa.ResponsavelId = dto.ResponsavelId;
        tarefa.AtualizadoEm = DateTime.UtcNow;

        await _tarefaRepository.SaveChangesAsync();

        var responsavelNome = tarefa.ResponsavelId.HasValue
            ? (await _usuarioRepository.GetByIdAsync(tarefa.ResponsavelId.Value))?.Nome
            : null;

        return tarefa.ToResponse(responsavelNome);
    }
}
