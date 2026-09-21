import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  AdjustPartQuantityRequest,
  CreatePartRequest,
  Part,
  UpdatePartRequest,
} from './models';

@Injectable({ providedIn: 'root' })
export class PartApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/parts';

  list(): Observable<Part[]> {
    return this.http.get<Part[]>(this.baseUrl);
  }

  get(id: string): Observable<Part> {
    return this.http.get<Part>(`${this.baseUrl}/${id}`);
  }

  create(request: CreatePartRequest): Observable<Part> {
    return this.http.post<Part>(this.baseUrl, request);
  }

  update(id: string, request: UpdatePartRequest): Observable<Part> {
    return this.http.put<Part>(`${this.baseUrl}/${id}`, request);
  }

  remove(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  adjust(id: string, request: AdjustPartQuantityRequest): Observable<Part> {
    return this.http.post<Part>(`${this.baseUrl}/${id}/adjust`, request);
  }
}
