using System.Net;
using Notifications.Application.Commands;
using Notifications.Application.Interfaces;

namespace Notifications.Infrastructure.Email;

public sealed class EmailBodyBuilder : IEmailBodyBuilder
{
    public string BuildAppointmentResultEmail(
        SendAppointmentResultCommand command)
    {
        var patientName = WebUtility.HtmlEncode(
            command.PatientFullName);

        var doctorName = WebUtility.HtmlEncode(
            command.DoctorFullName);

        var specialization = WebUtility.HtmlEncode(
            command.DoctorSpecialization);

        var serviceName = WebUtility.HtmlEncode(
            command.ServiceName);

        return $"""
                <html>
                <body>
                    <h2>Result of your appointment</h2>

                    <p>Hello, {patientName}!</p>

                    <p>
                        Your appointment result is ready.
                    </p>

                    <p>
                        <strong>Doctor:</strong> {doctorName}<br>
                        <strong>Specialization:</strong> {specialization}<br>
                        <strong>Service:</strong> {serviceName}<br>
                        <strong>Date:</strong> {command.ResultDate:dd.MM.yyyy HH:mm}
                    </p>

                    <p>
                        The appointment result is attached to this email as a PDF file.
                    </p>

                    <p>
                        Best regards,<br> 
                        Medical Center 
                    </p>
                </body>
                </html>
                """;
    }
}