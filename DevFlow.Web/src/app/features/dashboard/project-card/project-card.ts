import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Project, ProjectStatus } from '../../../shared/models/project.model';
import { ProjectIcon } from '../project-icon/project-icon';

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
  selector: 'app-project-card',
  imports: [ProjectIcon, RouterLink],
  templateUrl: './project-card.html',
  styleUrl: './project-card.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectCard {
  readonly project = input.required<Project>();
  readonly deleteRequested = output<Project>();

  protected readonly status = computed(() => STATUS_PRESENTATION[this.project().status]);

  protected onDeleteClick(event: MouseEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.deleteRequested.emit(this.project());
  }
}
