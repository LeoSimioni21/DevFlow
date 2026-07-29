// Projeto.cs
namespace DevFlow.Domain.Projetos;

public class Projeto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? Icone { get; set; }
    public DateTime? PrazoEm { get; set; }
    public DevFlow.Domain.Enums.StatusProjeto Status { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public int ResponsavelId { get; set; }
    public DevFlow.Domain.Usuarios.Usuario Responsavel { get; set; } = null!;

    public ICollection<DevFlow.Domain.Tarefas.Tarefa> Tarefas { get; set; } = new List<DevFlow.Domain.Tarefas.Tarefa>();
}