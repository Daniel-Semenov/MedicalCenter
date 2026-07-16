using Authorization.API.Extensions;
using Authorization.Application.Extensions;
using Authorization.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddApiLayer();

builder.Services.AddApplicationLayer();

builder.Services.AddInfrastructureLayer(
    builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();