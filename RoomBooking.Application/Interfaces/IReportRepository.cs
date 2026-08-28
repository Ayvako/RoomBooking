namespace RoomBooking.Application.Interfaces;

using System;
using RoomBooking.Application.DTOs.Reports;

public interface IReportRepository
{
    Task<BookingStatisticsResponse> GetBookingStatisticsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RoomBookingStatisticsResponse>> GetRoomBookingStatisticsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}