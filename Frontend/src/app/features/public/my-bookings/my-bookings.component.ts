import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CustomerBookingService, CustomerBookingResponseDto } from '../../../core/services/customer-booking.service';

@Component({
  selector: 'app-my-bookings',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './my-bookings.component.html',
  styleUrls: ['./my-bookings.component.scss']
})
export class MyBookingsComponent implements OnInit {
  private bookingService = inject(CustomerBookingService);

  bookings: CustomerBookingResponseDto[] = [];
  isLoading = true;

  ngOnInit() {
    this.loadBookings();
  }

  loadBookings() {
    this.isLoading = true;
    this.bookingService.getMyBookings().subscribe({
      next: (res: any) => {
        if (res.success) {
          this.bookings = res.data.items || res.data;
        }
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  cancelBooking(id: number) {
    if (confirm('Are you sure you want to cancel this reservation?')) {
      this.bookingService.cancelBooking(id).subscribe({
        next: (res: any) => {
          if (res.success) {
            this.loadBookings();
          }
        },
        error: (err: any) => {
          alert(err.error?.message || 'Failed to cancel booking.');
        }
      });
    }
  }

  getStatusBadgeClass(status: string): string {
    switch (status) {
      case 'Pending': return 'bg-warning bg-opacity-10 text-warning';
      case 'Confirmed': return 'bg-primary bg-opacity-10 text-primary';
      case 'Completed': return 'bg-success bg-opacity-10 text-success';
      case 'Cancelled': return 'bg-danger bg-opacity-10 text-danger';
      case 'NoShow': return 'bg-dark bg-opacity-10 text-dark';
      default: return 'bg-secondary bg-opacity-10 text-secondary';
    }
  }
}
