using DevFlow.Domain.Usuarios;

namespace DevFlow.Application.Interfaces.Usuarios;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByIdAsync(int id);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<List<Usuario>> GetAllAsync();
    Task<Usuario> CreateAsync(Usuario usuario);
    Task UpdateAsync(Usuario usuario);
    Task DeleteAsync(Usuario usuario);
}