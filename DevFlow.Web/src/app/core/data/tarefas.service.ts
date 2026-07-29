import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { API_BASE_URL } from '../config/api.config';
import { Task, TaskStatus } from '../../shared/models/task.model';

interface TarefaDto {
  id: number;
  titulo: string;
  descricao: string | null;
  nivel: string;
  status: string;
  projetoId: number;
  responsavelId: number | null;
  responsavelNome: string | null;
  criadoEm: string;
  atualizadoEm: string;
}

export interface CreateTarefaPayload {
  titulo: string;
  descricao: string | null;
  nivel: number;
  status: number;
  responsavelId: number | null;
}

export interface UpdateTarefaPayload {
  titulo: string;
  descricao: string | null;
  nivel: number;
  status: number;
  responsavelId: number | null;
}

const STATUS_FROM_API: Record<string, TaskStatus> = {
  AFazer: 'analise',
  Fazendo: 'em-desenvolvimento',
  Concluida: 'concluido',
};

export const STATUS_TO_API: Record<TaskStatus, number> = {
  analise: 0,
  'em-desenvolvimento': 1,
  concluido: 2,
};

function buildInitials(name: string): string {
  const parts = name.trim().split(/\s+/);
  const first = parts[0]?.[0] ?? '';
  const last = parts.length > 1 ? parts[parts.length - 1][0] : '';
  return (first + last).toUpperCase() || '?';
}

function mapTarefa(dto: TarefaDto): Task {
  return {
    id: String(dto.id),
    projectId: String(dto.projetoId),
    title: dto.titulo,
    description: dto.descricao ?? '',
    assigneeInitials: dto.responsavelNome ? buildInitials(dto.responsavelNome) : '—',
    status: STATUS_FROM_API[dto.status] ?? 'analise',
    createdAt: dto.criadoEm,
    updatedAt: dto.atualizadoEm,
  };
}

@Injectable({ providedIn: 'root' })
export class TarefasService {
  private readonly http = inject(HttpClient);

  listAll(): Observable<Task[]> {
    return this.http.get<TarefaDto[]>(`${API_BASE_URL}/tarefas`).pipe(map((dtos) => dtos.map(mapTarefa)));
  }

  listByProjeto(projetoId: string): Observable<Task[]> {
    return this.http
      .get<TarefaDto[]>(`${API_BASE_URL}/projetos/${projetoId}/tarefas`)
      .pipe(map((dtos) => dtos.map(mapTarefa)));
  }

  create(projetoId: string, payload: CreateTarefaPayload): Observable<Task> {
    return this.http
      .post<TarefaDto>(`${API_BASE_URL}/projetos/${projetoId}/tarefas`, payload)
      .pipe(map(mapTarefa));
  }

  update(id: string, payload: UpdateTarefaPayload): Observable<Task> {
    return this.http.put<TarefaDto>(`${API_BASE_URL}/tarefas/${id}`, payload).pipe(map(mapTarefa));
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${API_BASE_URL}/tarefas/${id}`);
  }
}
