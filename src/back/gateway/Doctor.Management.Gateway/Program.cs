using Doctor.Management.Gateway.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServices();

WebApplication app = builder.Build();

app.MapReverseProxy();

app.Run();
