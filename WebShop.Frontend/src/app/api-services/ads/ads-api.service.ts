import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import { PageResult } from '../../core/models/paging/page-result';
import { BasePagedQuery } from '../../core/models/paging/base-paged-query';

/** Corresponds to AdStatusType.cs (serialized as a string). */
export type AdStatusType = 'Live' | 'Closed' | 'AwaitingReview' | 'Removed' | 'ShadowBanned' | 'Boosted';

/** Validation limits - mirror Ad.Constraints on the backend. */
export const AD_CONSTRAINTS = {
  titleMaxLength: 100,
  descriptionMaxLength: 2000,
  priceMax: 1_000_000,
  maxOptions: 10,
  optionMaxLength: 30
} as const;

export interface CreateAdCommand {
  title: string;
  price: number;
  description: string;
  images?: string[];
  categoryId: number;
  location?: string | null;
  colors?: string[];
  sizes?: string[];
}

export interface AdCategoryDto {
  id: number;
  name: string;
}

/** Public seller info (no email). */
export interface AdSellerDto {
  name: string;
  surname: string;
}

export interface ListAdDto {
  id: number;
  title: string;
  price: number;
  description: string;
  userId: number;
  images?: string[];
  category?: AdCategoryDto;
  location?: string;
  status: AdStatusType | null;
  isBoosted: boolean;
  createdAtUtc: string;
  userDto: AdSellerDto;
}

export interface AdDetailDto {
  id: number;
  title: string;
  price: number;
  description: string;
  userId: number;
  images: string[];
  category?: AdCategoryDto;
  location?: string;
  colors: string[];
  sizes: string[];
  status: AdStatusType | null;
  isBoosted: boolean;
  isAvailable: boolean;
  createdAtUtc: string;
  userDto: AdSellerDto;
}

/** Corresponds to ListAdsQuery.cs */
export class ListAdsRequest extends BasePagedQuery {
  search?: string | null;
  categoryName?: string | null;
  minPrice?: number | null;
  maxPrice?: number | null;
  location?: string | null;
  sortBy?: string | null;
  /** only the current user's ads, in every status */
  mine?: boolean | null;
  status?: AdStatusType | null;
}

@Injectable({ providedIn: 'root' })
export class AdsApiService {
  private readonly baseUrl = `${environment.apiUrl}/Ads`;
  private http = inject(HttpClient);

  list(request?: ListAdsRequest): Observable<PageResult<ListAdDto>> {
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<PageResult<ListAdDto>>(this.baseUrl, { params });
  }

  getById(id: number): Observable<AdDetailDto> {
    return this.http.get<AdDetailDto>(`${this.baseUrl}/${id}`);
  }

  create(payload: CreateAdCommand): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.baseUrl, payload);
  }

  update(id: number, payload: CreateAdCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  /** Owner/admin only. Changes the primary status; 'Boosted' adds the boost flag. */
  changeStatus(id: number, status: AdStatusType): Observable<void> {
    const params = new HttpParams().set('statusType', status);
    return this.http.put<void>(`${this.baseUrl}/${id}/changeAdStatus`, null, { params });
  }
}
