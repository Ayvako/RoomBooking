namespace RoomBooking.Infrastructure.Persistence;

using RoomBooking.Domain.Entities;

public static class SeedData
{
    public static async Task InitializeAsync(RoomBookingDbContext context)
    {
        if (context.Rooms.Any())
        {
            return;
        }

        var roomA = new Room
        {
            Id = Guid.NewGuid(),
            Name = "Зал A",
            Capacity = 50,
            BaseHourlyRate = 2000,
        };

        var roomB = new Room
        {
            Id = Guid.NewGuid(),
            Name = "Зал B",
            Capacity = 100,
            BaseHourlyRate = 3500,
        };

        var roomC = new Room
        {
            Id = Guid.NewGuid(),
            Name = "Зал C",
            Capacity = 30,
            BaseHourlyRate = 1500,
        };

        var projector = new RoomService
        {
            Id = Guid.NewGuid(),
            Name = "Проєктор",
            Price = 500,
        };

        var wiFi = new RoomService
        {
            Id = Guid.NewGuid(),
            Name = "Wi-Fi",
            Price = 300,
        };

        var sound = new RoomService
        {
            Id = Guid.NewGuid(),
            Name = "Звук",
            Price = 700,
        };

        roomA.Services.Add(projector);
        roomA.Services.Add(wiFi);
        roomA.Services.Add(sound);

        roomB.Services.Add(projector);
        roomB.Services.Add(wiFi);
        roomB.Services.Add(sound);

        roomC.Services.Add(projector);
        roomC.Services.Add(wiFi);
        roomC.Services.Add(sound);

        await context.Rooms.AddRangeAsync(roomA, roomB, roomC);

        await context.SaveChangesAsync();
    }
}