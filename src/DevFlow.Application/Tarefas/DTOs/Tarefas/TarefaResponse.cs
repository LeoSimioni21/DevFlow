namespace DevFlow.Application.Tarefas.DTOs.Tarefas;

public record TarefaResponse
(
    int Id,
    string Codigo,
    string Titulo,
    string? Descricao,
    string Nivel,
    string Status,
    string Prioridade,
    DateTime? HoraInicio,
    DateTime? HoraFim,
    double? HorasTrabalhadas,
    int ProjetoId,
    int? ResponsavelId,
    string? ResponsavelNome,
    DateTime CriadoEm,
    DateTime AtualizadoEm
);
