namespace DevFlow.Application.Tarefas.DTOs.Tarefas;

public record UpdateTarefaRequest
(
    string Titulo,
    string? Descricao,
    DevFlow.Domain.Enums.NivelTarefa Nivel,
    DevFlow.Domain.Enums.StatusTarefa Status,
    int? ResponsavelId
);
