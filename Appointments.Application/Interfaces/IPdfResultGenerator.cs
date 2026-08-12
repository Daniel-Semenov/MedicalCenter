using Appointments.Application.DTO_s.Response.AppointmentResult;

namespace Appointments.Application.Interfaces;

public interface IPdfResultGenerator
{
    byte[] Generate(AppointmentResultDto result);
}