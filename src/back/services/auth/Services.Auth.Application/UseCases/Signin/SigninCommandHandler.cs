using Common.Services.Hashing.Abstractions;
using Common.Services.Mails.Abstractions;
using CQRS.MediatR.Helper.Abstractions.Messaging;
using Microsoft.Extensions.Options;
using Services.Auth.Application.Settings;
using Services.Auth.Domain.Entities;
using Services.Auth.Domain.Errors;
using Shared.Common.Helper.ErrorsHandler;
using Shared.Common.Helper.Providers;
using System.IdentityModel.Tokens.Jwt;
using Value.Objects.Helper.Values.Domain;

namespace Services.Auth.Application.UseCases;

internal sealed class SigninCommandHandler
    : ICommandHandler<SigninCommand, SigninResponse>
{
    private readonly IHashingService _hashService;
    private readonly IMailService _emailService;

    private readonly JwtSettings _jwtSettings;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
    private readonly EntitiesEventsManagementProvider _entitiesEventsManagement;

    public SigninCommandHandler(
        IHashingService hashService,
        IMailService emailService,

        IOptions<JwtSettings> jwtSettings,
        JwtSecurityTokenHandler jwtSecurityTokenHandler,
        EntitiesEventsManagementProvider entitiesEventsManagement)
    {
        ArgumentNullException.ThrowIfNull(hashService, nameof(hashService));
        ArgumentNullException.ThrowIfNull(emailService, nameof(emailService));

        ArgumentNullException.ThrowIfNull(jwtSettings, nameof(jwtSettings));
        ArgumentNullException.ThrowIfNull(jwtSecurityTokenHandler, nameof(jwtSecurityTokenHandler));
        ArgumentNullException.ThrowIfNull(entitiesEventsManagement, nameof(entitiesEventsManagement));

        _hashService = hashService;
        _emailService = emailService;

        _jwtSettings = jwtSettings.Value;
        _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
        _entitiesEventsManagement = entitiesEventsManagement;
    }

    public async Task<Result<SigninResponse>> Handle(
        SigninCommand request,
        CancellationToken cancellationToken)
    {
        Result<EmailAddress> requestEmailResult = EmailAddress.Create(request.Email);
        if (requestEmailResult.IsFailure)
            return Result.Failure<SigninResponse>(requestEmailResult.Error);

        Result<Credential> found = await _unitOfWork.Credential.ByEmailAsync(requestEmailResult.Value, cancellationToken: cancellationToken);
        if (found.IsFailure)
            return Result.Failure<SigninResponse>(found.Error);

        Result<string> token = found.Value.BuildJwt(in _jwtSettings, in _jwtSecurityTokenHandler);
        if (token.IsFailure)
            return Result.Failure<SigninResponse>(token.Error);

        return new SigninResponse(token.Value, CredentialResponse.MapFromEntity(found.Value));
    }
}