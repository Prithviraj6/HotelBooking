import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, NgOptimizedImage } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HotelService, Hotel } from '../../../core/services/hotel.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule, NgOptimizedImage],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  private hotelService = inject(HotelService);

  hotels: Hotel[] = [];
  isLoading = true;

  ngOnInit() {
    this.hotelService.getHotels().subscribe({
      next: (res: any) => {
        if (res.success) {
          this.hotels = res.data.items || res.data;
        }
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  getStars(rating: number): any[] {
    return Array(rating).fill(0);
  }
}
