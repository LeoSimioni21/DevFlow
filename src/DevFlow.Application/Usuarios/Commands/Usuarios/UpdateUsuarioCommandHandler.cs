using MediatR;
using DevFlow.Application.Usuarios.DTOs.Usuarios;
using DevFlow.Application.Interfaces.Usuarios;

namespace DevFlow.Application.Usuarios.Commands.Usuarios;

public class UpdateUsuarioCommandHandler : IRequestHandler<UpdateUsuarioCommand, UsuarioResponse?>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UpdateUsuarioCommandHandler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<UsuarioResponse?> Handle(UpdateUsuarioCommand updateUsuarioCommand, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(updateUsuarioCommand.Id);
        if (usuario is null)
            return null;

        var dto = updateUsuarioCommand.Request;

        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new BusinessException("O nome é obrigatório");

        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new BusinessException("O email é obrigatório");

        var existente = await _usuarioRepository.GetByEmailAsync(dto.Email);
        if (existente is not null && existente.Id != usuario.Id)
            throw new BusinessException("Já existe um usuário com este email");

        usuario.Nome = dto.Nome;
        usuario.Email = dto.Email;

        await _usuarioRepository.UpdateAsync(usuario);

        return new UsuarioResponse(usuario.Id, usuario.Nome, usuario.Email, usuario.CriadoEm);
    }
}
