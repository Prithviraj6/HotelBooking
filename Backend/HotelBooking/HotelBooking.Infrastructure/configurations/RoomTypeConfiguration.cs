using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.configurations
{
    public class RoomTypeConfiguration : IEntityTypeConfiguration<RoomType>
    {
        public void Configure(EntityTypeBuilder<RoomType> builder)
        {
            builder.HasKey(rt => rt.Id);
            builder.Property(rt => rt.TypeName).IsRequired().HasMaxLength(100);
            builder.Property(rt => rt.PricePerNight)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");
            builder.Property(rt => rt.MaxOccupancy).IsRequired();
            builder.Property(rt => rt.Category).HasConversion<string>();
            builder.HasQueryFilter(rt => !rt.IsDeleted);
        }
    }
}
