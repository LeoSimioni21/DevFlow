// Tarefa.cs
namespace DevFlow.Domain.Tarefas;

public class Tarefa
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DevFlow.Domain.Enums.NivelTarefa Nivel { get; set; }
    public DevFlow.Domain.Enums.StatusTarefa Status { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;

    public int ProjetoId { get; set; }
    public DevFlow.Domain.Projetos.Projeto Projeto { get; set; } = null!;

    public int? ResponsavelId { get; set; }
    public DevFlow.Domain.Usuarios.Usuario? Responsavel { get; set; }
}