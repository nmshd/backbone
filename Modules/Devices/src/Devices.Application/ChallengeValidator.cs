using System.Text.Json;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.Crypto;
using Enmeshed.Crypto.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Application;

public class ChallengeValidator
{
    private readonly IDevicesDbContext _dbContext;
    private readonly ISignatureHelper _signatureHelper;

    public ChallengeValidator(IDevicesDbContext dbContext, ISignatureHelper signatureHelper)
    {
        _dbContext = dbContext;
        _signatureHelper = signatureHelper;
    }

    public async Task Validate(SignedChallengeDTO signedChallenge, PublicKey publicKey)
    {
        ValidateSignature(signedChallenge.Challenge, Signature.FromBytes(signedChallenge.Signature).Bytes, publicKey.Key);
        await ValidateChallengeExpiracy(signedChallenge.Challenge);
    }

    private void ValidateSignature(string challenge, byte[] signature, byte[] publicKey)
    {
        var signatureIsValid = _signatureHelper.VerifySignature(
            ConvertibleString.FromUtf8(challenge),
            ConvertibleString.FromByteArray(signature),
            ConvertibleString.FromByteArray(publicKey));

        if (!signatureIsValid)
            throw new OperationFailedException(ApplicationErrors.Devices.InvalidSignature());
    }

    private async Task ValidateChallengeExpiracy(string serializedChallenge)
    {
        var deserializedChallenge = JsonSerializer.Deserialize<ChallengeDTO>(serializedChallenge, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (deserializedChallenge == null)
            throw new NotFoundException(nameof(Challenge));

        var challenge = await _dbContext.SetReadOnly<Challenge>().FirstOrDefaultAsync(c => c.Id == deserializedChallenge.Id);

        if (challenge == null)
            throw new NotFoundException(nameof(Challenge));

        if (challenge.IsExpired())
            throw new OperationFailedException(ApplicationErrors.Devices.ChallengeHasExpired());
    }
}
