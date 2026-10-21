import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import { PageResult } from '../../core/models/paging/page-result';
import { BasePagedQuery } from '../../core/models/paging/base-paged-query';

/** Mirrors Comment.Constraints.ContentMaxLength on the backend. */
export const COMMENT_MAX_LENGTH = 1000;

export interface CommentDto {
  id: number;
  content: string;
  adId: number;
  userId: number;
  createdAtUtc: string;
  user: { name: string; surname: string };
}

/** Corresponds to ListCommentsQuery.cs */
export class ListCommentsRequest extends BasePagedQuery {
  adId?: number | null;
  search?: string | null;
}

@Injectable({ providedIn: 'root' })
export class CommentsApiService {
  private readonly baseUrl = `${environment.apiUrl}/api/Comment`;
  private http = inject(HttpClient);

  list(request: ListCommentsRequest): Observable<PageResult<CommentDto>> {
    return this.http.get<PageResult<CommentDto>>(this.baseUrl, { params: buildHttpParams(request as any) });
  }

  create(adId: number, content: string): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.baseUrl, { adId, content });
  }

  update(id: number, content: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, { content });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
