using Documents.Domain.Entities;

namespace Documents.Application.Interfaces;

public interface IObjectKeyGenerator
{
    string Generate(DocumentOwnerType documentOwnerType, Guid ownerId, Guid documentId, string fileName);
}