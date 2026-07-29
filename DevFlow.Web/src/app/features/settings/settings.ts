import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/auth/auth.service';
import { UsuariosService } from '../../core/data/usuarios.service';
import { BreadcrumbService } from '../../core/layout/breadcrumb.service';
import { ThemeService } from '../../core/theme/theme.service';

@Component({
  selector: 'app-settings',
  imports: [ReactiveFormsModule],
  templateUrl: './settings.html',
  styleUrl: './settings.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Settings {
  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly usuariosService = inject(UsuariosService);
  protected readonly themeService = inject(ThemeService);

  protected readonly isSaving = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly successMessage = signal<string | null>(null);

  protected readonly form = this.formBuilder.nonNullable.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
  });

  constructor() {
    this.breadcrumbService.set([{ label: 'Início', link: '/projects' }, { label: 'Configurações' }]);

    const currentUser = this.authService.currentUser();
    if (currentUser) {
      this.form.setValue({ name: currentUser.name, email: currentUser.email });
    }
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const currentUser = this.authService.currentUser();
    if (!currentUser) {
      return;
    }

    const { name, email } = this.form.getRawValue();
    this.isSaving.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.usuariosService.update(currentUser.id, { nome: name, email }).subscribe({
      next: () => {
        this.authService.updateCurrentUser({ name, email });
        this.isSaving.set(false);
        this.successMessage.set('Perfil atualizado com sucesso.');
      },
      error: (err: HttpErrorResponse) => {
        this.isSaving.set(false);
        this.errorMessage.set(err.error?.erro ?? `Falha ao salvar (${err.status}).`);
      },
    });
  }
}
