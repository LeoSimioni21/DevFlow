
namespace DevFlow.Domain.Usuarios;

public class Usuario
{
    public int Id { get; set;}
    public string? Nome { get; set;}
    public string? Email { get; set;}
    public string? SenhaHash { get; set; }
    public DateTime CriadoEm  { get; set; }

    // Navegações inversas
    public ICollection<DevFlow.Domain.Projetos.Projeto> ProjetosResponsavel { get; set; } = new List<DevFlow.Domain.Projetos.Projeto>();
    public ICollection<DevFlow.Domain.Tarefas.Tarefa> TarefasResponsavel { get; set; } = new List<DevFlow.Domain.Tarefas.Tarefa>();
}