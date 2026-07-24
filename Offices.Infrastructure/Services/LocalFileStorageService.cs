using Microsoft.AspNetCore.Hosting;
using Offices.Application.Interfaces;

namespace Offices.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadsPath;

    public LocalFileStorageService(IWebHostEnvironment environment)
    {
        _uploadsPath = Path.Combine(environment.WebRootPath, "uploads");
        
        if (!Directory.Exists(_uploadsPath))
        {
            Directory.CreateDirectory(_uploadsPath);
        }
    }

    public async Task<string> UploadFileAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
        var fullPath = Path.Combine(_uploadsPath, uniqueFileName);

        using var fileStreamOutput = new FileStream(fullPath, FileMode.Create);
        await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);

        return $"/uploads/{uniqueFileName}";
    }

    public Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(fileUrl))
            return Task.CompletedTask;

        var fileName = Path.GetFileName(fileUrl);
        var fullPath = Path.Combine(_uploadsPath, fileName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}