import { Injectable, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { CreateProjetoPayload, ProjetosService } from './projetos.service';
import { Project } from '../../shared/models/project.model';

@Injectable({ providedIn: 'root' })
export class ProjectsStore {
  private readonly projetosService = inject(ProjetosService);

  private readonly _projects = signal<Project[]>([]);
  private readonly _isLoading = signal(false);

  readonly projects = this._projects.asReadonly();
  readonly isLoading = this._isLoading.asReadonly();

  load(): Observable<Project[]> {
    this._isLoading.set(true);
    return this.projetosService.list().pipe(
      tap({
        next: (projects) => {
          this._projects.set(projects);
          this._isLoading.set(false);
        },
        error: () => this._isLoading.set(false),
      }),
    );
  }

  create(payload: CreateProjetoPayload): Observable<Project> {
    return this.projetosService.create(payload).pipe(
      tap((project) => this._projects.update((current) => [...current, project])),
    );
  }

  delete(id: string): Observable<void> {
    return this.projetosService.delete(id).pipe(
      tap(() => this._projects.update((current) => current.filter((project) => project.id !== id))),
    );
  }
}
