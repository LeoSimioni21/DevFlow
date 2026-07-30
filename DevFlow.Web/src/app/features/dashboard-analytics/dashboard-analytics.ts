import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { DashboardData, DashboardService } from '../../core/data/dashboard.service';
import { BreadcrumbService } from '../../core/layout/breadcrumb.service';

interface PresentationItem {
  label: string;
  className: string;
}

const STATUS_PRESENTATION: Record<string, PresentationItem> = {
  AFazer: { label: 'A fazer', className: 'neutral' },
  Fazendo: { label: 'Em desenvolvimento', className: 'warning' },
  Concluida: { label: 'Concluído', className: 'success' },
};

const PRIORIDADE_PRESENTATION: Record<string, PresentationItem> = {
  Baixa: { label: 'Baixa', className: 'neutral' },
  Media: { label: 'Média', className: 'warning' },
  Alta: { label: 'Alta', className: 'danger' },
};

const PONTO_CRITICO_LABEL: Record<string, string> = {
  PrioridadeAltaEmAberto: 'Prioridade alta em aberto',
  SemHoraFim: 'Sem horário final',
  ProjetoCritico: 'Projeto crítico',
};

function eficaciaClassName(percentual: number): string {
  if (percentual >= 80) return 'success';
  if (percentual >= 50) return 'warning';
  return 'danger';
}

@Component({
  selector: 'app-dashboard-analytics',
  imports: [RouterLink, DecimalPipe, FormsModule],
  templateUrl: './dashboard-analytics.html',
  styleUrl: './dashboard-analytics.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DashboardAnalytics {
  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly dashboardService = inject(DashboardService);

  protected readonly isLoading = signal(true);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly data = signal<DashboardData | null>(null);

  protected readonly dataInicio = signal('');
  protected readonly dataFim = signal('');

  protected readonly statusItems = computed(() =>
    (this.data()?.porStatus ?? []).map((item) => ({
      ...item,
      presentation: STATUS_PRESENTATION[item.chave] ?? { label: item.chave, className: 'neutral' },
    })),
  );

  protected readonly prioridadeItems = computed(() =>
    (this.data()?.porPrioridade ?? []).map((item) => ({
      ...item,
      presentation: PRIORIDADE_PRESENTATION[item.chave] ?? { label: item.chave, className: 'neutral' },
    })),
  );

  protected readonly pontosCriticos = computed(() =>
    (this.data()?.pontosCriticos ?? []).map((ponto) => ({
      ...ponto,
      label: PONTO_CRITICO_LABEL[ponto.tipo] ?? ponto.tipo,
    })),
  );

  protected readonly desempenho = computed(() =>
    (this.data()?.desempenho ?? []).map((item) => ({
      ...item,
      eficaciaClassName: eficaciaClassName(item.eficaciaPercentual),
    })),
  );

  constructor() {
    this.breadcrumbService.set([{ label: 'Início', link: '/projects' }, { label: 'Dashboard' }]);
    this.loadDashboard();
  }

  protected applyFilter(): void {
    this.loadDashboard();
  }

  protected clearFilter(): void {
    this.dataInicio.set('');
    this.dataFim.set('');
    this.loadDashboard();
  }

  private loadDashboard(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.dashboardService
      .get({ dataInicio: this.dataInicio() || null, dataFim: this.dataFim() || null })
      .subscribe({
        next: (data) => {
          this.data.set(data);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.errorMessage.set('Não foi possível carregar o dashboard.');
        },
      });
  }
}
