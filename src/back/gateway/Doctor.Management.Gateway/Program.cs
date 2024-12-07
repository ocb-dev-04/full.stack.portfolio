WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

WebApplication app = builder.Build();

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapReverseProxy();

app.Run();
