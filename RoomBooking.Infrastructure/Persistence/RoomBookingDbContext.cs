namespace RoomBooking.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using RoomBooking.Domain.Entities;

public class RoomBookingDbContext(DbContextOptions<RoomBookingDbContext> options) : DbContext(options)
{
    public DbSet<Room> Rooms => this.Set<Room>();

    public DbSet<RoomService> RoomServices => this.Set<RoomService>();

    public DbSet<Booking> Bookings => this.Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(RoomBookingDbContext).Assembly);
    }
}