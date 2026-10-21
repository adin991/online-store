// src/app/core/models/page-result.ts

/**
 * Paged response from the API.
 * Corresponds to: PageResult.cs ({ total, items })
 */
export interface PageResult<T> {
  items: T[];
  total: number;
}

/** Number of pages for a given total and page size (at least 1). */
export function pageCount(total: number, pageSize: number): number {
  return Math.max(1, Math.ceil(total / Math.max(1, pageSize)));
}
