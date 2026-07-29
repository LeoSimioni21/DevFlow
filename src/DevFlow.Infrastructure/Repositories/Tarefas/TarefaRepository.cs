using DevFlow.Domain.Tarefas;
using DevFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DevFlow.Application.Interfaces;

namespace DevFlow.Infrastructure.Repositories.Tarefas;

public class TarefaRepository : ITarefasRepository
{
    private readonly AppDbContext _context;

    public TarefaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tarefa>> GetTarefasAsync()
        => await _context.Tarefas.ToListAsync();

    public async Task<List<Tarefa>> GetTarefasByProjetoIdAsync(int projetoId)
        => await _context.Tarefas
            .Where(t => t.ProjetoId == projetoId)
            .ToListAsync();

    public async Task<Tarefa?> GetTarefaByIdAsync(int id)
        => await _context.Tarefas.FirstOrDefaultAsync(t => t.Id == id);

    public async Task AddAsync(Tarefa tarefa)
    {
        await _context.Tarefas.AddAsync(tarefa);
    }

    public async Task DeleteByAsync(int id)
    {
        var tarefa = await GetTarefaByIdAsync(id);
        if (tarefa != null)
        {
            _context.Tarefas.Remove(tarefa);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
