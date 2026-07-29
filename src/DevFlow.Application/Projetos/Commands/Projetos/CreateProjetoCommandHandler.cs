using MediatR;
using DevFlow.Application.Projetos.DTOs.Projetos;
using DevFlow.Application.Interfaces;
using DevFlow.Domain.Projetos;

namespace DevFlow.Application.Projetos.Commands.Projetos;

public class CreateProjetoCommandHandler : IRequestHandler<CreateProjetoCommand, ProjetoResponse>
{
    private readonly IProjetosRepository _projetoRepository;

    public CreateProjetoCommandHandler(IProjetosRepository projetosRepository)
    {
        _projetoRepository = projetosRepository;
    }

    public async Task<ProjetoResponse> Handle(CreateProjetoCommand createProjetoCommand, CancellationToken cancellationToken)
    {
        var dto = createProjetoCommand.Request;

        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new BusinessException("O nome é obrigatório");

        var projeto = new Projeto
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            Icone = dto.Icone,
            PrazoEm = dto.PrazoEm,
            Status = DevFlow.Domain.Enums.StatusProjeto.EmDia,
            CriadoEm = DateTime.UtcNow,
            ResponsavelId = dto.ResponsavelId
        };

        await _projetoRepository.AddAsync(projeto);
        await _projetoRepository.SaveChangesAsync();

        return new ProjetoResponse(
            projeto.Id,
            projeto.Nome,
            projeto.Descricao,
            projeto.Icone,
            projeto.PrazoEm,
            projeto.Status.ToString(),
            projeto.CriadoEm,
            projeto.ResponsavelId,
            TotalTarefas: 0,
            Progresso: 0
        );
    }
}
