namespace DevFlow.Application.Projetos.DTOs.Projetos;

public record ProjetoResponse
(
    int Id,
    string Nome,
    string? Descricao,
    string? Icone,
    DateTime? PrazoEm,
    string Status,
    DateTime CriadoEm,
    int ResponsavelId,
    int TotalTarefas,
    double Progresso
);
