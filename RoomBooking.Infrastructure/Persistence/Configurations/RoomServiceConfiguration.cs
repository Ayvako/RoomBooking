namespace RoomBooking.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomBooking.Domain.Entities;

public class RoomServiceConfiguration : IEntityTypeConfiguration<RoomService>
{
    public void Configure(EntityTypeBuilder<RoomService> builder)
    {
        builder.HasKey(service => service.Id);

        builder.Property(service => service.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(service => service.Price)
            .IsRequired()
            .HasPrecision(18, 2);
    }
}