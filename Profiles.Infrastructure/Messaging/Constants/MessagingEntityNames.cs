namespace Profiles.Infrastructure.Messaging.Constants;

public static class MessagingEntityNames
{
    public const string DoctorCreated = "Profiles.Infrastructure.Messaging.Contracts.Events.Doctors:DoctorCreated";
    public const string DoctorUpdated = "Profiles.Infrastructure.Messaging.Contracts.Events.Doctors:DoctorUpdated";
    public const string DoctorDeleted = "Profiles.Infrastructure.Messaging.Contracts.Events.Doctors:DoctorDeleted";

    public const string PatientCreated = "Profiles.Infrastructure.Messaging.Contracts.Events.Patients:PatientCreated";
    public const string PatientUpdated = "Profiles.Infrastructure.Messaging.Contracts.Events.Patients:PatientUpdated";
    public const string PatientDeleted = "Profiles.Infrastructure.Messaging.Contracts.Events.Patients:PatientDeleted";

    public const string ReceptionistCreated = "Profiles.Infrastructure.Messaging.Contracts.Events.Receptionists:ReceptionistCreated";
    public const string ReceptionistUpdated = "Profiles.Infrastructure.Messaging.Contracts.Events.Receptionists:ReceptionistUpdated";
    public const string ReceptionistDeleted = "Profiles.Infrastructure.Messaging.Contracts.Events.Receptionists:ReceptionistDeleted";
    
    public const string AccountCreated = "Authorization.Infrastructure.Messaging.Contracts.Events.Accounts:AccountCreated";
    public const string AccountUpdated = "Authorization.Infrastructure.Messaging.Contracts.Events.Accounts:AccountUpdated";
    public const string AccountDeactivated = "Authorization.Infrastructure.Messaging.Contracts.Events.Accounts:AccountDeactivated";
}