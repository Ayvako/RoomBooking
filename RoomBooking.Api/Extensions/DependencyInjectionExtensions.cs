namespace RoomBooking.Api.Extensions;

using Microsoft.EntityFrameworkCore;
using RoomBooking.Application.Interfaces;
using RoomBooking.Application.Services;
using RoomBooking.Infrastructure.Persistence;
using RoomBooking.Infrastructure.Repositories;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RoomBookingDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IRoomServiceRepository, RoomServiceRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        services.AddScoped<RoomApplicationService>();
        services.AddScoped<RoomServiceApplicationService>();
        services.AddScoped<BookingApplicationService>();
        return services;
    }

    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();

        return services;
    }
}