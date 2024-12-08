using Doctor.Management.Gateway.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServices();

WebApplication app = builder.Build();

//await app.AddDynamicRoutes();

app.MapReverseProxy();

app.Run();
