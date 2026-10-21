import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface UploadFileResultDto {
  url: string;
}

@Injectable({ providedIn: 'root' })
export class FilesApiService {
  private readonly baseUrl = `${environment.apiUrl}/api/Files`;
  private http = inject(HttpClient);

  upload(file: File): Observable<UploadFileResultDto> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<{ url: string }>(`${this.baseUrl}/upload`, formData).pipe(
      map(res => ({ url: `${environment.apiUrl}${res.url}` }))
    );
  }
}
