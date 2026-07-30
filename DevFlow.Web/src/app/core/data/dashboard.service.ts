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

export interface DesempenhoFuncionario {
  usuarioId: number;
  nome: string;
  demanda: number;
  entrega: number;
  capacidadeHoras: number;
  eficaciaPercentual: number;
}

export interface DashboardData {
  totalTarefas: number;
  totalHorasTrabalhadas: number;
  mediaHorasPorTarefa: number;
  porStatus: DashboardPercentualItem[];
  porPrioridade: DashboardPercentualItem[];
  pontosCriticos: PontoCritico[];
  desempenho: DesempenhoFuncionario[];
}

export interface DashboardFilter {
  dataInicio: string | null;
  dataFim: string | null;
}

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly http = inject(HttpClient);

  get(filter?: DashboardFilter): Observable<DashboardData> {
    const params: string[] = [];
    if (filter?.dataInicio) {
      params.push(`dataInicio=${encodeURIComponent(filter.dataInicio)}`);
    }
    if (filter?.dataFim) {
      params.push(`dataFim=${encodeURIComponent(filter.dataFim)}`);
    }
    const query = params.length ? `?${params.join('&')}` : '';
    return this.http.get<DashboardData>(`${API_BASE_URL}/dashboard${query}`);
  }
}
