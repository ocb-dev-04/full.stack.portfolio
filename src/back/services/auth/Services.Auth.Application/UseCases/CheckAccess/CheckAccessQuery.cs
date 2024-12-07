using CQRS.MediatR.Helper.Abstractions.Messaging;

namespace Services.Auth.Application.UseCases;

internal sealed record CheckAccessQuery() 
    : IQuery<string>;
