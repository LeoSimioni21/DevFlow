import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../config/api.config';

export interface DashboardPercentualItem {
  chave: string;
  quantidade: number;
  percentual: number;
}

export interface PontoCritico {
  tipo: string;
  descricao: string;
  tarefaId: number | null;
  tarefaCodigo: string | null;
  projetoId: number | null;
  projetoNome: string | null;
}

export interface DashboardData {
  totalTarefas: number;
  totalHorasTrabalhadas: number;
  mediaHorasPorTarefa: number;
  porStatus: DashboardPercentualItem[];
  porPrioridade: DashboardPercentualItem[];
  pontosCriticos: PontoCritico[];
}

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly http = inject(HttpClient);

  get(): Observable<DashboardData> {
    return this.http.get<DashboardData>(`${API_BASE_URL}/dashboard`);
  }
}
