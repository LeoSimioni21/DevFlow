import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ProjectsStore } from '../../core/data/projects.store';
import { BreadcrumbService } from '../../core/layout/breadcrumb.service';
import { ProjectStatus } from '../../shared/models/project.model';

interface StatusPresentation {
  label: string;
  className: string;
}

const STATUS_PRESENTATION: Record<ProjectStatus, StatusPresentation> = {
  'in-progress': { label: 'Em andamento', className: 'success' },
  attention: { label: 'Atenção', className: 'warning' },
  critical: { label: 'Crítico', className: 'danger' },
};

@Component({
  selector: 'app-projects-table',
  imports: [RouterLink],
  templateUrl: './projects-table.html',
  styleUrl: './projects-table.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectsTable {
  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly projectsStore = inject(ProjectsStore);

  protected readonly projects = this.projectsStore.projects;
  protected readonly isLoading = this.projectsStore.isLoading;

  constructor() {
    this.breadcrumbService.set([{ label: 'Início', link: '/projects' }, { label: 'Projetos' }]);
    this.projectsStore.load().subscribe();
  }

  protected status(status: ProjectStatus): StatusPresentation {
    return STATUS_PRESENTATION[status];
  }
}
