namespace Appointments.Infrastructure.Messaging;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string Host { get; set; } = null!;
    public string VirtualHost { get; set; } = "/";
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}