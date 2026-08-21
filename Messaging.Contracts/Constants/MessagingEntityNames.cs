namespace Messaging.Contracts.Constants;

public static class MessagingEntityNames
{
    public const string DoctorCreated = "Profiles.DoctorCreated";
    public const string DoctorUpdated = "Profiles.DoctorUpdated";
    public const string DoctorDeleted = "Profiles.DoctorDeleted";

    public const string PatientCreated = "Profiles.PatientCreated";
    public const string PatientUpdated = "Profiles.PatientUpdated";
    public const string PatientDeleted = "Profiles.PatientDeleted";

    public const string ReceptionistCreated = "Profiles.ReceptionistCreated";
    public const string ReceptionistUpdated = "Profiles.ReceptionistUpdated";
    public const string ReceptionistDeleted = "Profiles.ReceptionistDeleted";

    public const string OfficeCreated = "Offices.OfficeCreated";
    public const string OfficeUpdated = "Offices.OfficeUpdated";
    public const string OfficeStatusChanged = "Offices.OfficeStatusChanged";
    public const string OfficeDeleted = "Offices.OfficeDeleted";

    public const string ServiceCreated = "Services.ServiceCreated";
    public const string ServiceUpdated = "Services.ServiceUpdated";
    public const string ServiceDeleted = "Services.ServiceDeleted";

    public const string SpecializationCreated = "Services.SpecializationCreated";
    public const string SpecializationUpdated = "Services.SpecializationUpdated";
    public const string SpecializationDeleted = "Services.SpecializationDeleted";

    public const string AccountCreated = "Authorization.AccountCreated";
    public const string AccountUpdated = "Authorization.AccountUpdated";
    public const string AccountDeactivated = "Authorization.AccountDeactivated";
}