using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.configurations
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasKey(h => h.Id);
            builder.Property(h => h.Name).IsRequired().HasMaxLength(200);
            builder.Property(h => h.Email).IsRequired().HasMaxLength(200);
            builder.Property(h => h.City).IsRequired().HasMaxLength(100);
            builder.Property(h => h.Country).IsRequired().HasMaxLength(100);
            builder.Property(h => h.StarRating).IsRequired();
            builder.HasQueryFilter(h => !h.IsDeleted);

            builder.HasMany(h => h.Rooms)
                   .WithOne(r => r.Hotel)
                   .HasForeignKey(r => r.HotelId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(h => h.RoomTypes)
                   .WithOne(rt => rt.Hotel)
                   .HasForeignKey(rt => rt.HotelId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(h => h.Reviews)
                   .WithOne(r => r.Hotel)
                   .HasForeignKey(r => r.HotelId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(h => h.Promotions)
                   .WithOne(p => p.Hotel)
                   .HasForeignKey(p => p.HotelId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
