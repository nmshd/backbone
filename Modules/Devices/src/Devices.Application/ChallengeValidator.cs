using System.Text.Json;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Crypto;
using Backbone.Crypto.Abstractions;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Application;

public class ChallengeValidator
{
    private static readonly JsonSerializerOptions JSON_SERIALIZER_OPTIONS = new() { PropertyNameCaseInsensitive = true };

    private readonly ISignatureHelper _signatureHelper;
    private readonly IChallengesRepository _challengesRepository;

    public ChallengeValidator(ISignatureHelper signatureHelper, IChallengesRepository challengesRepository)
    {
        _signatureHelper = signatureHelper;
        _challengesRepository = challengesRepository;
    }

    public async Task Validate(SignedChallengeDTO signedChallenge, PublicKey publicKey)
    {
        ValidateSignature(signedChallenge.Challenge, Signature.FromBytes(signedChallenge.Signature).Bytes, publicKey.Key);
        await ValidateChallengeExpiry(signedChallenge.Challenge);
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

    private async Task ValidateChallengeExpiry(string serializedChallenge)
    {
        var deserializedChallenge = JsonSerializer.Deserialize<ChallengeDTO>(serializedChallenge, JSON_SERIALIZER_OPTIONS) ?? throw new NotFoundException(nameof(Challenge));
        var challenge = await _challengesRepository.GetById(deserializedChallenge.Id, CancellationToken.None) ?? throw new NotFoundException(nameof(Challenge));
        if (challenge.IsExpired())
            throw new OperationFailedException(ApplicationErrors.Devices.ChallengeHasExpired());
    }
}
