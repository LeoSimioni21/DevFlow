using DevFlow.Application.Usuarios.DTOs.Usuarios;
using MediatR;

namespace DevFlow.Application.Usuarios.Commands.Usuarios;

public record UpdateUsuarioCommand(int Id, UpdateUsuarioRequest Request) : IRequest<UsuarioResponse?>;
