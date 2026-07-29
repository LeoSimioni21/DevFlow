import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../config/api.config';

export interface UpdateUsuarioPayload {
  nome: string;
  email: string;
}

export interface UsuarioResponse {
  id: number;
  nome: string;
  email: string;
  criadoEm: string;
}

@Injectable({ providedIn: 'root' })
export class UsuariosService {
  private readonly http = inject(HttpClient);

  update(id: number, payload: UpdateUsuarioPayload): Observable<UsuarioResponse> {
    return this.http.put<UsuarioResponse>(`${API_BASE_URL}/usuarios/${id}`, payload);
  }
}
