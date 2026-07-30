using DevFlow.Application.Dashboard.DTOs;
using DevFlow.Application.Interfaces;
using DevFlow.Application.Interfaces.Usuarios;
using DevFlow.Domain.Enums;
using MediatR;

namespace DevFlow.Application.Dashboard.Queries;

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardResponse>
{
    private static readonly TimeSpan LimiteSemHoraFim = TimeSpan.FromHours(24);

    private readonly ITarefasRepository _tarefaRepository;
    private readonly IProjetosRepository _projetoRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public GetDashboardQueryHandler(
        ITarefasRepository tarefaRepository,
        IProjetosRepository projetoRepository,
        IUsuarioRepository usuarioRepository)
    {
        _tarefaRepository = tarefaRepository;
        _projetoRepository = projetoRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<DashboardResponse> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var todasTarefas = await _tarefaRepository.GetTarefasAsync();
        var projetos = await _projetoRepository.GetProjetosAsync();
        var usuarios = await _usuarioRepository.GetAllAsync();
        var nomeProjetoPorId = projetos.ToDictionary(p => p.Id, p => p.Nome);
        var nomeUsuarioPorId = usuarios.ToDictionary(u => u.Id, u => u.Nome);

        // O fim do período é inclusivo até o final daquele dia.
        var inicio = request.DataInicio;
        var fimExclusivo = request.DataFim?.Date.AddDays(1);

        bool NoPeriodo(DateTime data) =>
            (!inicio.HasValue || data >= inicio.Value) &&
            (!fimExclusivo.HasValue || data < fimExclusivo.Value);

        var tarefas = todasTarefas.Where(t => NoPeriodo(t.CriadoEm)).ToList();

        var totalTarefas = tarefas.Count;

        var horasPorTarefa = todasTarefas
            .Where(t => t.HoraInicio.HasValue && t.HoraFim.HasValue && NoPeriodo(t.HoraInicio!.Value))
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

        pontosCriticos.AddRange(todasTarefas
            .Where(t => t.Prioridade == PrioridadeTarefa.Alta && t.Status != StatusTarefa.Concluida)
            .Select(t => new PontoCriticoResponse(
                "PrioridadeAltaEmAberto",
                "Prioridade alta ainda não concluída",
                t.Id,
                t.Codigo,
                t.ProjetoId,
                nomeProjetoPorId.GetValueOrDefault(t.ProjetoId))));

        pontosCriticos.AddRange(todasTarefas
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

        var desempenho = todasTarefas
            .Where(t => t.ResponsavelId.HasValue)
            .GroupBy(t => t.ResponsavelId!.Value)
            .Select(grupo =>
            {
                var demanda = grupo.Count(t => NoPeriodo(t.CriadoEm));
                var entrega = grupo.Count(t => t.Status == StatusTarefa.Concluida && NoPeriodo(t.AtualizadoEm));
                var capacidade = grupo
                    .Where(t => t.HoraInicio.HasValue && t.HoraFim.HasValue && NoPeriodo(t.HoraInicio!.Value))
                    .Sum(t => (t.HoraFim!.Value - t.HoraInicio!.Value).TotalHours);
                var eficacia = demanda > 0 ? Percentual(entrega, demanda) : (entrega > 0 ? 100 : 0);

                return new DesempenhoFuncionarioResponse(
                    grupo.Key,
                    nomeUsuarioPorId.GetValueOrDefault(grupo.Key, "Usuário removido"),
                    demanda,
                    entrega,
                    capacidade,
                    eficacia
                );
            })
            .OrderByDescending(d => d.Demanda)
            .ToList();

        return new DashboardResponse(
            totalTarefas,
            totalHoras,
            mediaHoras,
            porStatus,
            porPrioridade,
            pontosCriticos,
            desempenho
        );
    }

    private static double Percentual(int quantidade, int total)
        => total > 0 ? quantidade * 100.0 / total : 0;
}
