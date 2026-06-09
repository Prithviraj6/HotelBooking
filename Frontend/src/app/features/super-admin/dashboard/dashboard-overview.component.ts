import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HotelService } from '../../../core/services/hotel.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-dashboard-overview',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard-overview.component.html',
  styleUrls: ['./dashboard-overview.component.scss']
})
export class DashboardOverviewComponent implements OnInit {
  private hotelService = inject(HotelService);
  authService = inject(AuthService);

  totalHotels = 0;
  isLoading = true;

  ngOnInit() {
    this.hotelService.getHotels(1, 1).subscribe({
      next: (res: any) => {
        if (res.success && res.data) {
          this.totalHotels = res.data.totalCount || res.data.items?.length || 0;
        }
        this.isLoading = false;
      },
      error: (err: any) => {
        this.isLoading = false;
      }
    });
  }
}
