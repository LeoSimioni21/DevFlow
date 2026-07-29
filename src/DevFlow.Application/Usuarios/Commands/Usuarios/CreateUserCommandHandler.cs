using MediatR;
using DevFlow.Application.Usuarios.DTOs.Usuarios;
using DevFlow.Application.Interfaces.Usuarios;
using DevFlow.Application.Security;
using DevFlow.Domain.Usuarios;

namespace DevFlow.Application.Usuarios.Commands.Usuarios;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UsuarioResponse>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public CreateUserCommandHandler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<UsuarioResponse> Handle(CreateUserCommand createUserCommand, CancellationToken cancellationToken)
    {
        var dto = createUserCommand.Request;

        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new BusinessException("O nome é obrigatório");

        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new BusinessException("O email é obrigatório");

        if (string.IsNullOrWhiteSpace(dto.Senha) || dto.Senha.Length < 6)
            throw new BusinessException("A senha deve ter no mínimo 6 caracteres");

        var existente = await _usuarioRepository.GetByEmailAsync(dto.Email);
        if (existente is not null)
            throw new BusinessException("Já existe um usuário com este email");

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            SenhaHash = PasswordHasher.Hash(dto.Senha),
            CriadoEm = DateTime.UtcNow
        };

        await _usuarioRepository.CreateAsync(usuario);

        return new UsuarioResponse(usuario.Id, usuario.Nome, usuario.Email, usuario.CriadoEm);
    }
}
