using Appointments.Application.Commands.Appointments;
using Appointments.Domain.Entities;
using Appointments.Shared.Common.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Appointments.Infrastructure.Db;
using Xunit;

namespace Appointments.Tests.Integration;

[Collection(PostgresCollection.Name)]
public class AppointmentBookingIntegrationTests
{
    private readonly PostgresFixture _fx;

    public AppointmentBookingIntegrationTests(PostgresFixture fx) => _fx = fx;

    private async Task<Guid> BookAsync(Seed seed, Guid slotId)
    {
        var result = await _fx.SendAsync(new CreateAppointmentCommand
        {
            PatientId = seed.PatientId,
            ServiceId = seed.ServiceId,
            TimeSlotId = slotId
        }); 

        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value;
    }

    private Task<TimeSlot> GetSlotAsync(Guid id) =>
        _fx.QueryDbAsync(db => db.TimeSlots.AsNoTracking().SingleAsync(t => t.Id == id));

    private Task<Appointment> GetAppointmentAsync(Guid id) =>
        _fx.QueryDbAsync(db => db.Appointments.AsNoTracking().SingleAsync(a => a.Id == id));


    [Fact]
    public async Task CreateAppointment_WhenSlotAvailable_PersistsAppointmentAndBooksSlot()
    {
        var seed = await TestData.SeedAsync(_fx);
        var slotId = await TestData.AddSlotAsync(_fx, seed, TestData.FutureSlotStart());

        var appointmentId = await BookAsync(seed, slotId);

        var appointment = await GetAppointmentAsync(appointmentId);
        var slot = await GetSlotAsync(slotId);

        Assert.Equal(seed.PatientId, appointment.PatientId);
        Assert.Equal(seed.DoctorId, appointment.DoctorId);
        Assert.Equal(seed.OfficeId, appointment.OfficeId);
        Assert.Equal(seed.SpecializationId, appointment.SpecializationId);
        Assert.Equal(AppointmentStatus.Scheduled, appointment.Status);
        Assert.Equal(slot.StartTime, appointment.StartTime);
        Assert.Equal(slot.EndTime, appointment.EndTime);

        Assert.Equal(TimeSlotStatus.Booked, slot.Status);
        Assert.Equal(appointmentId, slot.AppointmentId);
    }

    [Fact]
    public async Task CreateAppointment_WhenSlotAlreadyBooked_ReturnsNotAvailable()
    {
        var seed = await TestData.SeedAsync(_fx);
        var slotId = await TestData.AddSlotAsync(_fx, seed, TestData.FutureSlotStart());
        await BookAsync(seed, slotId);

        var second = await _fx.SendAsync(new CreateAppointmentCommand
        {
            PatientId = Guid.NewGuid(),
            ServiceId = seed.ServiceId,
            TimeSlotId = slotId
        });

        Assert.True(second.IsFailure);
        Assert.Equal("TIME_SLOT_NOT_AVAILABLE", second.Error!.Code);
        Assert.Equal(1, await _fx.QueryDbAsync(db => db.Appointments.CountAsync(a => a.DoctorId == seed.DoctorId)));
    }

    [Fact]
    public async Task CreateAppointment_WhenSlotDoesNotExist_ReturnsNotFound()
    {
        var result = await _fx.SendAsync(new CreateAppointmentCommand
        {
            PatientId = Guid.NewGuid(),
            ServiceId = Guid.NewGuid(),
            TimeSlotId = Guid.NewGuid()
        });

        Assert.True(result.IsFailure);
        Assert.Equal("TIME_SLOT_NOT_FOUND", result.Error!.Code);
    }

    [Fact]
    public async Task CreateAppointment_WithEmptyIds_FailsValidationThroughPipeline()
    {
        var result = await _fx.SendAsync(new CreateAppointmentCommand());

        Assert.True(result.IsFailure);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
        Assert.Equal(400, result.Error.StatusCode);
    }

    [Fact]
    [Trait("Bug", "concurrent-double-booking")]
    public async Task CreateAppointment_WhenTwoRequestsRaceForSameSlot_ExactlyOneWinsAndTheOtherGetsConflict()
    {
        var seed = await TestData.SeedAsync(_fx);
        var slotId = await TestData.AddSlotAsync(_fx, seed, TestData.FutureSlotStart());

        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        async Task<Result<Guid>> Book(Guid patientId)
        {
            await gate.Task;
            return await _fx.SendAsync(new CreateAppointmentCommand
            {
                PatientId = patientId,
                ServiceId = seed.ServiceId,
                TimeSlotId = slotId
            });
        }

        var t1 = Book(Guid.NewGuid());
        var t2 = Book(Guid.NewGuid());
        gate.SetResult();

        var results = await Task.WhenAll(t1, t2);

        Assert.Equal(1, results.Count(r => r.IsSuccess));
        var loser = results.Single(r => r.IsFailure);
        Assert.Equal(409, loser.Error!.StatusCode);

        Assert.Equal(1, await _fx.QueryDbAsync(db => db.Appointments.CountAsync(a => a.DoctorId == seed.DoctorId)));
    }

