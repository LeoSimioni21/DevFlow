namespace DevFlow.Application.Interfaces;

public interface IProjetosRepository
{
    Task<List<DevFlow.Domain.Projetos.Projeto>> GetProjetosAsync();
    Task<DevFlow.Domain.Projetos.Projeto?> GetProjetoByIdAsync(int id);
    Task AddAsync(DevFlow.Domain.Projetos.Projeto projeto);
    Task DeleteByAsync(int id);
    Task SaveChangesAsync();
}
