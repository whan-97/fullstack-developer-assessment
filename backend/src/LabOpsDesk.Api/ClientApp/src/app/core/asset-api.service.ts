import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Asset,
  CheckOutAssetRequest,
  CreateAssetRequest,
  UpdateAssetRequest,
} from './models';

@Injectable({ providedIn: 'root' })
export class AssetApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/assets';

  list(): Observable<Asset[]> {
    return this.http.get<Asset[]>(this.baseUrl);
  }

  get(id: string): Observable<Asset> {
    return this.http.get<Asset>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateAssetRequest): Observable<Asset> {
    return this.http.post<Asset>(this.baseUrl, request);
  }

  update(id: string, request: UpdateAssetRequest): Observable<Asset> {
    return this.http.put<Asset>(`${this.baseUrl}/${id}`, request);
  }

  remove(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  checkOut(id: string, request: CheckOutAssetRequest): Observable<Asset> {
    return this.http.post<Asset>(`${this.baseUrl}/${id}/checkout`, request);
  }

  checkIn(id: string): Observable<Asset> {
    return this.http.post<Asset>(`${this.baseUrl}/${id}/checkin`, {});
  }
}
