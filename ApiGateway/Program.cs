using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapGet("/__debug/routes", (IProxyConfigProvider provider) =>
{
    return provider.GetConfig().Routes.Select(r => new { r.RouteId, Path = r.Match.Path });
});

app.MapReverseProxy();

app.Run();

