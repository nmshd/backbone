using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;
using Backbone.Tooling;

namespace Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

public class RelationshipTemplate : Entity
{
    // ReSharper disable once UnusedMember.Local
    private RelationshipTemplate()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        CreatedBy = null!;
        CreatedByDevice = null!;
    }

    public RelationshipTemplate(IdentityAddress createdBy, DeviceId createdByDevice, int? maxNumberOfAllocations, DateTime? expiresAt, byte[] content, IdentityAddress? forIdentity = null,
        byte[]? password = null)
    {
        Id = RelationshipTemplateId.New();
        CreatedAt = SystemTime.UtcNow;

        CreatedBy = createdBy;
        CreatedByDevice = createdByDevice;
        MaxNumberOfAllocations = maxNumberOfAllocations;
        ExpiresAt = expiresAt;
        Content = content;
        ForIdentity = forIdentity;
        Password = password;

        RaiseDomainEvent(new RelationshipTemplateCreatedDomainEvent(this));
    }

    public RelationshipTemplateId Id { get; set; }

    public ICollection<Relationship> Relationships { get; set; } = [];

    public IdentityAddress CreatedBy { get; set; }
    public DeviceId CreatedByDevice { get; set; }
    public int? MaxNumberOfAllocations { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public byte[]? Content { get; private set; }

    public DateTime CreatedAt { get; set; }

    public IdentityAddress? ForIdentity { get; set; }
    public byte[]? Password { get; set; }

    public List<RelationshipTemplateAllocation> Allocations { get; set; } = [];

    public void AllocateFor(IdentityAddress identity, DeviceId device)
    {
        if (identity == CreatedBy)
            return;

        if (IsAllocatedBy(identity))
            return;

        if (Allocations.Count == MaxNumberOfAllocations)
            throw new DomainException(DomainErrors.MaxNumberOfAllocationsExhausted());

        Allocations.Add(new RelationshipTemplateAllocation(Id, identity, device));
    }

    public bool IsAllocatedBy(IdentityAddress identity)
    {
        return Allocations.All(x => x.AllocatedBy != identity);
    }

    // TODO @Nikola: Find a better name for this
    public bool CanBeCollectedWithPassword2(IdentityAddress address, byte[]? password)
    {
        return CanBeCollectedWithPassword(address, password).Compile()(this);
    }

    #region Expressions

    public static Expression<Func<RelationshipTemplate, bool>> HasId(RelationshipTemplateId id)
    {
        return r => r.Id == id;
    }

    public static Expression<Func<RelationshipTemplate, bool>> WasCreatedBy(IdentityAddress identityAddress)
    {
        return r => r.CreatedBy == identityAddress.ToString();
    }

    public static Expression<Func<RelationshipTemplate, bool>> CanBeCollectedBy(IdentityAddress address)
    {
        return relationshipTemplate => relationshipTemplate.ForIdentity == null || relationshipTemplate.ForIdentity == address || relationshipTemplate.CreatedBy == address;
    }

    public static Expression<Func<RelationshipTemplate, bool>> CanBeCollectedWithPassword(IdentityAddress address, byte[]? password)
    {
        return relationshipTemplate => relationshipTemplate.Password == null || relationshipTemplate.Password == password || relationshipTemplate.CreatedBy == address;
    }

    // relationshipTemplate.Password.SequenceEqual(password!)
    // relationshipTemplate.Password == password

    #endregion
}
