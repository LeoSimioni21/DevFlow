using DevFlow.Application.Projetos.DTOs.Projetos;
using MediatR;

namespace DevFlow.Application.Projetos.Queries.Projetos;

public record GetProjetoByIdQuery(int Id) : IRequest<ProjetoResponse?>;
