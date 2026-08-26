namespace RoomBooking.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomBooking.Domain.Entities;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(booking => booking.Id);

        builder.Property(booking => booking.StartTime)
            .IsRequired();

        builder.Property(booking => booking.EndTime)
            .IsRequired();

        builder.Property(booking => booking.TotalPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(booking => booking.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(booking => booking.Room)
            .WithMany()
            .HasForeignKey(booking => booking.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}