using Microsoft.EntityFrameworkCore;
using Offices.API.Middleware;
using Offices.Application.Extensions;
using Offices.Infrastructure.Db;
using Offices.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication();

var app = builder.Build();

app.UseMiddleware<ExceptionHandling>();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsEnvironment("Docker"))
{
    app.UseHttpsRedirection();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OfficesOutboxDbContext>();
    await db.Database.MigrateAsync();
}

app.UseAuthorization();
app.MapControllers();

app.Run();