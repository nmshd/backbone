using System.Collections.Generic;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using CSharpFunctionalExtensions;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.BuildingBlocks.Domain.Errors;
using Enmeshed.BuildingBlocks.Domain.StronglyTypedIds.Records;
using Enmeshed.DevelopmentKit.Identity.Entities;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class Identity
{
    private readonly List<IdentityDeletionProcess> _deletionProcesses;


    public Identity(string? clientId, IdentityAddress address, byte[] publicKey, TierId tierId, byte identityVersion)
    {
        ClientId = clientId;
        Address = address;
        PublicKey = publicKey;
        IdentityVersion = identityVersion;
        CreatedAt = SystemTime.UtcNow;
        Devices = new List<Device>();
        TierId = tierId;
        _deletionProcesses = new List<IdentityDeletionProcess>();
    }

    public string? ClientId { get; private set; }

    public IdentityAddress Address { get; private set; }
    public byte[] PublicKey { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public List<Device> Devices { get; }

    public byte IdentityVersion { get; private set; }

    public TierId? TierId { get; private set; }

    public IReadOnlyList<IdentityDeletionProcess> DeletionProcesses => _deletionProcesses;

    public bool IsNew()
    {
        return Devices.Count < 1;
    }

    public void ChangeTier(TierId id)
    {
        if (TierId == id)
        {
            throw new DomainException(GenericDomainErrors.NewAndOldParametersMatch("TierId"));
        }

        TierId = id;
    }

    public void StartDeletionProcess(DeviceId asDevice, IHasher hasher)
    {
        var activeProcessExists = DeletionProcesses.Any(d => d.IsActive());

        if (activeProcessExists)
            throw new DomainException(DomainErrors.OnlyOneActiveDeletionProcessAllowed());

        _deletionProcesses.Add(IdentityDeletionProcess.Create(Address, asDevice, hasher));
    }
}

public enum DeletionProcessStatus
{
    WaitingForApproval
}

public record IdentityDeletionProcessId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;

    private const string PREFIX = "IDP";

    private static readonly StronglyTypedIdHelpers UTILS = new(PREFIX, DEFAULT_VALID_CHARS, MAX_LENGTH);

    private IdentityDeletionProcessId(string value) : base(value) { }

    public static IdentityDeletionProcessId Generate()
    {
        var randomPart = StringUtils.Generate(DEFAULT_VALID_CHARS, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new IdentityDeletionProcessId(PREFIX + randomPart);
    }

    public static Result<IdentityDeletionProcessId, DomainError> Create(string value)
    {
        var validationError = UTILS.Validate(value);

        if (validationError != null)
            return Result.Failure<IdentityDeletionProcessId, DomainError>(validationError);

        return Result.Success<IdentityDeletionProcessId, DomainError>(new IdentityDeletionProcessId(value));
    }
}
