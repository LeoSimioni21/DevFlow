namespace DevFlow.Application.Projetos.DTOs.Projetos;

public record CreateProjetoRequest
(
    string Nome,
    string? Descricao,
    string? Icone,
    DateTime? PrazoEm,
    int ResponsavelId
);