    [Fact]
    [Trait("Bug", "rowversion-not-generated-on-npgsql")]
    public async Task TimeSlot_WhenTwoContextsUpdateSameStaleSlot_SecondSaveThrowsConcurrencyException()
    {
        var seed = await TestData.SeedAsync(_fx);
        var slotId = await TestData.AddSlotAsync(_fx, seed, TestData.FutureSlotStart());

        using var scope1 = _fx.Provider.CreateScope();
        using var scope2 = _fx.Provider.CreateScope();
        var db1 = scope1.ServiceProvider.GetRequiredService<AppointmentsDbContext>();
        var db2 = scope2.ServiceProvider.GetRequiredService<AppointmentsDbContext>();

        var slot1 = await db1.TimeSlots.SingleAsync(t => t.Id == slotId);
        var slot2 = await db2.TimeSlots.SingleAsync(t => t.Id == slotId);

        slot1.Status = TimeSlotStatus.Booked;
        await db1.SaveChangesAsync();

        slot2.Status = TimeSlotStatus.Booked;

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => db2.SaveChangesAsync());
    }

    [Fact]
    [Trait("Bug", "fallback-specialization-guid")]
    public async Task CreateAppointment_WhenServiceSnapshotMissing_ReturnsFailureInsteadOfRandomSpecialization()
    {
        var seed = await TestData.SeedAsync(_fx, withService: false);
        var slotId = await TestData.AddSlotAsync(_fx, seed, TestData.FutureSlotStart());

        var result = await _fx.SendAsync(new CreateAppointmentCommand
        {
            PatientId = seed.PatientId,
            ServiceId = seed.ServiceId,
            TimeSlotId = slotId
        });

        Assert.True(result.IsFailure);
        Assert.Equal(0, await _fx.QueryDbAsync(db => db.Appointments.CountAsync(a => a.DoctorId == seed.DoctorId)));
    }


    [Fact]
    public async Task CancelAppointment_MarksCancelledAndFreesSlot()
    {
        var seed = await TestData.SeedAsync(_fx);
        var slotId = await TestData.AddSlotAsync(_fx, seed, TestData.FutureSlotStart());
        var appointmentId = await BookAsync(seed, slotId);

        var result = await _fx.SendAsync(new CancelAppointmentCommand { Id = appointmentId });

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(AppointmentStatus.Cancelled, (await GetAppointmentAsync(appointmentId)).Status);

        var slot = await GetSlotAsync(slotId);
        Assert.Equal(TimeSlotStatus.Available, slot.Status);
        Assert.Null(slot.AppointmentId);
    }

    [Fact]
    public async Task CancelAppointment_WhenAlreadyCancelled_ReturnsInvalidStatusTransition()
    {
        var seed = await TestData.SeedAsync(_fx);
        var slotId = await TestData.AddSlotAsync(_fx, seed, TestData.FutureSlotStart());
        var appointmentId = await BookAsync(seed, slotId);

        await _fx.SendAsync(new CancelAppointmentCommand { Id = appointmentId });
        var second = await _fx.SendAsync(new CancelAppointmentCommand { Id = appointmentId });

        Assert.True(second.IsFailure);
        Assert.Equal("APPOINTMENT_INVALID_STATUS_TRANSITION", second.Error!.Code);
    }

    [Fact]
    public async Task CancelAppointment_WhenNotFound_ReturnsNotFound()
    {
        var result = await _fx.SendAsync(new CancelAppointmentCommand { Id = Guid.NewGuid() });

        Assert.True(result.IsFailure);
        Assert.Equal("APPOINTMENT_NOT_FOUND", result.Error!.Code);
    }

    [Fact]
    public async Task CancelAppointment_WithEmptyId_FailsValidationThroughPipeline()
    {
        var result = await _fx.SendAsync(new CancelAppointmentCommand { Id = Guid.Empty });

        Assert.True(result.IsFailure);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
    }

    [Fact]
    [Trait("Bug", "cancelled-appointment-blocks-slot-via-unique-index")]
    public async Task CancelThenRebookSameSlot_ShouldSucceed()
    {
        var seed = await TestData.SeedAsync(_fx);
        var slotId = await TestData.AddSlotAsync(_fx, seed, TestData.FutureSlotStart());

        var first = await BookAsync(seed, slotId);
        var cancel = await _fx.SendAsync(new CancelAppointmentCommand { Id = first });
        Assert.True(cancel.IsSuccess, cancel.Error?.Message);

        var rebook = await _fx.SendAsync(new CreateAppointmentCommand
        {
            PatientId = Guid.NewGuid(),
            ServiceId = seed.ServiceId,
            TimeSlotId = slotId
        });

        Assert.True(rebook.IsSuccess, rebook.Error?.Message);
    }


    [Fact]
    public async Task RescheduleAppointment_MovesBookingToNewSlotAndFreesOldOne()
    {
        var seed = await TestData.SeedAsync(_fx);
        var oldSlotId = await TestData.AddSlotAsync(_fx, seed, TestData.FutureSlotStart(hour: 9));
        var newSlotId = await TestData.AddSlotAsync(_fx, seed, TestData.FutureSlotStart(hour: 10));
        var appointmentId = await BookAsync(seed, oldSlotId);

        var result = await _fx.SendAsync(new RescheduleAppointmentCommand
        {
            Id = appointmentId,
            NewTimeSlotId = newSlotId
        });

        Assert.True(result.IsSuccess, result.Error?.Message);

        var oldSlot = await GetSlotAsync(oldSlotId);
        var newSlot = await GetSlotAsync(newSlotId);
        var appointment = await GetAppointmentAsync(appointmentId);

        Assert.Equal(TimeSlotStatus.Available, oldSlot.Status);
        Assert.Null(oldSlot.AppointmentId);
        Assert.Equal(TimeSlotStatus.Booked, newSlot.Status);
        Assert.Equal(appointmentId, newSlot.AppointmentId);
        Assert.Equal(newSlot.StartTime, appointment.StartTime);
        Assert.Equal(newSlot.EndTime, appointment.EndTime);
        Assert.Equal(AppointmentStatus.Scheduled, appointment.Status);
        Assert.NotNull(appointment.UpdatedAt);
    }

    [Fact]
    public async Task RescheduleAppointment_ToSlotOfAnotherDoctor_ReturnsDoctorMismatch()
    {
        var seed = await TestData.SeedAsync(_fx);
        var otherDoctor = await TestData.SeedAsync(_fx);
        var slotId = await TestData.AddSlotAsync(_fx, seed, TestData.FutureSlotStart(hour: 9));
        var foreignSlotId = await TestData.AddSlotAsync(_fx, otherDoctor, TestData.FutureSlotStart(hour: 9));
        var appointmentId = await BookAsync(seed, slotId);

        var result = await _fx.SendAsync(new RescheduleAppointmentCommand
        {
            Id = appointmentId,
            NewTimeSlotId = foreignSlotId
        });

        Assert.True(result.IsFailure);
        Assert.Equal("TIME_SLOT_DOCTOR_MISMATCH", result.Error!.Code);
        Assert.Equal(slotId, (await GetSlotAsync(slotId)).Id);
        Assert.Equal(TimeSlotStatus.Booked, (await GetSlotAsync(slotId)).Status);
    }

    [Fact]
    public async Task RescheduleAppointment_ToBookedSlot_ReturnsNotAvailableAndKeepsOldBooking()
    {
        var seed = await TestData.SeedAsync(_fx);
        var slotId = await TestData.AddSlotAsync(_fx, seed, TestData.FutureSlotStart(hour: 9));
        var blockedSlotId = await TestData.AddSlotAsync(_fx, seed, TestData.FutureSlotStart(hour: 10), TimeSlotStatus.Booked);
        var appointmentId = await BookAsync(seed, slotId);

        var result = await _fx.SendAsync(new RescheduleAppointmentCommand
        {
            Id = appointmentId,
            NewTimeSlotId = blockedSlotId
        });

        Assert.True(result.IsFailure);
        Assert.Equal("TIME_SLOT_NOT_AVAILABLE", result.Error!.Code);
        Assert.Equal(TimeSlotStatus.Booked, (await GetSlotAsync(slotId)).Status);
        Assert.Equal(slotId, (await _fx.QueryDbAsync(db =>
            db.TimeSlots.AsNoTracking().Where(t => t.AppointmentId == appointmentId).Select(t => t.Id).SingleAsync())));
    }

    [Fact]
    public async Task RescheduleAppointment_WhenAppointmentCancelled_ReturnsInvalidStatusTransition()
    {
        var seed = await TestData.SeedAsync(_fx);
        var slotId = await TestData.AddSlotAsync(_fx, seed, TestData.FutureSlotStart(hour: 9));
        var newSlotId = await TestData.AddSlotAsync(_fx, seed, TestData.FutureSlotStart(hour: 10));
        var appointmentId = await BookAsync(seed, slotId);
        await _fx.SendAsync(new CancelAppointmentCommand { Id = appointmentId });

        var result = await _fx.SendAsync(new RescheduleAppointmentCommand
        {
            Id = appointmentId,
            NewTimeSlotId = newSlotId
        });

        Assert.True(result.IsFailure);
        Assert.Equal("APPOINTMENT_INVALID_STATUS_TRANSITION", result.Error!.Code);
        Assert.Equal(TimeSlotStatus.Available, (await GetSlotAsync(newSlotId)).Status);
    }
}
