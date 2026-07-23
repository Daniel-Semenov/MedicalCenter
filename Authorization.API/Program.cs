using Authorization.API.Extensions;
using Authorization.Application.Extensions;
using Authorization.Domain.Entities;
using Authorization.Infrastructure.Extensions;
using Authorization.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddApiLayer();

builder.Services.AddApplicationLayer();

builder.Services.AddInfrastructureLayer(
    builder.Configuration);

var app = builder.Build();

await app.SeedIdentityAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();