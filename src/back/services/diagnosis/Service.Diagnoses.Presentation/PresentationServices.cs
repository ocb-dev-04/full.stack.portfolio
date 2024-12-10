using Microsoft.Extensions.DependencyInjection;

namespace Service.DiagnosesPresentation;

public static class PresentationServices
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor()
            .AddControllers()
            .AddApplicationPart(typeof(PresentationServices).Assembly);

        return services;
    }
}

