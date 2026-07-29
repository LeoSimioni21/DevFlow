namespace DevFlow.Application.Usuarios.DTOs.Usuarios;

public record CreateUserRequest
(
    string Nome,
    string Email,
    string Senha
);
