using Appointments.Application.Commands.Appointments;
using Appointments.Application.Commands.AppointmentResults;
using Appointments.Application.Queries.AppointmentResults;
using Appointments.Domain.Entities;
using Messaging.Contracts.Events.AppointmentResults;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Appointments.Tests.Integration;

[Collection(PostgresCollection.Name)]
public class AppointmentResultIntegrationTests
{
    private readonly PostgresFixture _fx;

    public AppointmentResultIntegrationTests(PostgresFixture fx) => _fx = fx;

    private async Task<(Seed Seed, Guid AppointmentId)> BookedAppointmentAsync()
    {
        var seed = await TestData.SeedAsync(_fx);
        var slotId = await TestData.AddSlotAsync(_fx, seed, TestData.FutureSlotStart());

        var booking = await _fx.SendAsync(new CreateAppointmentCommand
        {
            PatientId = seed.PatientId,
            ServiceId = seed.ServiceId,
            TimeSlotId = slotId
        });

        Assert.True(booking.IsSuccess, booking.Error?.Message);
        return (seed, booking.Value);
    }

    private static CreateAppointmentResultCommand CreateCommand(Guid appointmentId) => new()
    {
        AppointmentId = appointmentId,
        Complaints = "Headache",
        Conclusion = "Migraine",
        Recommendations = "Rest and drink water"
    };

    private Task<AppointmentResult> GetResultAsync(Guid id) =>
        _fx.QueryDbAsync(db => db.AppointmentResults.AsNoTracking().SingleAsync(r => r.Id == id));


