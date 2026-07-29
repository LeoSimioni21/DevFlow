import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../core/auth/auth.service';
import { ProjectsStore } from '../../core/data/projects.store';
import { TarefasService } from '../../core/data/tarefas.service';
import { BreadcrumbService } from '../../core/layout/breadcrumb.service';
import { Project, ProjectIconType } from '../../shared/models/project.model';
import { Task } from '../../shared/models/task.model';
import { ProjectCard } from './project-card/project-card';

interface SummaryStat {
  label: string;
  value: number;
  variant?: 'success' | 'danger';
}

const ICON_OPTIONS: { value: ProjectIconType; label: string }[] = [
  { value: 'code', label: 'Código' },
  { value: 'mobile', label: 'Mobile' },
  { value: 'devops', label: 'DevOps' },
  { value: 'api', label: 'API' },
];

@Component({
  selector: 'app-dashboard',
  imports: [ProjectCard, ReactiveFormsModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Dashboard {
  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly projectsStore = inject(ProjectsStore);
  private readonly tarefasService = inject(TarefasService);

  protected readonly projects = this.projectsStore.projects;
  protected readonly isLoading = this.projectsStore.isLoading;
  protected readonly errorMessage = signal<string | null>(null);

  private readonly allTasks = signal<Task[]>([]);

  protected readonly summary = computed<SummaryStat[]>(() => {
    const tasks = this.allTasks();
    const concluidas = tasks.filter((task) => task.status === 'concluido').length;

    return [
      { label: 'Projetos', value: this.projects().length },
      { label: 'Tarefas abertas', value: tasks.length - concluidas, variant: 'success' },
      { label: 'Concluídas', value: concluidas, variant: 'success' },
    ];
  });

  protected readonly activeProjectsCount = computed(() => this.projects().length);

  protected readonly iconOptions = ICON_OPTIONS;

  protected readonly isCreateOpen = signal(false);

  protected readonly createForm = this.formBuilder.nonNullable.group({
    name: ['', Validators.required],
    description: ['', Validators.required],
    icon: ['code' as ProjectIconType, Validators.required],
    deadline: [''],
  });

  constructor() {
    this.breadcrumbService.set([{ label: 'Início', link: '/projects' }, { label: 'Projetos' }]);
    this.loadProjects();

    this.tarefasService.listAll().subscribe({
      next: (tasks) => this.allTasks.set(tasks),
      error: () => this.allTasks.set([]),
    });
  }

  protected loadProjects(): void {
    this.errorMessage.set(null);
    this.projectsStore.load().subscribe({
      error: (err: HttpErrorResponse) => {
        this.errorMessage.set(`Não foi possível carregar os projetos (${err.status}). A API está rodando em localhost:5080?`);
      },
    });
  }

  protected openCreateForm(): void {
    this.createForm.reset({ name: '', description: '', icon: 'code', deadline: '' });
    this.isCreateOpen.set(true);
  }

  protected closeCreateForm(): void {
    this.isCreateOpen.set(false);
  }

  protected submitCreateForm(): void {
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }

    const currentUser = this.authService.currentUser();
    if (!currentUser) {
      return;
    }

    const { name, description, icon, deadline } = this.createForm.getRawValue();

    this.projectsStore
      .create({
        nome: name,
        descricao: description || null,
        icone: icon,
        prazoEm: deadline || null,
        responsavelId: currentUser.id,
      })
      .subscribe({
        next: () => this.isCreateOpen.set(false),
        error: (err: HttpErrorResponse) => {
          this.errorMessage.set(err.error?.erro ?? `Falha ao criar o projeto (${err.status}).`);
        },
      });
  }

  protected deleteProject(project: Project): void {
    if (!confirm(`Excluir o projeto "${project.name}"? Isso também apaga suas tarefas.`)) {
      return;
    }

    this.projectsStore.delete(project.id).subscribe({
      error: (err: HttpErrorResponse) => {
        this.errorMessage.set(`Falha ao excluir o projeto (${err.status}).`);
      },
    });
  }
}
