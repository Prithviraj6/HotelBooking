import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.model';

export interface Hotel {
  id: number;
  name: string;
  address: string;
  city: string;
  country: string;
  description: string;
  starRating: number;
  imageUrl?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface HotelDto {
  name: string;
  address: string;
  city: string;
  country: string;
  description: string;
  starRating: number;
}

@Injectable({
  providedIn: 'root'
})
export class HotelService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7198/api/hotels';

  getHotels(pageNumber = 1, pageSize = 20): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  getHotel(id: number): Observable<ApiResponse<Hotel>> {
    return this.http.get<ApiResponse<Hotel>>(`${this.apiUrl}/${id}`);
  }

  createHotel(hotel: HotelDto): Observable<ApiResponse<number>> {
    return this.http.post<ApiResponse<number>>(this.apiUrl, hotel);
  }

  updateHotel(id: number, hotel: HotelDto): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.apiUrl}/${id}`, hotel);
  }

  deleteHotel(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`);
  }

  uploadImage(id: number, file: File): Observable<ApiResponse<string>> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/${id}/image`, formData);
  }
}