    [Fact]
    public async Task CreateResult_PersistsResultCompletesAppointmentUploadsPdfAndPublishesEvent()
    {
        var (seed, appointmentId) = await BookedAppointmentAsync();

        var result = await _fx.SendAsync(CreateCommand(appointmentId));

        Assert.True(result.IsSuccess, result.Error?.Message);

        var stored = await GetResultAsync(result.Value);
        Assert.Equal(appointmentId, stored.AppointmentId);
        Assert.Equal("Headache", stored.Complaints);
        Assert.Equal("Migraine", stored.Conclusion);
        Assert.NotNull(stored.PdfDocumentId);

        var appointment = await _fx.QueryDbAsync(db =>
            db.Appointments.AsNoTracking().SingleAsync(a => a.Id == appointmentId));
        Assert.Equal(AppointmentStatus.Completed, appointment.Status);

        _fx.Documents.Verify(d => d.UploadAppointmentResultPdfAsync(
                result.Value,
                It.Is<byte[]>(b => b.Length > 0),
                $"appointment-result-{result.Value}.pdf",
                It.IsAny<CancellationToken>()),
            Times.Once);

        _fx.Publish.Verify(p => p.Publish(
                It.Is<AppointmentResultCreated>(e =>
                    e.AppointmentId == appointmentId &&
                    e.AppointmentResultId == result.Value &&
                    e.PatientId == seed.PatientId &&
                    e.PatientEmail == seed.PatientEmail &&
                    e.PdfDocumentId == stored.PdfDocumentId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateResult_WhenResultAlreadyExists_ReturnsAlreadyExists()
    {
        var (_, appointmentId) = await BookedAppointmentAsync();
        var first = await _fx.SendAsync(CreateCommand(appointmentId));
        Assert.True(first.IsSuccess, first.Error?.Message);

        var second = await _fx.SendAsync(CreateCommand(appointmentId));

        Assert.True(second.IsFailure);
        Assert.Equal("APPOINTMENT_RESULT_ALREADY_EXISTS", second.Error!.Code);
        Assert.Equal(1, await _fx.QueryDbAsync(db =>
            db.AppointmentResults.CountAsync(r => r.AppointmentId == appointmentId)));
    }

    [Fact]
    public async Task CreateResult_WhenAppointmentDoesNotExist_ReturnsRelatedAppointmentNotFound()
    {
        var result = await _fx.SendAsync(CreateCommand(Guid.NewGuid()));

        Assert.True(result.IsFailure);
        Assert.Equal("APPOINTMENT_RESULT_RELATED_APPOINTMENT_NOT_FOUND", result.Error!.Code);
    }

    [Fact]
    public async Task CreateResult_WhenTextIsTooLong_FailsValidationAndStoresNothing()
    {
        var (_, appointmentId) = await BookedAppointmentAsync();
        var command = CreateCommand(appointmentId);
        command.Complaints = new string('x', 2001);

        var result = await _fx.SendAsync(command);

        Assert.True(result.IsFailure);
        Assert.Equal("VALIDATION_ERROR", result.Error!.Code);
        Assert.Equal(0, await _fx.QueryDbAsync(db =>
            db.AppointmentResults.CountAsync(r => r.AppointmentId == appointmentId)));
    }

    [Fact]
    [Trait("Bug", "result-for-cancelled-appointment")]
    public async Task CreateResult_ForCancelledAppointment_ShouldBeRejected()
    {
        var (_, appointmentId) = await BookedAppointmentAsync();
        var cancel = await _fx.SendAsync(new CancelAppointmentCommand { Id = appointmentId });
        Assert.True(cancel.IsSuccess, cancel.Error?.Message);

        var result = await _fx.SendAsync(CreateCommand(appointmentId));

        Assert.True(result.IsFailure);
        var appointment = await _fx.QueryDbAsync(db =>
            db.Appointments.AsNoTracking().SingleAsync(a => a.Id == appointmentId));
        Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
    }


    [Fact]
    public async Task UpdateResult_RewritesFieldsReplacesPdfAndDeletesPreviousDocument()
    {
        var (_, appointmentId) = await BookedAppointmentAsync();
        var created = await _fx.SendAsync(CreateCommand(appointmentId));
        Assert.True(created.IsSuccess, created.Error?.Message);
        var previousPdfId = (await GetResultAsync(created.Value)).PdfDocumentId!.Value;

        var update = await _fx.SendAsync(new UpdateAppointmentResultCommand
        {
            Id = created.Value,
            Complaints = "Cough",
            Conclusion = "Bronchitis",
            Recommendations = "Antibiotics"
        });

        Assert.True(update.IsSuccess, update.Error?.Message);

        var stored = await GetResultAsync(created.Value);
        Assert.Equal("Cough", stored.Complaints);
        Assert.Equal("Bronchitis", stored.Conclusion);
        Assert.Equal("Antibiotics", stored.Recommendations);
        Assert.NotNull(stored.UpdatedAt);
        Assert.NotNull(stored.PdfDocumentId);
        Assert.NotEqual(previousPdfId, stored.PdfDocumentId);

        _fx.Documents.Verify(d => d.DeleteAsync(previousPdfId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateResult_WhenDeletingPreviousPdfFails_StillSucceeds()
    {
        var (_, appointmentId) = await BookedAppointmentAsync();
        var created = await _fx.SendAsync(CreateCommand(appointmentId));
        var previousPdfId = (await GetResultAsync(created.Value)).PdfDocumentId!.Value;

        _fx.Documents
            .Setup(d => d.DeleteAsync(previousPdfId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("documents unreachable"));

        var update = await _fx.SendAsync(new UpdateAppointmentResultCommand
        {
            Id = created.Value,
            Complaints = "Cough",
            Conclusion = "Bronchitis",
            Recommendations = "Antibiotics"
        });

        Assert.True(update.IsSuccess, update.Error?.Message);
        Assert.Equal("Bronchitis", (await GetResultAsync(created.Value)).Conclusion);
    }

    [Fact]
    public async Task UpdateResult_WhenNotFound_ReturnsNotFound()
    {
        var result = await _fx.SendAsync(new UpdateAppointmentResultCommand
        {
            Id = Guid.NewGuid(),
            Complaints = "a",
            Conclusion = "b",
            Recommendations = "c"
        });

        Assert.True(result.IsFailure);
        Assert.Equal("APPOINTMENT_RESULT_NOT_FOUND", result.Error!.Code);
    }


    [Fact]
    public async Task DownloadResult_WhenStoredPdfAvailable_ReturnsStoredBytes()
    {
        var (_, appointmentId) = await BookedAppointmentAsync();
        var created = await _fx.SendAsync(CreateCommand(appointmentId));
        var pdfId = (await GetResultAsync(created.Value)).PdfDocumentId!.Value;
        var stored = new byte[] { 9, 9, 9 };

        _fx.Documents
            .Setup(d => d.DownloadAsync(pdfId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(stored);

        var result = await _fx.SendAsync(new DownloadAppointmentResultQuery { Id = created.Value });

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(stored, result.Value.Content);
        Assert.Equal("application/pdf", result.Value.ContentType);
    }

    [Fact]
    public async Task DownloadResult_WhenDocumentsServiceFails_RegeneratesPdfOnTheFly()
    {
        var (_, appointmentId) = await BookedAppointmentAsync();
        var created = await _fx.SendAsync(CreateCommand(appointmentId));
        var pdfId = (await GetResultAsync(created.Value)).PdfDocumentId!.Value;

        _fx.Documents
            .Setup(d => d.DownloadAsync(pdfId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("documents down"));

        var result = await _fx.SendAsync(new DownloadAppointmentResultQuery { Id = created.Value });

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.StartsWith("%PDF", System.Text.Encoding.ASCII.GetString(result.Value.Content, 0, 4));
    }

    [Fact]
    public async Task DownloadResult_WhenNotFound_ReturnsNotFound()
    {
        var result = await _fx.SendAsync(new DownloadAppointmentResultQuery { Id = Guid.NewGuid() });

        Assert.True(result.IsFailure);
        Assert.Equal("APPOINTMENT_RESULT_NOT_FOUND", result.Error!.Code);
    }
}
