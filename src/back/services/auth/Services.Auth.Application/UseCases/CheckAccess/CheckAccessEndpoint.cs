using FastEndpoints;

namespace Services.Auth.Application.UseCases;

public sealed class CheckAccessEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/check-access");
        Summary(s =>
        {
            s.Summary = "Check if jwt used is valid";
            s.Responses[200] = "Valid JWT";
            s.Responses[401] = "Invalid JWT";
        });
    }
    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendAsync(string.Empty);
    }
}
