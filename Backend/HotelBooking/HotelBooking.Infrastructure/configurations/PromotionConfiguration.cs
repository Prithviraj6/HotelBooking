using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.configurations
{
    public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Code).IsRequired().HasMaxLength(50);
            builder.Property(p => p.DiscountPercent)
                   .IsRequired()
                   .HasColumnType("decimal(5,2)");
            builder.Property(p => p.Description).HasMaxLength(300);
            builder.HasIndex(p => p.Code).IsUnique();
            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
