import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { pageCount } from '../../models/paging/page-result';

/**
 * Paginator for paged CRUD lists - bound to the backend Paging.Page / Paging.PageSize.
 */
@Component({
  selector: 'app-paginator',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  template: `
    <nav class="paginator" *ngIf="total > 0">
      <span class="paginator-info">
        {{ 'PAGING.SHOWING' | translate: { from: from, to: to, total: total } }}
      </span>

      <div class="paginator-pages">
        <button type="button" (click)="go(page - 1)" [disabled]="page <= 1" [attr.aria-label]="'PAGING.PREV' | translate">‹</button>
        <ng-container *ngFor="let p of pages">
          <span *ngIf="p === -1" class="paginator-gap">…</span>
          <button *ngIf="p !== -1" type="button" [class.active]="p === page" (click)="go(p)">{{ p }}</button>
        </ng-container>
        <button type="button" (click)="go(page + 1)" [disabled]="page >= totalPages" [attr.aria-label]="'PAGING.NEXT' | translate">›</button>
      </div>

      <label class="paginator-size">
        {{ 'PAGING.PER_PAGE' | translate }}
        <select [value]="pageSize" (change)="changeSize($any($event.target).value)">
          <option *ngFor="let s of pageSizes" [value]="s" [selected]="s === pageSize">{{ s }}</option>
        </select>
      </label>
    </nav>
  `,
  styles: [`
    .paginator { display: flex; flex-wrap: wrap; align-items: center; justify-content: space-between; gap: 12px; margin: 24px 0 8px; font-size: 14px; color: #555; }
    .paginator-pages { display: flex; gap: 4px; align-items: center; }
    .paginator-pages button { min-width: 34px; height: 34px; border: 1px solid #ddd; background: #fff; border-radius: 8px; cursor: pointer; color: #333; }
    .paginator-pages button.active { background: #1a9e7a; border-color: #1a9e7a; color: #fff; }
    .paginator-pages button:disabled { opacity: .4; cursor: default; }
    .paginator-gap { padding: 0 4px; }
    .paginator-size select { margin-left: 6px; padding: 4px 8px; border: 1px solid #ddd; border-radius: 6px; }
  `]
})
export class PaginatorComponent {
  @Input() page = 1;
  @Input() pageSize = 10;
  @Input() total = 0;
  @Input() pageSizes: number[] = [5, 10, 20, 50];

  @Output() pageChange = new EventEmitter<number>();
  @Output() pageSizeChange = new EventEmitter<number>();

  get totalPages(): number { return pageCount(this.total, this.pageSize); }
  get from(): number { return this.total === 0 ? 0 : (this.page - 1) * this.pageSize + 1; }
  get to(): number { return Math.min(this.page * this.pageSize, this.total); }

  /** Page numbers with -1 as a gap marker, e.g. 1 … 4 5 6 … 12 */
  get pages(): number[] {
    const last = this.totalPages;
    const result: number[] = [];
    for (let p = 1; p <= last; p++) {
      if (p === 1 || p === last || Math.abs(p - this.page) <= 1) {
        result.push(p);
      } else if (result[result.length - 1] !== -1) {
        result.push(-1);
      }
    }
    return result;
  }

  go(p: number): void {
    if (p < 1 || p > this.totalPages || p === this.page) return;
    this.pageChange.emit(p);
  }

  changeSize(value: string): void {
    this.pageSizeChange.emit(Number(value));
  }
}
