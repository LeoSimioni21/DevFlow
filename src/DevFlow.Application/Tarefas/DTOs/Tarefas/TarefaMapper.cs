namespace DevFlow.Application.Tarefas.DTOs.Tarefas;

public static class TarefaMapper
{
    public static TarefaResponse ToResponse(this DevFlow.Domain.Tarefas.Tarefa tarefa, string? responsavelNome)
    {
        double? horasTrabalhadas = tarefa.HoraInicio.HasValue && tarefa.HoraFim.HasValue
            ? (tarefa.HoraFim.Value - tarefa.HoraInicio.Value).TotalHours
            : null;

        return new TarefaResponse(
            tarefa.Id,
            tarefa.Codigo,
            tarefa.Titulo,
            tarefa.Descricao,
            tarefa.Nivel.ToString(),
            tarefa.Status.ToString(),
            tarefa.Prioridade.ToString(),
            tarefa.HoraInicio,
            tarefa.HoraFim,
            horasTrabalhadas,
            tarefa.ProjetoId,
            tarefa.ResponsavelId,
            responsavelNome,
            tarefa.CriadoEm,
            tarefa.AtualizadoEm
        );
    }
}
