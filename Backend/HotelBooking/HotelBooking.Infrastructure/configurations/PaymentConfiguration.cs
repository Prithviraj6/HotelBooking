using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Amount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");
            builder.Property(p => p.Method).HasConversion<string>();
            builder.Property(p => p.Status).HasConversion<string>();
            builder.Property(p => p.TransactionId).HasMaxLength(100);
            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
