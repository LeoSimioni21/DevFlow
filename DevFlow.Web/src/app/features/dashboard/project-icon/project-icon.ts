import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { ProjectIconType } from '../../../shared/models/project.model';

@Component({
  selector: 'app-project-icon',
  templateUrl: './project-icon.html',
  styleUrl: './project-icon.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectIcon {
  readonly type = input.required<ProjectIconType>();
}
