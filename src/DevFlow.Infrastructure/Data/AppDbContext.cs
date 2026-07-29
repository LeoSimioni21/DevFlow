using Microsoft.EntityFrameworkCore;

namespace DevFlow.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Tabelas mapeadas usando os tipos absolutos do seu Domínio
    public DbSet<DevFlow.Domain.Usuarios.Usuario> Usuarios { get; set; }
    public DbSet<DevFlow.Domain.Projetos.Projeto> Projetos { get; set; }
    public DbSet<DevFlow.Domain.Tarefas.Tarefa> Tarefas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==========================================
        // 1. CONFIGURAÇÃO DA TABELA DE USUÁRIOS
        // ==========================================
        modelBuilder.Entity<DevFlow.Domain.Usuarios.Usuario>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Nome)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(u => u.Email)
                .HasMaxLength(200)
                .IsRequired();

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.CriadoEm)
                .IsRequired();
        });

        // ==========================================
        // 2. CONFIGURAÇÃO DA TABELA DE PROJETOS
        // ==========================================
        modelBuilder.Entity<DevFlow.Domain.Projetos.Projeto>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Nome)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(p => p.Descricao)
                .HasMaxLength(500);

            entity.Property(p => p.Icone)
                .HasMaxLength(30);

            entity.Property(p => p.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .HasDefaultValue(DevFlow.Domain.Enums.StatusProjeto.EmDia)
                .IsRequired();

            entity.Property(p => p.CriadoEm)
                .IsRequired();

            // Relacionamento: Projeto -> Responsável (Usuario)
            entity.HasOne(p => p.Responsavel)
                .WithMany(u => u.ProjetosResponsavel)
                .HasForeignKey(p => p.ResponsavelId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ==========================================
        // 3. CONFIGURAÇÃO DA TABELA DE TAREFAS
        // ==========================================
        modelBuilder.Entity<DevFlow.Domain.Tarefas.Tarefa>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Titulo)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(t => t.Descricao)
                .HasMaxLength(2000);

            entity.Property(t => t.Nivel)
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(DevFlow.Domain.Enums.NivelTarefa.Media)
                .IsRequired();

            entity.Property(t => t.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .HasDefaultValue(DevFlow.Domain.Enums.StatusTarefa.AFazer)
                .IsRequired();

            // Relacionamento: Tarefa -> Projeto
            entity.HasOne(t => t.Projeto)
                .WithMany(p => p.Tarefas)
                .HasForeignKey(t => t.ProjetoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento: Tarefa -> Responsável (Usuario)
            entity.HasOne(t => t.Responsavel)
                .WithMany(u => u.TarefasResponsavel)
                .HasForeignKey(t => t.ResponsavelId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        });
    }
}