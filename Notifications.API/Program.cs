using Microsoft.EntityFrameworkCore;
using Notifications.Application.Extensions;
using Notifications.Infrastructure.Db;
using Notifications.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsEnvironment("Docker"))
{
    app.UseHttpsRedirection();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
    await db.Database.MigrateAsync();
}

app.MapControllers();

app.Run();