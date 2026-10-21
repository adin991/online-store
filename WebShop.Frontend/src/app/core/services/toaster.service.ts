import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface Toast {
  id: number;
  message: string;
  type: 'success' | 'error' | 'warning' | 'info';
}

@Injectable({ providedIn: 'root' })
export class ToasterService {
  private counter = 0;
  toasts$ = new BehaviorSubject<Toast[]>([]);

  success(message: string): void { this.add(message, 'success'); }
  error(message: string): void   { this.add(message, 'error'); }
  warning(message: string): void { this.add(message, 'warning'); }
  info(message: string): void    { this.add(message, 'info'); }

  remove(id: number): void {
    this.toasts$.next(this.toasts$.value.filter(t => t.id !== id));
  }

  private add(message: string, type: Toast['type']): void {
    const id = ++this.counter;
    this.toasts$.next([...this.toasts$.value, { id, message, type }]);
    setTimeout(() => this.remove(id), 3500);
  }
}
