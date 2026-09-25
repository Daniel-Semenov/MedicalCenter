using Appointments.Application.Commands.AppointmentResults;
using Appointments.Application.DTO_s.Response.AppointmentResult;
using Appointments.Application.Interfaces;
using Appointments.Domain.Entities;
using Appointments.Shared.Common.Errors;
using Appointments.Shared.Common.Results;
using MassTransit;
using Messaging.Contracts.Events.AppointmentResults;
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

    private static CreateAppointmentResultCommand CreateCommand(Guid appointmentId) => new()
    {
        AppointmentId = appointmentId,
        Complaints = "Headache",
        Conclusion = "Migraine",
        Recommendations = "Rest and drink water"
    };

    private static AppointmentResultDto CreateDto(Guid appointmentId) => new()
    {
        Id = Guid.NewGuid(),
        AppointmentId = appointmentId,
        PatientFullName = "Danya Danyonov",
        PatientEmail = "danya@gmail.com",
        DoctorFullName = "Doctor Doctorov",
        DoctorSpecialization = "Neurology",
        ServiceName = "Consultation",
        Complaints = "Headache",
        Conclusion = "Migraine",
        Recommendations = "Rest and drink water",
        ResultDate = DateTime.UtcNow
    };

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

        var command = CreateCommand(appointmentId);
        var dto = CreateDto(appointmentId);

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

        _publishEndpoint.Verify(
            x => x.Publish(It.IsAny<AppointmentResultCreated>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _appointmentResultRepository.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAppointmentNotFound_ShouldReturnRelatedAppointmentNotFoundError()
    {
        var appointmentId = Guid.NewGuid();
        var command = CreateCommand(appointmentId);

        _appointmentRepository
            .Setup(x => x.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Appointment>.Failure(AppointmentErrors.NotFound(appointmentId)));

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("APPOINTMENT_RESULT_RELATED_APPOINTMENT_NOT_FOUND", result.Error!.Code);

        _appointmentResultRepository.Verify(
            x => x.GetByAppointmentIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _appointmentResultRepository.Verify(
            x => x.AddAsync(It.IsAny<AppointmentResult>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenAppointmentResultAlreadyExists_ShouldReturnAlreadyExistsError()
    {
        var appointmentId = Guid.NewGuid();
        var command = CreateCommand(appointmentId);
        var appointment = new Appointment { Id = appointmentId, Status = AppointmentStatus.Scheduled };
        var existingAppointmentResult = new AppointmentResult { Id = Guid.NewGuid(), AppointmentId = appointmentId };

        _appointmentRepository
            .Setup(x => x.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Appointment>.Success(appointment));

        _appointmentResultRepository
            .Setup(x => x.GetByAppointmentIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResult>.Success(existingAppointmentResult));

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("APPOINTMENT_RESULT_ALREADY_EXISTS", result.Error!.Code);

        _appointmentResultRepository.Verify(
            x => x.AddAsync(It.IsAny<AppointmentResult>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _appointmentRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenAddAsyncFails_ShouldReturnFailureAndNotUpdateAppointment()
    {
        var appointmentId = Guid.NewGuid();
        var command = CreateCommand(appointmentId);
        var appointment = new Appointment { Id = appointmentId, Status = AppointmentStatus.Scheduled };
        var addError = new Error("SOME_ERROR", "add failed", 400);

        _appointmentRepository
            .Setup(x => x.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Appointment>.Success(appointment));

        _appointmentResultRepository
            .Setup(x => x.GetByAppointmentIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResult>.Failure(AppointmentResultErrors.NotFound(appointmentId)));

        _appointmentResultRepository
            .Setup(x => x.AddAsync(It.IsAny<AppointmentResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResult>.Failure(addError));

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(addError, result.Error);

        _appointmentRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUpdateAppointmentFails_ShouldReturnFailureAndNotBuildDto()
    {
        var appointmentId = Guid.NewGuid();
        var command = CreateCommand(appointmentId);
        var appointment = new Appointment { Id = appointmentId, Status = AppointmentStatus.Scheduled };
        var updateError = new Error("UPDATE_FAILED", "update failed", 400);

        _appointmentRepository
            .Setup(x => x.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Appointment>.Success(appointment));

        _appointmentResultRepository
            .Setup(x => x.GetByAppointmentIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResult>.Failure(AppointmentResultErrors.NotFound(appointmentId)));

        _appointmentResultRepository
            .Setup(x => x.AddAsync(It.IsAny<AppointmentResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppointmentResult r, CancellationToken _) => Result<AppointmentResult>.Success(r));

        _appointmentRepository
            .Setup(x => x.UpdateAsync(appointment, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(updateError));

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(updateError, result.Error);

        _dtoBuilder.Verify(
            x => x.BuildAsync(It.IsAny<AppointmentResult>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDtoBuilderFails_ShouldReturnFailureAndNotGeneratePdf()
    {
        var appointmentId = Guid.NewGuid();
        var command = CreateCommand(appointmentId);
        var appointment = new Appointment { Id = appointmentId, Status = AppointmentStatus.Scheduled };
        var dtoError = new Error("DTO_BUILD_FAILED", "dto failed", 400);

        _appointmentRepository
            .Setup(x => x.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Appointment>.Success(appointment));

        _appointmentResultRepository
            .Setup(x => x.GetByAppointmentIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResult>.Failure(AppointmentResultErrors.NotFound(appointmentId)));

        _appointmentResultRepository
            .Setup(x => x.AddAsync(It.IsAny<AppointmentResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppointmentResult r, CancellationToken _) => Result<AppointmentResult>.Success(r));

        _appointmentRepository
            .Setup(x => x.UpdateAsync(appointment, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _dtoBuilder
            .Setup(x => x.BuildAsync(It.IsAny<AppointmentResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResultDto>.Failure(dtoError));

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(dtoError, result.Error);

        _pdfResultGenerator.Verify(x => x.Generate(It.IsAny<AppointmentResultDto>()), Times.Never);
        _documentsClient.Verify(
            x => x.UploadAppointmentResultPdfAsync(
                It.IsAny<Guid>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSaveChangesFails_ShouldDeleteOrphanedPdfAndReturnFailure()
    {
        var appointmentId = Guid.NewGuid();
        var pdfDocumentId = Guid.NewGuid();
        var command = CreateCommand(appointmentId);
        var appointment = new Appointment { Id = appointmentId, Status = AppointmentStatus.Scheduled };
        var dto = CreateDto(appointmentId);
        var saveError = new Error("SAVE_FAILED", "save failed", 500);

        _appointmentRepository
            .Setup(x => x.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Appointment>.Success(appointment));

        _appointmentResultRepository
            .Setup(x => x.GetByAppointmentIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResult>.Failure(AppointmentResultErrors.NotFound(appointmentId)));

        _appointmentResultRepository
            .Setup(x => x.AddAsync(It.IsAny<AppointmentResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppointmentResult r, CancellationToken _) => Result<AppointmentResult>.Success(r));

        _appointmentRepository
            .Setup(x => x.UpdateAsync(appointment, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _dtoBuilder
            .Setup(x => x.BuildAsync(It.IsAny<AppointmentResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResultDto>.Success(dto));

        _pdfResultGenerator
            .Setup(x => x.Generate(dto))
            .Returns(new byte[] { 1, 2, 3 });

        _documentsClient
            .Setup(x => x.UploadAppointmentResultPdfAsync(
                It.IsAny<Guid>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pdfDocumentId);

        _appointmentResultRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(saveError));

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(saveError, result.Error);

        _documentsClient.Verify(
            x => x.DeleteAsync(pdfDocumentId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPublishThrows_ShouldDeleteOrphanedPdfAndRethrow()
    {
        var appointmentId = Guid.NewGuid();
        var pdfDocumentId = Guid.NewGuid();
        var command = CreateCommand(appointmentId);
        var appointment = new Appointment { Id = appointmentId, Status = AppointmentStatus.Scheduled };
        var dto = CreateDto(appointmentId);

        _appointmentRepository
            .Setup(x => x.GetByIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Appointment>.Success(appointment));

        _appointmentResultRepository
            .Setup(x => x.GetByAppointmentIdAsync(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResult>.Failure(AppointmentResultErrors.NotFound(appointmentId)));

        _appointmentResultRepository
            .Setup(x => x.AddAsync(It.IsAny<AppointmentResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppointmentResult r, CancellationToken _) => Result<AppointmentResult>.Success(r));

        _appointmentRepository
            .Setup(x => x.UpdateAsync(appointment, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _dtoBuilder
            .Setup(x => x.BuildAsync(It.IsAny<AppointmentResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResultDto>.Success(dto));

        _pdfResultGenerator
            .Setup(x => x.Generate(dto))
            .Returns(new byte[] { 1, 2, 3 });

        _documentsClient
            .Setup(x => x.UploadAppointmentResultPdfAsync(
                It.IsAny<Guid>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pdfDocumentId);

        _publishEndpoint
            .Setup(x => x.Publish(It.IsAny<AppointmentResultCreated>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("broker down"));

        var handler = CreateHandler();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));

        _documentsClient.Verify(
            x => x.DeleteAsync(pdfDocumentId, It.IsAny<CancellationToken>()), Times.Once);

        _appointmentResultRepository.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}