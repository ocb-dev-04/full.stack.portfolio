using FastEndpoints;
using Services.Auth.Api.Extensions;
using Services.Auth.Api.Middlewares;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServices();

WebApplication app = builder.Build();

//app.UseCustomHealthChecks();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandler>();

app.UseFastEndpoints();

app.Run();
