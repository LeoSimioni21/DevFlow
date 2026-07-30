import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { Shell } from './layout/shell/shell';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then((m) => m.Login),
  },
  {
    path: '',
    component: Shell,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'projects', pathMatch: 'full' },
      {
        path: 'projects',
        loadComponent: () => import('./features/dashboard/dashboard').then((m) => m.Dashboard),
      },
      {
        path: 'projects/:id',
        loadComponent: () => import('./features/project-detail/project-detail').then((m) => m.ProjectDetail),
      },
      {
        path: 'projetos',
        loadComponent: () => import('./features/projects-table/projects-table').then((m) => m.ProjectsTable),
      },
      {
        path: 'tarefas',
        loadComponent: () => import('./features/my-tasks/my-tasks').then((m) => m.MyTasks),
      },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard-analytics/dashboard-analytics').then((m) => m.DashboardAnalytics),
      },
      {
        path: 'configuracoes',
        loadComponent: () => import('./features/settings/settings').then((m) => m.Settings),
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
