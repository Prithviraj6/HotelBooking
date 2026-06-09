import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.model';

export interface HotelAdminDto {
  id?: number;
  userId: number;
  hotelId: number;
  userEmail?: string;
  userFullName?: string;
  hotelName?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AdminManagementService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7198/api/admin/hotel-admins';

  getHotelAdmins(): Observable<ApiResponse<HotelAdminDto[]>> {
    return this.http.get<ApiResponse<HotelAdminDto[]>>(this.apiUrl);
  }

  assignHotelAdmin(assignment: HotelAdminDto): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(this.apiUrl, assignment);
  }

  removeHotelAdmin(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`);
  }
}
