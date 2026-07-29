namespace DevFlow.Application.Tarefas.DTOs.Tarefas;

public record TarefaResponse
(
    int Id,
    string Titulo,
    string? Descricao,
    string Nivel,
    string Status,
    int ProjetoId,
    int? ResponsavelId,
    string? ResponsavelNome,
    DateTime CriadoEm,
    DateTime AtualizadoEm
);
