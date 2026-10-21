/**
 * Pagination parameters for API requests.
 * Corresponds to: PageRequest.cs (the backend caps pageSize at 100)
 */
export class PageRequest {
  page: number;
  pageSize: number;

  constructor(page: number = 1, pageSize: number = 10) {
    this.page = page;
    this.pageSize = pageSize;
  }
}
