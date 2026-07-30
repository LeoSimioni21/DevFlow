namespace DevFlow.Application.Tarefas.DTOs.Tarefas;

public record CreateTarefaRequest
(
    string Titulo,
    string? Descricao,
    DevFlow.Domain.Enums.NivelTarefa Nivel,
    DevFlow.Domain.Enums.StatusTarefa Status,
    DevFlow.Domain.Enums.PrioridadeTarefa Prioridade,
    DateTime? HoraInicio,
    DateTime? HoraFim,
    int? ResponsavelId
);
