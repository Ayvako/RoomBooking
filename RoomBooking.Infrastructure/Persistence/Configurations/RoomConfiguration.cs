namespace RoomBooking.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomBooking.Domain.Entities;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(room => room.Id);

        builder.Property(room => room.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(room => room.Capacity)
            .IsRequired();

        builder.Property(room => room.BaseHourlyRate)
            .IsRequired()
            .HasPrecision(18, 2);
        builder
            .HasMany(room => room.Services)
            .WithMany(service => service.Rooms)
            .UsingEntity("RoomRoomService");
    }
}