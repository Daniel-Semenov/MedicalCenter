using Appointments.Application.Commands.AppointmentResults;
using Appointments.Application.DTO_s.Response.AppointmentResult;
using Appointments.Application.Interfaces;
using Appointments.Domain.Entities;
using Appointments.Shared.Common.Errors;
using Appointments.Shared.Common.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Appointments.Tests.Application.Commands.AppointmentResults;

public class UpdateAppointmentResultCommandHandlerTests
{
    private readonly Mock<IAppointmentResultRepository> _appointmentResultRepository = new();
    private readonly Mock<IAppointmentResultDtoBuilder> _dtoBuilder = new();
    private readonly Mock<IPdfResultGenerator> _pdfResultGenerator = new();
    private readonly Mock<IDocumentsClient> _documentsClient = new();
    private readonly Mock<ILogger<UpdateAppointmentResultCommandHandler>> _logger = new();

    private UpdateAppointmentResultCommandHandler CreateHandler()
    {
        return new UpdateAppointmentResultCommandHandler(
            _appointmentResultRepository.Object,
            _dtoBuilder.Object,
            _pdfResultGenerator.Object,
            _documentsClient.Object,
            _logger.Object);
    }

    private static UpdateAppointmentResultCommand CreateCommand(Guid id) => new()
    {
        Id = id,
        Complaints = "Cough",
        Conclusion = "Bronchitis",
        Recommendations = "Antibiotics"
    };

    private static AppointmentResult CreateExistingResult(Guid id, Guid? previousPdfDocumentId = null) => new()
    {
        Id = id,
        AppointmentId = Guid.NewGuid(),
        Complaints = "Old complaints",
        Conclusion = "Old conclusion",
        Recommendations = "Old recommendations",
        PdfDocumentId = previousPdfDocumentId
    };

    private static AppointmentResultDto CreateDto(Guid id) => new()
    {
        Id = id,
        AppointmentId = Guid.NewGuid(),
        PatientFullName = "Danya Danyonov",
        PatientEmail = "danya@gmail.com",
        DoctorFullName = "Doctor Doctorov",
        DoctorSpecialization = "Therapy",
        ServiceName = "Follow-up",
        Complaints = "Cough",
        Conclusion = "Bronchitis",
        Recommendations = "Antibiotics",
        ResultDate = DateTime.UtcNow
    };

    [Fact]
    public async Task Handle_WhenRequestIsValidAndNoPreviousPdf_ShouldUpdateAndNotDeleteAnything()
    {
        var id = Guid.NewGuid();
        var newPdfDocumentId = Guid.NewGuid();
        var command = CreateCommand(id);
        var existingResult = CreateExistingResult(id);
        var dto = CreateDto(id);

        _appointmentResultRepository
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResult>.Success(existingResult));

