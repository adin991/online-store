import { Directive } from '@angular/core';
import { PageResult, pageCount } from '../../models/paging/page-result';
import { BaseListComponent } from './base-list-component';
import { BasePagedQuery } from '../../models/paging/base-paged-query';

/**
 * Base class for paged lists. The child sets `request`, implements loadPagedData()
 * and passes the API response to handlePageResult(); the template binds the
 * shared <app-paginator> to page/pageSize/totalItems and calls goToPage/changePageSize.
 */
@Directive()
export abstract class BaseListPagedComponent<TItem, TRequest extends BasePagedQuery>
  extends BaseListComponent<TItem> {

  request!: TRequest;
  totalItems = 0;
  totalPages = 1;

  get paging() {
    return this.request.paging;
  }

  protected abstract loadPagedData(): void;

  protected override loadData(): void {
    this.loadPagedData();
  }

  protected handlePageResult(result: PageResult<TItem>) {
    this.items = result.items ?? [];
    this.totalItems = result.total ?? 0;
    this.totalPages = pageCount(this.totalItems, this.paging.pageSize);
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.paging.page = page;
    this.loadPagedData();
  }

  nextPage() { this.goToPage(this.paging.page + 1); }
  prevPage() { this.goToPage(this.paging.page - 1); }

  changePageSize(size: number) {
    this.paging.pageSize = size;
    this.paging.page = 1;
    this.loadPagedData();
  }

  /** Call when a filter changes - always restarts from the first page. */
  applyFilters(): void {
    this.paging.page = 1;
    this.loadPagedData();
  }
}
