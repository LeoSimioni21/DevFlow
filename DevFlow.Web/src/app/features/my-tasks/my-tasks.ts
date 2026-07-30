import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ProjectsStore } from '../../core/data/projects.store';
import { TarefasService } from '../../core/data/tarefas.service';
import { BreadcrumbService } from '../../core/layout/breadcrumb.service';
import { Task, TaskStatus } from '../../shared/models/task.model';

interface StatusPresentation {
  label: string;
  className: string;
}

interface TaskColumn {
  status: TaskStatus;
  label: string;
  className: string;
  tasks: Task[];
  pageTasks: Task[];
  page: number;
  totalPages: number;
}

const TASK_STATUS_PRESENTATION: Record<TaskStatus, StatusPresentation> = {
  analise: { label: 'Análise', className: 'neutral' },
  'em-desenvolvimento': { label: 'Em desenvolvimento', className: 'warning' },
  concluido: { label: 'Concluído', className: 'success' },
};

const TASK_COLUMN_ORDER: TaskStatus[] = ['analise', 'em-desenvolvimento', 'concluido'];
const PAGE_SIZE = 10;

@Component({
  selector: 'app-my-tasks',
  imports: [RouterLink, FormsModule],
  templateUrl: './my-tasks.html',
  styleUrl: './my-tasks.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MyTasks {
  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly tarefasService = inject(TarefasService);
  private readonly projectsStore = inject(ProjectsStore);

  protected readonly isLoading = signal(true);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly tasks = signal<Task[]>([]);
  protected readonly searchCode = signal('');

  protected readonly projectNameById = computed(() => {
    const map = new Map<string, string>();
    for (const project of this.projectsStore.projects()) {
      map.set(project.id, project.name);
    }
    return map;
  });

  protected readonly filteredTasks = computed(() => {
    const term = this.searchCode().trim().toLowerCase();
    const tasks = this.tasks();
    return term ? tasks.filter((task) => task.code.toLowerCase().includes(term)) : tasks;
  });

  private readonly pageByStatus = signal<Record<TaskStatus, number>>({
    analise: 0,
    'em-desenvolvimento': 0,
    concluido: 0,
  });

  protected readonly taskColumns = computed<TaskColumn[]>(() => {
    const tasks = this.filteredTasks();
    const pages = this.pageByStatus();

    return TASK_COLUMN_ORDER.map((status) => {
      const columnTasks = tasks.filter((task) => task.status === status);
      const totalPages = Math.max(1, Math.ceil(columnTasks.length / PAGE_SIZE));
      const page = Math.min(pages[status], totalPages - 1);

      return {
        status,
        label: TASK_STATUS_PRESENTATION[status].label,
        className: TASK_STATUS_PRESENTATION[status].className,
        tasks: columnTasks,
        pageTasks: columnTasks.slice(page * PAGE_SIZE, page * PAGE_SIZE + PAGE_SIZE),
        page,
        totalPages,
      };
    });
  });

  constructor() {
    this.breadcrumbService.set([{ label: 'Início', link: '/projects' }, { label: 'Minhas tarefas' }]);

    this.projectsStore.load().subscribe();

    this.tarefasService.listAll().subscribe({
      next: (tasks) => {
        this.tasks.set(tasks);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('Não foi possível carregar as tarefas.');
      },
    });
  }

  protected projectName(projectId: string): string {
    return this.projectNameById().get(projectId) ?? 'Projeto removido';
  }

  protected changePage(status: TaskStatus, delta: number): void {
    this.pageByStatus.update((pages) => ({ ...pages, [status]: Math.max(0, pages[status] + delta) }));
  }
}
