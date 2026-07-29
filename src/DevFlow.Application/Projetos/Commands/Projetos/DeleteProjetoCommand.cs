using MediatR;

namespace DevFlow.Application.Projetos.Commands.Projetos;

public record DeleteProjetoCommand(int Id) : IRequest<bool>;
