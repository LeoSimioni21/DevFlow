namespace DevFlow.Application.Usuarios.DTOs.Usuarios;

public record LoginRequest
(
    string Email,
    string Senha
);
