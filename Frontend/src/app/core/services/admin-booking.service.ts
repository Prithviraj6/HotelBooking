import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.model';

export interface BookingResponseDto {
  id: number;
  hotelName: string;
  roomTypeName: string;
  checkInDate: string;
  checkOutDate: string;
  totalPrice: number;
  status: string;
  guestName: string;
}

@Injectable({
  providedIn: 'root'
})
export class AdminBookingService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7198/api/bookings';

  getAllBookings(): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(this.apiUrl);
  }

  confirmBooking(id: number): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.apiUrl}/${id}/confirm`, {});
  }

  completeBooking(id: number): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.apiUrl}/${id}/complete`, {});
  }

  markNoShow(id: number): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.apiUrl}/${id}/no-show`, {});
  }
}
