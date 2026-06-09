import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { RoomTypeService } from '../../../core/services/room-type.service';
import { AdminBookingService } from '../../../core/services/admin-booking.service';

@Component({
  selector: 'app-hotel-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './hotel-admin-dashboard.component.html',
  styleUrls: ['./hotel-admin-dashboard.component.scss']
})
export class HotelAdminDashboardComponent implements OnInit {
  authService = inject(AuthService);
  private roomService = inject(RoomTypeService);
  private bookingService = inject(AdminBookingService);

  totalRooms = 0;
  pendingBookings = 0;
  isLoading = true;

  ngOnInit() {
    this.loadStats();
  }

  loadStats() {
    const hotelId = this.authService.currentUser()?.managedHotelId;
    if (!hotelId) {
      this.isLoading = false;
      return;
    }

    // Load Room Types Count
    this.roomService.getRoomTypesByHotel(hotelId).subscribe({
      next: (res: any) => {
        if (res.success) {
          const items = res.data.items || res.data;
          this.totalRooms = items.length || 0;
        }
      }
    });

    // Load Bookings
    this.bookingService.getAllBookings().subscribe({
      next: (res: any) => {
        if (res.success) {
          const items = res.data.items || res.data;
          // Just a simple mock count for pending bookings based on status
          this.pendingBookings = items.filter((b: any) => b.status === 'Pending').length;
        }
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }
}
