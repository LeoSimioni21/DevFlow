using DevFlow.Domain.Projetos;
using DevFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DevFlow.Application.Interfaces;

namespace DevFlow.Infrastructure.Repositories.Projetos;

public class ProjetoRepository : IProjetosRepository
{
    private readonly AppDbContext _context;

    public ProjetoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Projeto>> GetProjetosAsync()
        => await _context.Projetos.ToListAsync();

    public async Task<Projeto?> GetProjetoByIdAsync(int id)
        => await _context.Projetos.FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Projeto projeto)
    {
        await _context.Projetos.AddAsync(projeto);
    }

    public async Task DeleteByAsync(int id)
    {
        var projeto = await GetProjetoByIdAsync(id);
        if (projeto != null)
        {
            _context.Projetos.Remove(projeto);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
