using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using BCrypt.Net;

namespace HotelBooking.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static void SeedData(AppDbContext context)
        {
            // Apply migrations if any are pending
            context.Database.Migrate();

            // Check if data already exists
            if (context.Users.Any() || context.Hotels.Any())
                return;

            // 1. Create Users
            var superAdmin = new User
            {
                FirstName = "System",
                LastName = "Admin",
                Email = "admin@hotelbooking.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                PhoneNumber = "1234567890",
                Role = UserRole.SuperAdmin,
                CreatedAt = DateTime.UtcNow
            };

            var hotelAdmin1 = new User
            {
                FirstName = "Alice",
                LastName = "Manager",
                Email = "manager1@grandhotel.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                PhoneNumber = "1234567891",
                Role = UserRole.HotelAdmin,
                CreatedAt = DateTime.UtcNow
            };

            var hotelAdmin2 = new User
            {
                FirstName = "Bob",
                LastName = "Director",
                Email = "manager2@seaview.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                PhoneNumber = "1234567892",
                Role = UserRole.HotelAdmin,
                CreatedAt = DateTime.UtcNow
            };

            var customer1 = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password@123"),
                PhoneNumber = "9876543210",
                Role = UserRole.Customer,
                CreatedAt = DateTime.UtcNow
            };

            var customer2 = new User
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password@123"),
                PhoneNumber = "9876543211",
                Role = UserRole.Customer,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.AddRange(superAdmin, hotelAdmin1, hotelAdmin2, customer1, customer2);
            context.SaveChanges();

            // 2. Create Hotels
            var hotel1 = new Hotel
            {
                Name = "The Grand Horizon",
                Location = "123 Horizon Blvd",
                State = "NY",
                City = "New York",
                Country = "USA",
                Description = "Experience luxury with a stunning view of the skyline.",
                StarRating = 5,
                ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&q=80&w=1000",
                CreatedAt = DateTime.UtcNow
            };

            var hotel2 = new Hotel
            {
                Name = "Sea View Resort",
                Location = "456 Ocean Drive",
                State = "FL",
                City = "Miami",
                Country = "USA",
                Description = "A beautiful beachfront resort for your perfect vacation.",
                StarRating = 4,
                ImageUrl = "https://images.unsplash.com/photo-1582719508461-905c673771fd?auto=format&fit=crop&q=80&w=1000",
                CreatedAt = DateTime.UtcNow
            };

            var hotel3 = new Hotel
            {
                Name = "Alpine Escape",
                Location = "789 Mountain Way",
                State = "CO",
                City = "Denver",
                Country = "USA",
                Description = "Cozy cabins and suites nested in the mountains.",
                StarRating = 3,
                ImageUrl = "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?auto=format&fit=crop&q=80&w=1000",
                CreatedAt = DateTime.UtcNow
            };
            
            var hotel4 = new Hotel
            {
                Name = "The Royal Savoy",
                Location = "1 The Strand",
                State = "ENG",
                City = "London",
                Country = "UK",
                Description = "Iconic luxury hotel in the heart of London.",
                StarRating = 5,
                ImageUrl = "https://images.unsplash.com/photo-1551882547-ff40c0d5b9af?auto=format&fit=crop&q=80&w=1000",
                CreatedAt = DateTime.UtcNow
            };

            var hotel5 = new Hotel
            {
                Name = "Eiffel Views Boutique",
                Location = "24 Rue Cler",
                State = "IDF",
                City = "Paris",
                Country = "France",
                Description = "Charming boutique hotel with direct views of the Eiffel Tower.",
                StarRating = 4,
                ImageUrl = "https://images.unsplash.com/photo-1502602898657-3e91760cbb34?auto=format&fit=crop&q=80&w=1000",
                CreatedAt = DateTime.UtcNow
            };

            var hotel6 = new Hotel
            {
                Name = "Sydney Harbour Suites",
                Location = "1 Macquarie St",
                State = "NSW",
                City = "Sydney",
                Country = "Australia",
                Description = "Overlooking the beautiful Sydney Harbour and Opera House.",
                StarRating = 5,
                ImageUrl = "https://images.unsplash.com/photo-1506973035872-a4ec16b8e8d9?auto=format&fit=crop&q=80&w=1000",
                CreatedAt = DateTime.UtcNow
            };

            var hotel7 = new Hotel
            {
                Name = "Bali Tropicana",
                Location = "Jalan Pantai Kuta",
                State = "Bali",
                City = "Kuta",
                Country = "Indonesia",
                Description = "Tropical paradise with private pool villas.",
                StarRating = 4,
                ImageUrl = "https://images.unsplash.com/photo-1534438327276-14e5300c3a48?auto=format&fit=crop&q=80&w=1000",
                CreatedAt = DateTime.UtcNow
            };

            context.Hotels.AddRange(hotel1, hotel2, hotel3, hotel4, hotel5, hotel6, hotel7);
            context.SaveChanges();

            // Link HotelAdmins to their hotels
            hotelAdmin1.ManagedHotelId = hotel1.Id;
            hotelAdmin2.ManagedHotelId = hotel2.Id;
            context.SaveChanges();

            // 3. Create Room Types
            var roomType1 = new RoomType
            {
                HotelId = hotel1.Id,
                TypeName = "Deluxe Skyline Suite",
                Description = "Spacious suite with a panoramic view.",
                Category = RoomCategory.Suite,
                MaxOccupancy = 2,
                PricePerNight = 450.00m,
                Amenities = "WiFi, King Bed, Minibar, Ocean View",
                ImageUrl = "https://images.unsplash.com/photo-1611892440504-42a792e24d32?auto=format&fit=crop&q=80&w=1000",
                CreatedAt = DateTime.UtcNow
            };

            var roomType2 = new RoomType
            {
                HotelId = hotel2.Id,
                TypeName = "Oceanfront Double",
                Description = "Two queen beds with a balcony facing the sea.",
                Category = RoomCategory.Double,
                MaxOccupancy = 4,
                PricePerNight = 250.00m,
                Amenities = "WiFi, 2 Queen Beds, Balcony",
                ImageUrl = "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&q=80&w=1000",
                CreatedAt = DateTime.UtcNow
            };

            var roomType3 = new RoomType
            {
                HotelId = hotel3.Id,
                TypeName = "Mountain Cabin",
                Description = "A standalone cabin with a fireplace.",
                Category = RoomCategory.Deluxe,
                MaxOccupancy = 6,
                PricePerNight = 300.00m,
                Amenities = "WiFi, Fireplace, Kitchen",
                ImageUrl = "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?auto=format&fit=crop&q=80&w=1000",
                CreatedAt = DateTime.UtcNow
            };

            context.RoomTypes.AddRange(roomType1, roomType2, roomType3);
            context.SaveChanges();

            // 4. Create Rooms
            var room1 = new Room { RoomTypeId = roomType1.Id, HotelId = hotel1.Id, FloorNumber = 1, RoomNumber = "101", Status = RoomStatus.Available, CreatedAt = DateTime.UtcNow };
            var room2 = new Room { RoomTypeId = roomType1.Id, HotelId = hotel1.Id, FloorNumber = 1, RoomNumber = "102", Status = RoomStatus.Available, CreatedAt = DateTime.UtcNow };
            var room3 = new Room { RoomTypeId = roomType2.Id, HotelId = hotel2.Id, FloorNumber = 2, RoomNumber = "201", Status = RoomStatus.Available, CreatedAt = DateTime.UtcNow };
            var room4 = new Room { RoomTypeId = roomType3.Id, HotelId = hotel3.Id, FloorNumber = 1, RoomNumber = "Cabin 1", Status = RoomStatus.Available, CreatedAt = DateTime.UtcNow };

            context.Rooms.AddRange(room1, room2, room3, room4);
            context.SaveChanges();

            // 5. Create Promotions
            var promo1 = new Promotion
            {
                HotelId = hotel1.Id,
                Code = "GRAND10",
                Description = "10% off your stay",
                DiscountPercent = 10m,
                ValidFrom = DateTime.UtcNow,
                ValidTo = DateTime.UtcNow.AddMonths(1),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            context.Promotions.Add(promo1);
            context.SaveChanges();

            // 6. Create Bookings
            var booking1 = new Booking
            {
                UserId = customer1.Id,
                RoomId = room1.Id,
                CheckInDate = DateTime.UtcNow.AddDays(5),
                CheckOutDate = DateTime.UtcNow.AddDays(8),
                TotalNights = 3,
                TotalPrice = 1350.00m,
                Status = BookingStatus.Confirmed,
                SpecialRequests = "Late check-in",
                CreatedAt = DateTime.UtcNow
            };

            var booking2 = new Booking
            {
                UserId = customer2.Id,
                RoomId = room3.Id,
                CheckInDate = DateTime.UtcNow.AddDays(-10),
                CheckOutDate = DateTime.UtcNow.AddDays(-7),
                TotalNights = 3,
                TotalPrice = 750.00m,
                Status = BookingStatus.Completed,
                CreatedAt = DateTime.UtcNow
            };

            context.Bookings.AddRange(booking1, booking2);
            context.SaveChanges();

            // 7. Create Payments
            var payment1 = new Payment
            {
                BookingId = booking1.Id,
                Amount = 1350.00m,
                PaidAt = DateTime.UtcNow,
                Status = PaymentStatus.Success,
                TransactionId = "txn_1234567890",
                Method = PaymentMethod.Card,
                CreatedAt = DateTime.UtcNow
            };

            context.Payments.Add(payment1);
            context.SaveChanges();

            // 8. Create Reviews
            var review1 = new Review
            {
                HotelId = hotel2.Id,
                UserId = customer2.Id,
                BookingId = booking2.Id,
                Rating = 5,
                Comment = "Absolutely loved the ocean view! Will come back again.",
                CreatedAt = DateTime.UtcNow
            };

            context.Reviews.Add(review1);
            context.SaveChanges();
        }
    }
}
