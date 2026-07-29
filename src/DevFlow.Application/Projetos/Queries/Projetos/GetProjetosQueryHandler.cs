using MediatR;
using DevFlow.Application.Projetos.DTOs.Projetos;
using DevFlow.Application.Interfaces;
using DevFlow.Domain.Tarefas;

namespace DevFlow.Application.Projetos.Queries.Projetos;

public class GetProjetosQueryHandler : IRequestHandler<GetProjetosQuery, List<ProjetoResponse>>
{
    private readonly IProjetosRepository _projetoRepository;
    private readonly ITarefasRepository _tarefaRepository;

    public GetProjetosQueryHandler(IProjetosRepository projetosRepository, ITarefasRepository tarefasRepository)
    {
        _projetoRepository = projetosRepository;
        _tarefaRepository = tarefasRepository;
    }

    public async Task<List<ProjetoResponse>> Handle(GetProjetosQuery getProjetosQuery, CancellationToken cancellationToken)
    {
        var projetos = await _projetoRepository.GetProjetosAsync();
        var tarefas = await _tarefaRepository.GetTarefasAsync();

        var tarefasPorProjeto = tarefas.GroupBy(t => t.ProjetoId).ToDictionary(g => g.Key, g => g.ToList());

        return projetos
            .Select(projeto => MapToResponse(projeto, tarefasPorProjeto.GetValueOrDefault(projeto.Id, new List<Tarefa>())))
            .ToList();
    }

    private static ProjetoResponse MapToResponse(DevFlow.Domain.Projetos.Projeto projeto, List<Tarefa> tarefas)
    {
        var totalTarefas = tarefas.Count;
        var concluidas = tarefas.Count(t => t.Status == DevFlow.Domain.Enums.StatusTarefa.Concluida);
        var progresso = totalTarefas == 0 ? 0 : Math.Round((double)concluidas / totalTarefas * 100, 0);

        return new ProjetoResponse(
            projeto.Id,
            projeto.Nome,
            projeto.Descricao,
            projeto.Icone,
            projeto.PrazoEm,
            projeto.Status.ToString(),
            projeto.CriadoEm,
            projeto.ResponsavelId,
            TotalTarefas: totalTarefas,
            Progresso: progresso
        );
    }
}
