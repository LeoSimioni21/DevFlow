using DevFlow.Application.Usuarios.DTOs.Usuarios;
using MediatR;

namespace DevFlow.Application.Usuarios.Commands.Usuarios;

public record CreateUserCommand(CreateUserRequest Request) : IRequest<UsuarioResponse>;
