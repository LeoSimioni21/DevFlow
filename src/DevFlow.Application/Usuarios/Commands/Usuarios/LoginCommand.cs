using DevFlow.Application.Usuarios.DTOs.Usuarios;
using MediatR;

namespace DevFlow.Application.Usuarios.Commands.Usuarios;

public record LoginCommand(LoginRequest Request) : IRequest<UsuarioResponse?>;
