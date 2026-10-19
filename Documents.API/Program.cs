using Documents.Application.Extensions;
using Documents.Infrastructure.Db;
using Documents.Infrastructure.Extensions;
using Documents.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    var sp = scope.ServiceProvider;

    var db = sp.GetRequiredService<DocumentsDbContext>();
    await db.Database.MigrateAsync();

    var initializer = sp.GetRequiredService<MinioBucketInitializer>();
    await initializer.InitializeAsync();
}

app.MapControllers();

app.Run();