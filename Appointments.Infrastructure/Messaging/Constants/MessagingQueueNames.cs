namespace Appointments.Infrastructure.Messaging.Constants;

public static class MessagingQueueNames
{
    public const string ServiceEvents = "appointments-service-events";
    public const string SpecializationEvents = "appointments-specialization-events";
    public const string DoctorEvents = "appointments-doctor-events";
    public const string PatientEvents = "appointments-patient-events";
}