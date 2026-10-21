import { Injectable, signal } from '@angular/core';

export interface OrderModalData {
  title: string;
  message: string;
}

@Injectable({ providedIn: 'root' })
export class OrderModalService {
  private _data = signal<OrderModalData | null>(null);
  data = this._data.asReadonly();

  open(data: OrderModalData): void {
    this._data.set(data);
  }

  close(): void {
    this._data.set(null);
  }
}
