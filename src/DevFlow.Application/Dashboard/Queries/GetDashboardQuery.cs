using DevFlow.Application.Dashboard.DTOs;
using MediatR;

namespace DevFlow.Application.Dashboard.Queries;

public record GetDashboardQuery : IRequest<DashboardResponse>;
