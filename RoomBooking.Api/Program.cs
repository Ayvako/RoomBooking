namespace RoomBooking.Api;

using Microsoft.EntityFrameworkCore;
using RoomBooking.Api.Extensions;
using RoomBooking.Api.Middleware;
using RoomBooking.Infrastructure.Persistence;
using Scalar.AspNetCore;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApiServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider
                .GetRequiredService<RoomBookingDbContext>();

            await context.Database.MigrateAsync();

            await SeedData.InitializeAsync(context);
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}