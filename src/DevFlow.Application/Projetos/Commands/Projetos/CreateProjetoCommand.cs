using DevFlow.Application.Projetos.DTOs.Projetos;
using MediatR;

namespace DevFlow.Application.Projetos.Commands.Projetos;

public record CreateProjetoCommand(CreateProjetoRequest Request) : IRequest<ProjetoResponse>;
