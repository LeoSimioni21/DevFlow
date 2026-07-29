import { Injectable, signal } from '@angular/core';

export interface BreadcrumbItem {
  label: string;
  link?: string;
}

@Injectable({ providedIn: 'root' })
export class BreadcrumbService {
  private readonly _items = signal<BreadcrumbItem[]>([]);

  readonly items = this._items.asReadonly();

  set(items: BreadcrumbItem[]): void {
    this._items.set(items);
  }
}