        _dtoBuilder
            .Setup(x => x.BuildAsync(existingResult, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResultDto>.Success(dto));

        var pdfBytes = new byte[] { 4, 5, 6 };

        _pdfResultGenerator
            .Setup(x => x.Generate(dto))
            .Returns(pdfBytes);

        _documentsClient
            .Setup(x => x.UploadAppointmentResultPdfAsync(
                id, pdfBytes, $"appointment-result-{id}.pdf", It.IsAny<CancellationToken>()))
            .ReturnsAsync(newPdfDocumentId);

        _appointmentResultRepository
            .Setup(x => x.UpdateAsync(existingResult, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _appointmentResultRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(command.Complaints, existingResult.Complaints);
        Assert.Equal(command.Conclusion, existingResult.Conclusion);
        Assert.Equal(command.Recommendations, existingResult.Recommendations);
        Assert.Equal(newPdfDocumentId, existingResult.PdfDocumentId);
        Assert.NotNull(existingResult.UpdatedAt);

        _documentsClient.Verify(
            x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValidAndHasPreviousPdf_ShouldDeletePreviousPdf()
    {
        var id = Guid.NewGuid();
        var previousPdfDocumentId = Guid.NewGuid();
        var newPdfDocumentId = Guid.NewGuid();
        var command = CreateCommand(id);
        var existingResult = CreateExistingResult(id, previousPdfDocumentId);
        var dto = CreateDto(id);

        _appointmentResultRepository
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResult>.Success(existingResult));

        _dtoBuilder
            .Setup(x => x.BuildAsync(existingResult, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResultDto>.Success(dto));

        _pdfResultGenerator
            .Setup(x => x.Generate(dto))
            .Returns(new byte[] { 1 });

        _documentsClient
            .Setup(x => x.UploadAppointmentResultPdfAsync(
                id, It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newPdfDocumentId);

        _appointmentResultRepository
            .Setup(x => x.UpdateAsync(existingResult, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _appointmentResultRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);

        _documentsClient.Verify(
            x => x.DeleteAsync(previousPdfDocumentId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAppointmentResultNotFound_ShouldReturnFailure()
    {
        var id = Guid.NewGuid();
        var command = CreateCommand(id);
        var notFoundError = AppointmentResultErrors.NotFound(id);

        _appointmentResultRepository
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResult>.Failure(notFoundError));

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(notFoundError, result.Error);

        _dtoBuilder.Verify(
            x => x.BuildAsync(It.IsAny<AppointmentResult>(), It.IsAny<CancellationToken>()), Times.Never);
        _documentsClient.Verify(
            x => x.UploadAppointmentResultPdfAsync(
                It.IsAny<Guid>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDtoBuilderFails_ShouldReturnFailureAndNotUploadPdf()
    {
        var id = Guid.NewGuid();
        var command = CreateCommand(id);
        var existingResult = CreateExistingResult(id);
        var dtoError = new Error("DTO_BUILD_FAILED", "dto failed", 400);

        _appointmentResultRepository
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResult>.Success(existingResult));

        _dtoBuilder
            .Setup(x => x.BuildAsync(existingResult, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResultDto>.Failure(dtoError));

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(dtoError, result.Error);

        _documentsClient.Verify(
            x => x.UploadAppointmentResultPdfAsync(
                It.IsAny<Guid>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _appointmentResultRepository.Verify(
            x => x.UpdateAsync(It.IsAny<AppointmentResult>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUpdateAsyncFails_ShouldDeleteOrphanedPdfAndReturnFailure()
    {
        var id = Guid.NewGuid();
        var newPdfDocumentId = Guid.NewGuid();
        var command = CreateCommand(id);
        var existingResult = CreateExistingResult(id);
        var dto = CreateDto(id);
        var updateError = new Error("UPDATE_FAILED", "update failed", 400);

        _appointmentResultRepository
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResult>.Success(existingResult));

        _dtoBuilder
            .Setup(x => x.BuildAsync(existingResult, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResultDto>.Success(dto));

        _pdfResultGenerator
            .Setup(x => x.Generate(dto))
            .Returns(new byte[] { 1 });

        _documentsClient
            .Setup(x => x.UploadAppointmentResultPdfAsync(
                id, It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newPdfDocumentId);

        _appointmentResultRepository
            .Setup(x => x.UpdateAsync(existingResult, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(updateError));

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(updateError, result.Error);

        _documentsClient.Verify(
            x => x.DeleteAsync(newPdfDocumentId, It.IsAny<CancellationToken>()), Times.Once);
        _appointmentResultRepository.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSaveChangesFails_ShouldDeleteOrphanedPdfAndReturnFailure()
    {
        var id = Guid.NewGuid();
        var newPdfDocumentId = Guid.NewGuid();
        var command = CreateCommand(id);
        var existingResult = CreateExistingResult(id);
        var dto = CreateDto(id);
        var saveError = new Error("SAVE_FAILED", "save failed", 500);

        _appointmentResultRepository
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResult>.Success(existingResult));

        _dtoBuilder
            .Setup(x => x.BuildAsync(existingResult, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResultDto>.Success(dto));

        _pdfResultGenerator
            .Setup(x => x.Generate(dto))
            .Returns(new byte[] { 1 });

        _documentsClient
            .Setup(x => x.UploadAppointmentResultPdfAsync(
                id, It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newPdfDocumentId);

        _appointmentResultRepository
            .Setup(x => x.UpdateAsync(existingResult, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _appointmentResultRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(saveError));

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(saveError, result.Error);

        _documentsClient.Verify(
            x => x.DeleteAsync(newPdfDocumentId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSaveChangesThrows_ShouldDeleteOrphanedPdfAndRethrow()
    {
        var id = Guid.NewGuid();
        var newPdfDocumentId = Guid.NewGuid();
        var command = CreateCommand(id);
        var existingResult = CreateExistingResult(id);
        var dto = CreateDto(id);

        _appointmentResultRepository
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResult>.Success(existingResult));

        _dtoBuilder
            .Setup(x => x.BuildAsync(existingResult, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResultDto>.Success(dto));

        _pdfResultGenerator
            .Setup(x => x.Generate(dto))
            .Returns(new byte[] { 1 });

        _documentsClient
            .Setup(x => x.UploadAppointmentResultPdfAsync(
                id, It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newPdfDocumentId);

        _appointmentResultRepository
            .Setup(x => x.UpdateAsync(existingResult, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _appointmentResultRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db down"));

        var handler = CreateHandler();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));

        _documentsClient.Verify(
            x => x.DeleteAsync(newPdfDocumentId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenDeletingPreviousPdfThrows_ShouldStillReturnSuccess()
    {
        var id = Guid.NewGuid();
        var previousPdfDocumentId = Guid.NewGuid();
        var newPdfDocumentId = Guid.NewGuid();
        var command = CreateCommand(id);
        var existingResult = CreateExistingResult(id, previousPdfDocumentId);
        var dto = CreateDto(id);

        _appointmentResultRepository
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResult>.Success(existingResult));

        _dtoBuilder
            .Setup(x => x.BuildAsync(existingResult, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AppointmentResultDto>.Success(dto));

        _pdfResultGenerator
            .Setup(x => x.Generate(dto))
            .Returns(new byte[] { 1 });

        _documentsClient
            .Setup(x => x.UploadAppointmentResultPdfAsync(
                id, It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newPdfDocumentId);

        _appointmentResultRepository
            .Setup(x => x.UpdateAsync(existingResult, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _appointmentResultRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _documentsClient
            .Setup(x => x.DeleteAsync(previousPdfDocumentId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("documents unreachable"));

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}