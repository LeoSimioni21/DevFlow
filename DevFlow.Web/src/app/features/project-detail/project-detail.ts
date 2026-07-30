import { ChangeDetectionStrategy, Component, computed, effect, inject, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { ProjetosService } from '../../core/data/projetos.service';
import { PRIORITY_TO_API, STATUS_TO_API } from '../../core/data/tarefas.service';
import { TasksStore } from '../../core/data/tasks.store';
import { BreadcrumbService } from '../../core/layout/breadcrumb.service';
import { Project, ProjectStatus } from '../../shared/models/project.model';
import { Task, TaskPriority, TaskStatus } from '../../shared/models/task.model';
import { ProjectIcon } from '../dashboard/project-icon/project-icon';

interface StatusPresentation {
  label: string;
  className: string;
}

const TASK_PRIORITY_PRESENTATION: Record<TaskPriority, StatusPresentation> = {
  baixa: { label: 'Baixa', className: 'neutral' },
  media: { label: 'Média', className: 'warning' },
  alta: { label: 'Alta', className: 'danger' },
};

const TASK_PRIORITY_ORDER: TaskPriority[] = ['baixa', 'media', 'alta'];

function toDateTimeLocal(value: string | null): string {
  if (!value) {
    return '';
  }
  const date = new Date(value);
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
}

function fromDateTimeLocal(value: string): string | null {
  return value ? new Date(value).toISOString() : null;
}

interface TaskColumn {
  status: TaskStatus;
  label: string;
  className: string;
  tasks: Task[];
}

const PROJECT_STATUS_PRESENTATION: Record<ProjectStatus, StatusPresentation> = {
  'in-progress': { label: 'Em andamento', className: 'success' },
  attention: { label: 'Atenção', className: 'warning' },
  critical: { label: 'Crítico', className: 'danger' },
};

const TASK_STATUS_PRESENTATION: Record<TaskStatus, StatusPresentation> = {
  analise: { label: 'Análise', className: 'neutral' },
  'em-desenvolvimento': { label: 'Em desenvolvimento', className: 'warning' },
  concluido: { label: 'Concluído', className: 'success' },
};

const TASK_COLUMN_ORDER: TaskStatus[] = ['analise', 'em-desenvolvimento', 'concluido'];

const DATE_TIME_FORMATTER = new Intl.DateTimeFormat('pt-BR', {
  day: '2-digit',
  month: '2-digit',
  year: 'numeric',
  hour: '2-digit',
  minute: '2-digit',
});

@Component({
  selector: 'app-project-detail',
  imports: [RouterLink, ProjectIcon, ReactiveFormsModule, DecimalPipe],
  templateUrl: './project-detail.html',
  styleUrl: './project-detail.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectDetail {
  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly projetosService = inject(ProjetosService);
  private readonly tasksStore = inject(TasksStore);

  readonly id = input.required<string>();

  protected readonly project = signal<Project | undefined>(undefined);
  protected readonly isLoading = signal(true);
  protected readonly errorMessage = signal<string | null>(null);

  protected readonly status = computed(() => {
    const project = this.project();
    return project ? PROJECT_STATUS_PRESENTATION[project.status] : undefined;
  });

  // ---------- Tarefas ----------

  protected readonly tasks = computed(() => this.tasksStore.getTasks(this.id()));

  protected readonly taskColumns = computed<TaskColumn[]>(() => {
    const tasks = this.tasks();
    return TASK_COLUMN_ORDER.map((status) => ({
      status,
      label: TASK_STATUS_PRESENTATION[status].label,
      className: TASK_STATUS_PRESENTATION[status].className,
      tasks: tasks.filter((task) => task.status === status),
    }));
  });

  protected readonly taskStatusOptions = TASK_COLUMN_ORDER.map((status) => ({
    value: status,
    label: TASK_STATUS_PRESENTATION[status].label,
  }));

  protected readonly taskPriorityOptions = TASK_PRIORITY_ORDER.map((priority) => ({
    value: priority,
    label: TASK_PRIORITY_PRESENTATION[priority].label,
  }));

  protected readonly selectedTask = signal<Task | null>(null);

  protected readonly selectedTaskStatus = computed(() => {
    const task = this.selectedTask();
    return task ? TASK_STATUS_PRESENTATION[task.status] : undefined;
  });

  protected readonly priorityPresentation = TASK_PRIORITY_PRESENTATION;

  protected readonly taskForm = this.formBuilder.nonNullable.group({
    title: ['', Validators.required],
    description: ['', Validators.required],
    status: ['analise' as TaskStatus, Validators.required],
    priority: ['media' as TaskPriority, Validators.required],
    startedAt: [''],
    finishedAt: [''],
  });

  protected readonly isCreateTaskOpen = signal(false);

  protected readonly createTaskForm = this.formBuilder.nonNullable.group({
    title: ['', Validators.required],
    description: ['', Validators.required],
    status: ['analise' as TaskStatus, Validators.required],
    priority: ['media' as TaskPriority, Validators.required],
    startedAt: [''],
    finishedAt: [''],
  });

  constructor() {
    effect(() => {
      const projectId = this.id();
      this.isLoading.set(true);
      this.errorMessage.set(null);

      this.projetosService.getById(projectId).subscribe({
        next: (project) => {
          this.project.set(project);
          this.isLoading.set(false);
          this.breadcrumbService.set([
            { label: 'Início', link: '/projects' },
            { label: 'Projetos', link: '/projects' },
            { label: project.name },
          ]);
        },
        error: () => {
          this.project.set(undefined);
          this.isLoading.set(false);
          this.errorMessage.set('Projeto não encontrado.');
          this.breadcrumbService.set([{ label: 'Início', link: '/projects' }, { label: 'Projetos' }]);
        },
      });

      this.tasksStore.loadForProject(projectId).subscribe();
    });
  }

  // ---------- Tarefas: detalhe/edição ----------

  protected openTask(task: Task): void {
    this.selectedTask.set(task);
    this.taskForm.reset({
      title: task.title,
      description: task.description,
      status: task.status,
      priority: task.priority,
      startedAt: toDateTimeLocal(task.startedAt),
      finishedAt: toDateTimeLocal(task.finishedAt),
    });
  }

  protected closeTask(): void {
    this.selectedTask.set(null);
  }

  protected submitTaskForm(): void {
    const task = this.selectedTask();
    if (!task || this.taskForm.invalid) {
      this.taskForm.markAllAsTouched();
      return;
    }

    const { title, description, status, priority, startedAt, finishedAt } = this.taskForm.getRawValue();

    this.tasksStore
      .updateTask(this.id(), task.id, {
        titulo: title,
        descricao: description || null,
        nivel: 1,
        status: STATUS_TO_API[status],
        prioridade: PRIORITY_TO_API[priority],
        horaInicio: fromDateTimeLocal(startedAt),
        horaFim: fromDateTimeLocal(finishedAt),
        responsavelId: this.authService.currentUser()?.id ?? null,
      })
      .subscribe({
        next: () => this.selectedTask.set(null),
        error: (err: HttpErrorResponse) => {
          this.errorMessage.set(`Falha ao salvar a tarefa (${err.status}).`);
        },
      });
  }

  protected deleteSelectedTask(): void {
    const task = this.selectedTask();
    if (!task || !confirm(`Excluir a tarefa "${task.title}"?`)) {
      return;
    }

    this.tasksStore.deleteTask(this.id(), task.id).subscribe({
      next: () => this.selectedTask.set(null),
      error: (err: HttpErrorResponse) => {
        this.errorMessage.set(`Falha ao excluir a tarefa (${err.status}).`);
      },
    });
  }

  // ---------- Tarefas: criação ----------

  protected openCreateTaskForm(): void {
    this.createTaskForm.reset({
      title: '',
      description: '',
      status: 'analise',
      priority: 'media',
      startedAt: '',
      finishedAt: '',
    });
    this.isCreateTaskOpen.set(true);
  }

  protected closeCreateTaskForm(): void {
    this.isCreateTaskOpen.set(false);
  }

  protected submitCreateTaskForm(): void {
    if (this.createTaskForm.invalid) {
      this.createTaskForm.markAllAsTouched();
      return;
    }

    const currentUser = this.authService.currentUser();
    const { title, description, status, priority, startedAt, finishedAt } = this.createTaskForm.getRawValue();

    this.tasksStore
      .addTask(this.id(), {
        titulo: title,
        descricao: description || null,
        nivel: 1,
        status: STATUS_TO_API[status],
        prioridade: PRIORITY_TO_API[priority],
        horaInicio: fromDateTimeLocal(startedAt),
        horaFim: fromDateTimeLocal(finishedAt),
        responsavelId: currentUser?.id ?? null,
      })
      .subscribe({
        next: () => this.isCreateTaskOpen.set(false),
        error: (err: HttpErrorResponse) => {
          this.errorMessage.set(`Falha ao criar a tarefa (${err.status}).`);
        },
      });
  }

  // ---------- Utilitários ----------

  protected formatDateTime(value: string): string {
    return DATE_TIME_FORMATTER.format(new Date(value));
  }

  protected formatRelative(value: string): string {
    const diffDays = Math.floor((Date.now() - new Date(value).getTime()) / (1000 * 60 * 60 * 24));

    if (diffDays <= 0) {
      return 'hoje';
    }
    if (diffDays === 1) {
      return 'há 1 dia';
    }
    return `há ${diffDays} dias`;
  }
}
