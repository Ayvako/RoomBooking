using RoomBooking.Domain.Entities;

namespace RoomBooking.Application.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Booking>> GetByRoomAndPeriodAsync(Guid roomId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);

    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);

    Task UpdateAsync(Booking booking, CancellationToken cancellationToken = default);

    Task DeleteAsync(Booking booking, CancellationToken cancellationToken = default);
}