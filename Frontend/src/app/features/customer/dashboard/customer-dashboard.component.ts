import { Component, inject, OnInit } from '@angular/core';
import { CommonModule, NgOptimizedImage } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CustomerBookingService, CustomerBookingResponseDto } from '../../../core/services/customer-booking.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-customer-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './customer-dashboard.component.html',
  styleUrls: ['./customer-dashboard.component.scss']
})
export class CustomerDashboardComponent implements OnInit {
  private bookingService = inject(CustomerBookingService);
  authService = inject(AuthService);

  upcomingBookings: CustomerBookingResponseDto[] = [];
  pastBookings: CustomerBookingResponseDto[] = [];
  
  isLoading = true;
  errorMessage = '';

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.bookingService.getMyBookings().subscribe({
      next: (res: any) => {
        if (res.success && res.data) {
          const items: CustomerBookingResponseDto[] = res.data.items || res.data;
          const now = new Date().getTime();
          
          const sorted = items.sort((a: any, b: any) => new Date(a.checkInDate).getTime() - new Date(b.checkInDate).getTime());
          
          this.upcomingBookings = sorted.filter(b => new Date(b.checkInDate).getTime() >= now || b.status === 'Confirmed' || b.status === 'Pending');
          this.pastBookings = sorted.filter(b => new Date(b.checkInDate).getTime() < now && b.status !== 'Confirmed' && b.status !== 'Pending')
                                    .sort((a: any, b: any) => new Date(b.checkInDate).getTime() - new Date(a.checkInDate).getTime());
        }
        this.isLoading = false;
      },
      error: (err: any) => {
        this.errorMessage = err.error?.message || 'Failed to load bookings.';
        this.isLoading = false;
      }
    });
  }

  getStatusBadgeClass(status: string): string {
    switch (status) {
      case 'Confirmed': return 'bg-success';
      case 'Pending': return 'bg-warning text-dark';
      case 'Cancelled': return 'bg-danger';
      case 'Completed': return 'bg-info text-dark';
      default: return 'bg-secondary';
    }
  }
}
