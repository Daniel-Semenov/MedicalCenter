namespace Appointments.Infrastructure.Messaging.Constants;

public static class MessagingEntityNames
{
    public const string ServiceCreated = "Services.Messaging.Contracts.Events.Services:ServiceCreated";
    public const string ServiceUpdated = "Services.Messaging.Contracts.Events.Services:ServiceUpdated";
    public const string ServiceDeleted = "Services.Messaging.Contracts.Events.Services:ServiceDeleted";

    public const string SpecializationCreated = "Services.Messaging.Contracts.Events.Specializations:SpecializationCreated";
    public const string SpecializationUpdated = "Services.Messaging.Contracts.Events.Specializations:SpecializationUpdated";
    public const string SpecializationDeleted = "Services.Messaging.Contracts.Events.Specializations:SpecializationDeleted";
    
    public const string DoctorCreated = "Profiles.Infrastructure.Messaging.Contracts.Events.Doctors:DoctorCreated";
    public const string DoctorUpdated = "Profiles.Infrastructure.Messaging.Contracts.Events.Doctors:DoctorUpdated";
    public const string DoctorDeleted = "Profiles.Infrastructure.Messaging.Contracts.Events.Doctors:DoctorDeleted";

    public const string PatientCreated = "Profiles.Infrastructure.Messaging.Contracts.Events.Patients:PatientCreated";
    public const string PatientUpdated = "Profiles.Infrastructure.Messaging.Contracts.Events.Patients:PatientUpdated";
    public const string PatientDeleted = "Profiles.Infrastructure.Messaging.Contracts.Events.Patients:PatientDeleted";
}