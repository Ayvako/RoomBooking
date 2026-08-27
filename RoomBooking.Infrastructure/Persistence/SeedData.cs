namespace RoomBooking.Infrastructure.Persistence.Seed;

using RoomBooking.Domain.Entities;

public static class SeedData
{
    public static async Task InitializeAsync(RoomBookingDbContext context)
    {
        if (context.Rooms.Any())
        {
            return;
        }

        var conferenceRoom = new Room
        {
            Id = Guid.NewGuid(),
            Name = "Conference Room A",
            Capacity = 10,
            BaseHourlyRate = 100,
        };

        var meetingRoom = new Room
        {
            Id = Guid.NewGuid(),
            Name = "Meeting Room B",
            Capacity = 6,
            BaseHourlyRate = 60,
        };

        var smallRoom = new Room
        {
            Id = Guid.NewGuid(),
            Name = "Small Room C",
            Capacity = 3,
            BaseHourlyRate = 40,
        };

        var projector = new RoomService
        {
            Id = Guid.NewGuid(),
            Name = "Projector",
            Price = 25,
        };

        var coffee = new RoomService
        {
            Id = Guid.NewGuid(),
            Name = "Coffee service",
            Price = 15,
        };

        var videoConference = new RoomService
        {
            Id = Guid.NewGuid(),
            Name = "Video conference",
            Price = 50,
        };

        conferenceRoom.Services.Add(projector);
        conferenceRoom.Services.Add(coffee);
        conferenceRoom.Services.Add(videoConference);

        meetingRoom.Services.Add(projector);
        meetingRoom.Services.Add(coffee);

        smallRoom.Services.Add(coffee);

        await context.Rooms.AddRangeAsync(conferenceRoom, meetingRoom, smallRoom);

        await context.SaveChangesAsync();
    }
}