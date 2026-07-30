namespace DevFlow.Application.Dashboard.DTOs;

public record DashboardPercentualItem(string Chave, int Quantidade, double Percentual);

public record PontoCriticoResponse(
    string Tipo,
    string Descricao,
    int? TarefaId,
    string? TarefaCodigo,
    int? ProjetoId,
    string? ProjetoNome
);

public record DesempenhoFuncionarioResponse(
    int UsuarioId,
    string Nome,
    int Demanda,
    int Entrega,
    double CapacidadeHoras,
    double EficaciaPercentual
);

public record DashboardResponse(
    int TotalTarefas,
    double TotalHorasTrabalhadas,
    double MediaHorasPorTarefa,
    List<DashboardPercentualItem> PorStatus,
    List<DashboardPercentualItem> PorPrioridade,
    List<PontoCriticoResponse> PontosCriticos,
    List<DesempenhoFuncionarioResponse> Desempenho
);
