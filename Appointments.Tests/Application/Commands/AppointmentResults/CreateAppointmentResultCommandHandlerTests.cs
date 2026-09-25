using Appointments.Application.Commands.AppointmentResults;
using Appointments.Application.DTO_s.Response.AppointmentResult;
using Appointments.Application.Interfaces;
using Appointments.Domain.Entities;
using Appointments.Shared.Common.Errors;
using Appointments.Shared.Common.Results;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Appointments.Tests.Application.Commands.AppointmentResults;

public class CreateAppointmentResultCommandHandlerTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepository = new();
    private readonly Mock<IAppointmentResultRepository> _appointmentResultRepository = new();
    private readonly Mock<IAppointmentResultDtoBuilder> _dtoBuilder = new();
    private readonly Mock<IPdfResultGenerator> _pdfResultGenerator = new();
    private readonly Mock<IDocumentsClient> _documentsClient = new();
    private readonly Mock<IPublishEndpoint> _publishEndpoint = new();
    private readonly Mock<ILogger<CreateAppointmentResultCommandHandler>> _logger = new();

    private CreateAppointmentResultCommandHandler CreateHandler()
    {
        return new CreateAppointmentResultCommandHandler(
            _appointmentRepository.Object,
            _appointmentResultRepository.Object,
            _dtoBuilder.Object,
            _pdfResultGenerator.Object,
            _documentsClient.Object,
            _logger.Object,
            _publishEndpoint.Object);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldCreateAppointmentResult()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var pdfDocumentId = Guid.NewGuid();

        var appointment = new Appointment
        {
            Id = appointmentId,
            PatientId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            Status = AppointmentStatus.Scheduled
        };

        var command = new CreateAppointmentResultCommand
        {
            AppointmentId = appointmentId,
            Complaints = "Headache",
            Conclusion = "Migraine",
            Recommendations = "Rest and drink water"
        };

        var dto = new AppointmentResultDto
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointmentId,
            PatientFullName = "Danya Danyonov",
            PatientEmail = "danya@gmail.com",
            DoctorFullName = "Doctor Doctorov",
            DoctorSpecialization = "Neurology",
            ServiceName = "Consultation",
            Complaints = command.Complaints,
            Conclusion = command.Conclusion,
            Recommendations = command.Recommendations,
            ResultDate = DateTime.UtcNow
        };

        _appointmentRepository
            .Setup(x => x.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Appointment>.Success(appointment));

        _appointmentResultRepository
            .Setup(x => x.GetByAppointmentIdAsync(
                appointmentId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResult>.Failure(
                AppointmentResultErrors.NotFound(appointmentId)));

        AppointmentResult? createdResult = null;

        _appointmentResultRepository
            .Setup(x => x.AddAsync(
                It.IsAny<AppointmentResult>(),
                It.IsAny<CancellationToken>()))
            .Callback<AppointmentResult, CancellationToken>((r, _) => createdResult = r)
            .ReturnsAsync((AppointmentResult r, CancellationToken _) =>
                Result<AppointmentResult>.Success(r));

        _appointmentRepository
            .Setup(x => x.UpdateAsync(
                appointment,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _dtoBuilder
            .Setup(x => x.BuildAsync(
                It.IsAny<AppointmentResult>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResultDto>.Success(dto));

        var pdfBytes = new byte[] { 1, 2, 3 };

        _pdfResultGenerator
            .Setup(x => x.Generate(dto))
            .Returns(pdfBytes);

        _documentsClient
            .Setup(x => x.UploadAppointmentResultPdfAsync(
                It.IsAny<Guid>(),
                pdfBytes,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pdfDocumentId);

        _appointmentResultRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(createdResult);
        Assert.Equal(createdResult!.Id, result.Value);

        Assert.Equal(AppointmentStatus.Completed, appointment.Status);

        _appointmentResultRepository.Verify(
            x => x.AddAsync(
                It.Is<AppointmentResult>(r =>
                    r.AppointmentId == appointmentId &&
                    r.Complaints == command.Complaints &&
                    r.Conclusion == command.Conclusion &&
                    r.Recommendations == command.Recommendations),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _appointmentRepository.Verify(
            x => x.UpdateAsync(
                It.Is<Appointment>(a =>
                    a.Id == appointmentId &&
                    a.Status == AppointmentStatus.Completed),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _pdfResultGenerator.Verify(
            x => x.Generate(dto),
            Times.Once);

        _documentsClient.Verify(
            x => x.UploadAppointmentResultPdfAsync(
                createdResult!.Id,
                pdfBytes,
                $"appointment-result-{createdResult!.Id}.pdf",
                It.IsAny<CancellationToken>()),
            Times.Once);

        _appointmentResultRepository.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}