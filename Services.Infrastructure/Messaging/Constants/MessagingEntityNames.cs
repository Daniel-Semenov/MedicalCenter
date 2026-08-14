namespace Services.Infrastructure.Messaging.Constants;

public static class MessagingEntityNames
{
    public const string ServiceCreated = "Services.Messaging.Contracts.Events.Services:ServiceCreated";
    public const string ServiceUpdated = "Services.Messaging.Contracts.Events.Services:ServiceUpdated";
    public const string ServiceDeleted = "Services.Messaging.Contracts.Events.Services:ServiceDeleted";

    public const string SpecializationCreated = "Services.Messaging.Contracts.Events.Specializations:SpecializationCreated";
    public const string SpecializationUpdated = "Services.Messaging.Contracts.Events.Specializations:SpecializationUpdated";
    public const string SpecializationDeleted = "Services.Messaging.Contracts.Events.Specializations:SpecializationDeleted";
}