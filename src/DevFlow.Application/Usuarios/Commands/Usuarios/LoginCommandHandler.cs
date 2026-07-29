using MediatR;
using DevFlow.Application.Usuarios.DTOs.Usuarios;
using DevFlow.Application.Interfaces.Usuarios;
using DevFlow.Application.Security;

namespace DevFlow.Application.Usuarios.Commands.Usuarios;

public class LoginCommandHandler : IRequestHandler<LoginCommand, UsuarioResponse?>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public LoginCommandHandler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<UsuarioResponse?> Handle(LoginCommand loginCommand, CancellationToken cancellationToken)
    {
        var dto = loginCommand.Request;

        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
            return null;

        var usuario = await _usuarioRepository.GetByEmailAsync(dto.Email);
        if (usuario is null || string.IsNullOrEmpty(usuario.SenhaHash))
            return null;

        if (!PasswordHasher.Verify(dto.Senha, usuario.SenhaHash))
            return null;

        return new UsuarioResponse(usuario.Id, usuario.Nome, usuario.Email, usuario.CriadoEm);
    }
}
