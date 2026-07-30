using DevFlow.Application.Dashboard.DTOs;
using MediatR;

namespace DevFlow.Application.Dashboard.Queries;

public record GetDashboardQuery(DateTime? DataInicio, DateTime? DataFim) : IRequest<DashboardResponse>;
