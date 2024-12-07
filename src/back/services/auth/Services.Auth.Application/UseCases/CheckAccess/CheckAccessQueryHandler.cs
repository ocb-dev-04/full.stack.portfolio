using Shared.Common.Helper.ErrorsHandler;
using CQRS.MediatR.Helper.Abstractions.Messaging;

namespace Services.Auth.Application.UseCases;

internal sealed class CheckAccessQueryHandler
    : IQueryHandler<CheckAccessQuery, string>
{
    public Task<Result<string>> Handle(CheckAccessQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
