import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.model';

export interface RoomType {
  id: number;
  hotelId: number;
  name: string;
  description: string;
  capacity: number;
  basePrice: number;
  imageUrl?: string;
  isActive: boolean;
}

export interface RoomTypeDto {
  hotelId: number;
  name: string;
  description: string;
  capacity: number;
  basePrice: number;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class RoomTypeService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7198/api/roomtypes';

  getRoomTypesByHotel(hotelId: number): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/hotel/${hotelId}`);
  }

  getRoomType(id: number): Observable<ApiResponse<RoomType>> {
    return this.http.get<ApiResponse<RoomType>>(`${this.apiUrl}/${id}`);
  }

  createRoomType(roomType: RoomTypeDto): Observable<ApiResponse<RoomType>> {
    return this.http.post<ApiResponse<RoomType>>(this.apiUrl, roomType);
  }

  updateRoomType(id: number, roomType: RoomTypeDto): Observable<ApiResponse<RoomType>> {
    return this.http.put<ApiResponse<RoomType>>(`${this.apiUrl}/${id}`, roomType);
  }

  deleteRoomType(id: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`);
  }

  uploadImage(id: number, file: File): Observable<ApiResponse<string>> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/${id}/image`, formData);
  }
}
