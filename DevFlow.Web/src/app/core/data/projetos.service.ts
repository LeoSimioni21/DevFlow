import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { API_BASE_URL } from '../config/api.config';
import { Project, ProjectIconType, ProjectStatus } from '../../shared/models/project.model';

interface ProjetoDto {
  id: number;
  nome: string;
  descricao: string | null;
  icone: string | null;
  prazoEm: string | null;
  status: string;
  criadoEm: string;
  responsavelId: number;
  totalTarefas: number;
  progresso: number;
}

export interface CreateProjetoPayload {
  nome: string;
  descricao: string | null;
  icone: ProjectIconType;
  prazoEm: string | null;
  responsavelId: number;
}

const STATUS_FROM_API: Record<string, ProjectStatus> = {
  EmDia: 'in-progress',
  Atencao: 'attention',
  Critico: 'critical',
};

const ICON_TYPES: ProjectIconType[] = ['code', 'mobile', 'devops', 'api'];
const MONTHS = ['jan', 'fev', 'mar', 'abr', 'mai', 'jun', 'jul', 'ago', 'set', 'out', 'nov', 'dez'];

function mapProjeto(dto: ProjetoDto): Project {
  const deadlineDate = dto.prazoEm ? new Date(dto.prazoEm) : null;

  return {
    id: String(dto.id),
    name: dto.nome,
    description: dto.descricao ?? '',
    icon: ICON_TYPES.includes(dto.icone as ProjectIconType) ? (dto.icone as ProjectIconType) : 'code',
    progress: dto.progresso,
    deadline: deadlineDate ? `${deadlineDate.getDate()} ${MONTHS[deadlineDate.getMonth()]}` : '',
    hasDeadline: deadlineDate !== null,
    status: STATUS_FROM_API[dto.status] ?? 'in-progress',
    tasksCount: dto.totalTarefas,
  };
}

@Injectable({ providedIn: 'root' })
export class ProjetosService {
  private readonly http = inject(HttpClient);

  list(): Observable<Project[]> {
    return this.http.get<ProjetoDto[]>(`${API_BASE_URL}/projetos`).pipe(map((dtos) => dtos.map(mapProjeto)));
  }

  getById(id: string): Observable<Project> {
    return this.http.get<ProjetoDto>(`${API_BASE_URL}/projetos/${id}`).pipe(map(mapProjeto));
  }

  create(payload: CreateProjetoPayload): Observable<Project> {
    return this.http.post<ProjetoDto>(`${API_BASE_URL}/projetos`, payload).pipe(map(mapProjeto));
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${API_BASE_URL}/projetos/${id}`);
  }
}
