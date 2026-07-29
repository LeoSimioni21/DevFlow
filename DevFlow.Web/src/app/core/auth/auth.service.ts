import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, map, tap } from 'rxjs';
import { API_BASE_URL } from '../config/api.config';

export interface AuthUser {
  id: number;
  name: string;
  email: string;
  initials: string;
}

interface LoginApiResponse {
  token: string;
  usuario: { id: number; nome: string; email: string; criadoEm: string };
}

interface StoredSession {
  token: string;
  user: AuthUser;
}

const STORAGE_KEY = 'devflow.auth.session';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly _session = signal<StoredSession | null>(this.restoreSession());

  readonly currentUser = computed(() => this._session()?.user ?? null);
  readonly token = computed(() => this._session()?.token ?? null);
  readonly isAuthenticated = computed(() => this._session() !== null);

  login(email: string, senha: string, rememberMe: boolean): Observable<AuthUser> {
    return this.http.post<LoginApiResponse>(`${API_BASE_URL}/auth/login`, { email, senha }).pipe(
      map((response) => this.toSession(response)),
      tap((session) => this.setSession(session, rememberMe)),
      map((session) => session.user),
    );
  }

  updateCurrentUser(patch: { name: string; email: string }): void {
    const session = this._session();
    if (!session) {
      return;
    }

    const updated: StoredSession = {
      ...session,
      user: { ...session.user, name: patch.name, email: patch.email, initials: this.buildInitials(patch.name) },
    };
    this._session.set(updated);

    const storage = localStorage.getItem(STORAGE_KEY) !== null ? localStorage : sessionStorage;
    storage.setItem(STORAGE_KEY, JSON.stringify(updated));
  }

  logout(): void {
    this._session.set(null);
    localStorage.removeItem(STORAGE_KEY);
    sessionStorage.removeItem(STORAGE_KEY);
  }

  private toSession(response: LoginApiResponse): StoredSession {
    return {
      token: response.token,
      user: {
        id: response.usuario.id,
        name: response.usuario.nome,
        email: response.usuario.email,
        initials: this.buildInitials(response.usuario.nome),
      },
    };
  }

  private setSession(session: StoredSession, rememberMe: boolean): void {
    this._session.set(session);
    const storage = rememberMe ? localStorage : sessionStorage;
    storage.setItem(STORAGE_KEY, JSON.stringify(session));
  }

  private restoreSession(): StoredSession | null {
    const raw = localStorage.getItem(STORAGE_KEY) ?? sessionStorage.getItem(STORAGE_KEY);
    return raw ? (JSON.parse(raw) as StoredSession) : null;
  }

  private buildInitials(name: string): string {
    const parts = name.trim().split(/\s+/);
    const first = parts[0]?.[0] ?? '';
    const last = parts.length > 1 ? parts[parts.length - 1][0] : '';
    return (first + last).toUpperCase() || 'U';
  }
}
