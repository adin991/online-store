import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

/** Gender as serialized by the API (JsonStringEnumConverter) or as a number. */
export type GenderValue = 'Male' | 'Female' | number;

/** GET api/User/me and GET api/User/{id} (self or admin only). */
export interface UserProfileDto {
  id: number;
  name: string;
  surname: string;
  email: string;
  gender: GenderValue;
  pfp: string | null;
  isAdmin: boolean;
}

/** GET api/User/{id}/public - minimal seller info, no private fields. */
export interface PublicUserProfileDto {
  id: number;
  name: string;
  surname: string;
  pfp: string | null;
  activeAdsCount: number;
}

export interface UpdateUserDto {
  name: string;
  surname: string;
  email: string;
  gender: number;
}

/** GET api/User (admin only). Corresponds to ListUserQueryDto.cs */
export interface ListUserDto {
  id: number;
  name: string;
  surname: string;
  email: string;
  isAdmin: boolean;
  isEnabled: boolean;
  createdAtUtc: string;
}

/** Corresponds to ListUserQuery.cs */
export class ListUsersRequest extends BasePagedQuery {
  search?: string | null;
  isAdmin?: boolean | null;
  sortBy?: string | null;
}

@Injectable({ providedIn: 'root' })
export class UserApiService {
  private readonly baseUrl = `${environment.apiUrl}/api/User`;
  private http = inject(HttpClient);

  static genderToNumber(gender: GenderValue): number {
    if (typeof gender === 'number') return gender;
    return gender === 'Female' ? 1 : 0;
  }

  getMe(): Observable<UserProfileDto> {
    return this.http.get<UserProfileDto>(`${this.baseUrl}/me`);
  }

  getById(id: number): Observable<UserProfileDto> {
    return this.http.get<UserProfileDto>(`${this.baseUrl}/${id}`);
  }

  getPublicProfile(id: number): Observable<PublicUserProfileDto> {
    return this.http.get<PublicUserProfileDto>(`${this.baseUrl}/${id}/public`);
  }

  list(request: ListUsersRequest): Observable<PageResult<ListUserDto>> {
    return this.http.get<PageResult<ListUserDto>>(this.baseUrl, { params: buildHttpParams(request as any) });
  }

  update(id: number, data: UpdateUserDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, data);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
