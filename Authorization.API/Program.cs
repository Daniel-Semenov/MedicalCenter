using Authorization.API.Extensions;
using Authorization.Application.Extensions;
using Authorization.Infrastructure.Db;
using Authorization.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddApiLayer();

builder.Services.AddApplicationLayer();

builder.Services.AddInfrastructureLayer(
    builder.Configuration);

var app = builder.Build();

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
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

await app.SeedIdentityAsync();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();