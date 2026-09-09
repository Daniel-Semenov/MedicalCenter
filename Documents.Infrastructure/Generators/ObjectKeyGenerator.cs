using Documents.Application.Interfaces;
using Documents.Domain.Entities;

namespace Documents.Infrastructure.Generators;

public sealed class ObjectKeyGenerator : IObjectKeyGenerator
{
    public string Generate(
        DocumentOwnerType ownerType,
        Guid ownerId,
        Guid documentId,
        string fileName)
    {
        var extension = Path.GetExtension(fileName);

        var folder = ownerType switch
        {
            DocumentOwnerType.OfficePhoto =>
                "offices",

            DocumentOwnerType.ProfilePhoto =>
                "profiles",

            DocumentOwnerType.AppointmentResultPdf =>
                "appointment-results",

            _ => throw new ArgumentOutOfRangeException(
                nameof(ownerType),
                ownerType,
                null)
        };

        return $"{folder}/{ownerId}/{documentId}{extension}";
    }
}