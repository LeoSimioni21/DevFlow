using MediatR;
using DevFlow.Application.Projetos.DTOs.Projetos;
using DevFlow.Application.Interfaces;

namespace DevFlow.Application.Projetos.Queries.Projetos;

public class GetProjetoByIdQueryHandler : IRequestHandler<GetProjetoByIdQuery, ProjetoResponse?>
{
    private readonly IProjetosRepository _projetoRepository;
    private readonly ITarefasRepository _tarefaRepository;

    public GetProjetoByIdQueryHandler(IProjetosRepository projetosRepository, ITarefasRepository tarefasRepository)
    {
        _projetoRepository = projetosRepository;
        _tarefaRepository = tarefasRepository;
    }

    public async Task<ProjetoResponse?> Handle(GetProjetoByIdQuery getProjetoByIdQuery, CancellationToken cancellationToken)
    {
        var projeto = await _projetoRepository.GetProjetoByIdAsync(getProjetoByIdQuery.Id);
        if (projeto is null)
            return null;

        var tarefas = await _tarefaRepository.GetTarefasByProjetoIdAsync(getProjetoByIdQuery.Id);

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
