namespace DevFlow.Application.Interfaces;

public interface ITarefasRepository
{
    Task<List<DevFlow.Domain.Tarefas.Tarefa>> GetTarefasAsync();
    Task<List<DevFlow.Domain.Tarefas.Tarefa>> GetTarefasByProjetoIdAsync(int projetoId);
    Task<DevFlow.Domain.Tarefas.Tarefa?> GetTarefaByIdAsync(int id);
    Task AddAsync(DevFlow.Domain.Tarefas.Tarefa tarefa);
    Task DeleteByAsync(int id);
    Task SaveChangesAsync();
}