import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.model';

export interface CreateBookingDto {
  roomTypeId: number;
  checkInDate: string;
  checkOutDate: string;
  specialRequests?: string;
}

export interface CustomerBookingResponseDto {
  id: number;
  hotelName: string;
  roomTypeName: string;
  checkInDate: string;
  checkOutDate: string;
  totalPrice: number;
  status: string;
}

@Injectable({
  providedIn: 'root'
})
export class CustomerBookingService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7198/api/bookings';

  createBooking(dto: CreateBookingDto): Observable<ApiResponse<CustomerBookingResponseDto>> {
    return this.http.post<ApiResponse<CustomerBookingResponseDto>>(this.apiUrl, dto);
  }

  getMyBookings(): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/my`);
  }

  cancelBooking(id: number): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.apiUrl}/${id}/cancel`, {});
  }
}
