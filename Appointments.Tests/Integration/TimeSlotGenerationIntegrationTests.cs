using Appointments.Application.Commands.TimeSlots;
using Appointments.Application.Queries.TimeSlots;
using Appointments.Application.Commands.Appointments;
using Appointments.Domain.Entities;
using Appointments.Domain.Entities.Snapshots;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Appointments.Tests.Integration;

[Collection(PostgresCollection.Name)]
public class TimeSlotGenerationIntegrationTests
{
    private const int WindowDays = 14;
    private const int SlotsPerDay = 3; 

    private readonly PostgresFixture _fx;

    public TimeSlotGenerationIntegrationTests(PostgresFixture fx) => _fx = fx;

    private Task AddScheduleForEveryDayAsync(Guid doctorId) =>
        _fx.ExecuteDbAsync(async db =>
        {
            foreach (var day in Enum.GetValues<DayOfWeek>())
            {
                db.DoctorScheduleSnapshots.Add(new DoctorScheduleSnapshot
                {
                    Id = Guid.NewGuid(),
                    DoctorId = doctorId,
                    DayOfWeek = day,
                    StartTime = TimeSpan.Zero,
                    EndTime = TimeSpan.FromHours(1)
                });
            }

            await db.SaveChangesAsync();
        });

    private Task<int> CountSlotsAsync(Guid doctorId) =>
        _fx.QueryDbAsync(db => db.TimeSlots.CountAsync(t => t.DoctorId == doctorId));

    [Fact]
    public async Task Generate_RunTwice_IsIdempotentAndDoesNotTouchBookedSlots()
    {
        var seed = await TestData.SeedAsync(_fx);
        await AddScheduleForEveryDayAsync(seed.DoctorId);

        await _fx.SendAsync(new GenerateTimeSlotsCommand());
        var afterFirst = await CountSlotsAsync(seed.DoctorId);

        var slot = await _fx.QueryDbAsync(db => db.TimeSlots.AsNoTracking()
            .Where(t => t.DoctorId == seed.DoctorId && t.StartTime > DateTime.UtcNow)
            .OrderBy(t => t.StartTime)
            .FirstAsync());

        var booking = await _fx.SendAsync(new CreateAppointmentCommand
        {
            PatientId = seed.PatientId,
            ServiceId = seed.ServiceId,
            TimeSlotId = slot.Id
        });
        Assert.True(booking.IsSuccess, booking.Error?.Message);

        var second = await _fx.SendAsync(new GenerateTimeSlotsCommand());

        Assert.True(second.IsSuccess, second.Error?.Message);
        Assert.Equal(afterFirst, await CountSlotsAsync(seed.DoctorId));

        var reloaded = await _fx.QueryDbAsync(db => db.TimeSlots.AsNoTracking().SingleAsync(t => t.Id == slot.Id));
        Assert.Equal(TimeSlotStatus.Booked, reloaded.Status);
        Assert.Equal(booking.Value, reloaded.AppointmentId);
    }

    [Fact]
    public async Task Generate_SkipsInactiveDoctorsDoctorsWithoutOfficeAndDoctorsWithoutSchedule()
    {
        var inactive = await TestData.SeedAsync(_fx, doctorActive: false);
        var noOffice = await TestData.SeedAsync(_fx, doctorHasOffice: false);
        var noSchedule = await TestData.SeedAsync(_fx);

        await AddScheduleForEveryDayAsync(inactive.DoctorId);
        await AddScheduleForEveryDayAsync(noOffice.DoctorId);

        var result = await _fx.SendAsync(new GenerateTimeSlotsCommand());

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(0, await CountSlotsAsync(inactive.DoctorId));
        Assert.Equal(0, await CountSlotsAsync(noOffice.DoctorId));
        Assert.Equal(0, await CountSlotsAsync(noSchedule.DoctorId));
    }

    [Fact]
    public async Task AvailableSlotsQuery_ReturnsOnlyAvailableSlotsInRangeOrderedByStart()
    {
        var seed = await TestData.SeedAsync(_fx);
        var from = TestData.FutureSlotStart(daysAhead: 30, hour: 8);
        await TestData.AddSlotAsync(_fx, seed, from.AddMinutes(40));
        await TestData.AddSlotAsync(_fx, seed, from);
        await TestData.AddSlotAsync(_fx, seed, from.AddMinutes(20), TimeSlotStatus.Booked);
        await TestData.AddSlotAsync(_fx, seed, from.AddMinutes(60), TimeSlotStatus.Blocked);
        await TestData.AddSlotAsync(_fx, seed, from.AddHours(5)); // outside the range

        var result = await _fx.SendAsync(new GetAvailableTimeSlotsQuery
        {
            DoctorId = seed.DoctorId,
            FromDate = from,
            ToDate = from.AddHours(2)
        });

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(new[] { from, from.AddMinutes(40) }, result.Value.Select(s => s.StartTime).ToArray());
    }

    [Fact]
    [Trait("Bug", "generates-slots-in-the-past")]
    public async Task Generate_ShouldNotCreateSlotsThatAlreadyStarted()
    {
        var seed = await TestData.SeedAsync(_fx);
        await AddScheduleForEveryDayAsync(seed.DoctorId);

        await _fx.SendAsync(new GenerateTimeSlotsCommand());

        var pastSlots = await _fx.QueryDbAsync(db => db.TimeSlots
            .CountAsync(t => t.DoctorId == seed.DoctorId && t.StartTime < DateTime.UtcNow));

        Assert.Equal(0, pastSlots);
    }
}
