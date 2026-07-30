using DevFlow.Application.Dashboard.DTOs;
using DevFlow.Application.Interfaces;
using DevFlow.Domain.Enums;
using MediatR;

namespace DevFlow.Application.Dashboard.Queries;

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardResponse>
{
    private static readonly TimeSpan LimiteSemHoraFim = TimeSpan.FromHours(24);

    private readonly ITarefasRepository _tarefaRepository;
    private readonly IProjetosRepository _projetoRepository;

    public GetDashboardQueryHandler(ITarefasRepository tarefaRepository, IProjetosRepository projetoRepository)
    {
        _tarefaRepository = tarefaRepository;
        _projetoRepository = projetoRepository;
    }

    public async Task<DashboardResponse> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var tarefas = await _tarefaRepository.GetTarefasAsync();
        var projetos = await _projetoRepository.GetProjetosAsync();
        var nomeProjetoPorId = projetos.ToDictionary(p => p.Id, p => p.Nome);

        var totalTarefas = tarefas.Count;

        var horasPorTarefa = tarefas
            .Where(t => t.HoraInicio.HasValue && t.HoraFim.HasValue)
            .Select(t => (t.HoraFim!.Value - t.HoraInicio!.Value).TotalHours)
            .ToList();

        var totalHoras = horasPorTarefa.Sum();
        var mediaHoras = horasPorTarefa.Count > 0 ? totalHoras / horasPorTarefa.Count : 0;

        var porStatus = tarefas
            .GroupBy(t => t.Status.ToString())
            .Select(g => new DashboardPercentualItem(g.Key, g.Count(), Percentual(g.Count(), totalTarefas)))
            .ToList();

        var porPrioridade = tarefas
            .GroupBy(t => t.Prioridade.ToString())
            .Select(g => new DashboardPercentualItem(g.Key, g.Count(), Percentual(g.Count(), totalTarefas)))
            .ToList();

        var pontosCriticos = new List<PontoCriticoResponse>();
        var agora = DateTime.UtcNow;

        pontosCriticos.AddRange(tarefas
            .Where(t => t.Prioridade == PrioridadeTarefa.Alta && t.Status != StatusTarefa.Concluida)
            .Select(t => new PontoCriticoResponse(
                "PrioridadeAltaEmAberto",
                "Prioridade alta ainda não concluída",
                t.Id,
                t.Codigo,
                t.ProjetoId,
                nomeProjetoPorId.GetValueOrDefault(t.ProjetoId))));

        pontosCriticos.AddRange(tarefas
            .Where(t => t.HoraInicio.HasValue && !t.HoraFim.HasValue && agora - t.HoraInicio.Value > LimiteSemHoraFim)
            .Select(t => new PontoCriticoResponse(
                "SemHoraFim",
                "Iniciada há mais de 24h e sem horário final registrado",
                t.Id,
                t.Codigo,
                t.ProjetoId,
                nomeProjetoPorId.GetValueOrDefault(t.ProjetoId))));

        pontosCriticos.AddRange(projetos
            .Where(p => p.Status == StatusProjeto.Critico)
            .Select(p => new PontoCriticoResponse(
                "ProjetoCritico",
                "Projeto com status crítico",
                null,
                null,
                p.Id,
                p.Nome)));

        return new DashboardResponse(
            totalTarefas,
            totalHoras,
            mediaHoras,
            porStatus,
            porPrioridade,
            pontosCriticos
        );
    }

    private static double Percentual(int quantidade, int total)
        => total > 0 ? quantidade * 100.0 / total : 0;
}
