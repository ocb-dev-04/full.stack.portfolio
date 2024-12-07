using Microsoft.AspNetCore.Mvc;
using Shared.Global.Sources.Behaviors;
using System.IdentityModel.Tokens.Jwt;
using Services.Auth.Application.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Services.Auth.Application;

public static class Services
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddTransient<TokenProvider>();
        services.AddSingleton<JwtSecurityTokenHandler>();

        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(Services).Assembly);

            config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });
        return services;
    }
}
