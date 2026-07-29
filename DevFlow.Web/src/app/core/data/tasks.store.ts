import { Injectable, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { CreateTarefaPayload, TarefasService, UpdateTarefaPayload } from './tarefas.service';
import { Task } from '../../shared/models/task.model';

@Injectable({ providedIn: 'root' })
export class TasksStore {
  private readonly tarefasService = inject(TarefasService);

  private readonly _tasksByProject = signal<Record<string, Task[]>>({});

  getTasks(projectId: string): Task[] {
    return this._tasksByProject()[projectId] ?? [];
  }

  loadForProject(projectId: string): Observable<Task[]> {
    return this.tarefasService.listByProjeto(projectId).pipe(
      tap((tasks) => this._tasksByProject.update((current) => ({ ...current, [projectId]: tasks }))),
    );
  }

  addTask(projectId: string, payload: CreateTarefaPayload): Observable<Task> {
    return this.tarefasService.create(projectId, payload).pipe(
      tap((task) =>
        this._tasksByProject.update((current) => ({
          ...current,
          [projectId]: [...(current[projectId] ?? []), task],
        })),
      ),
    );
  }

  updateTask(projectId: string, taskId: string, payload: UpdateTarefaPayload): Observable<Task> {
    return this.tarefasService.update(taskId, payload).pipe(
      tap((task) =>
        this._tasksByProject.update((current) => ({
          ...current,
          [projectId]: (current[projectId] ?? []).map((item) => (item.id === taskId ? task : item)),
        })),
      ),
    );
  }

  deleteTask(projectId: string, taskId: string): Observable<void> {
    return this.tarefasService.delete(taskId).pipe(
      tap(() =>
        this._tasksByProject.update((current) => ({
          ...current,
          [projectId]: (current[projectId] ?? []).filter((item) => item.id !== taskId),
        })),
      ),
    );
  }
}
