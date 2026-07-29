namespace DevFlow.Application.Usuarios.DTOs.Usuarios;

public record UsuarioResponse
(
    int Id,
    string? Nome,
    string? Email,
    DateTime CriadoEm
);