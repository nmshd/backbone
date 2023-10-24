﻿using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Errors;
using Backbone.Modules.Relationships.Domain.Ids;
using Backbone.Tooling;

namespace Backbone.Modules.Relationships.Domain.Entities;

public class RelationshipTemplate
{
#pragma warning disable CS8618
    private RelationshipTemplate() { }
#pragma warning restore CS8618

    public RelationshipTemplate(IdentityAddress createdBy, DeviceId createdByDevice, int? maxNumberOfAllocations, DateTime? expiresAt, byte[] content)
    {
        Id = RelationshipTemplateId.New();
        CreatedAt = SystemTime.UtcNow;

        CreatedBy = createdBy;
        CreatedByDevice = createdByDevice;
        MaxNumberOfAllocations = maxNumberOfAllocations;
        ExpiresAt = expiresAt;
        Content = content;
    }

    public RelationshipTemplateId Id { get; set; }

    public ICollection<Relationship> Relationships { get; set; } = new List<Relationship>();

    public IdentityAddress CreatedBy { get; set; }
    public DeviceId CreatedByDevice { get; set; }
    public int? MaxNumberOfAllocations { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public byte[]? Content { get; private set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public List<RelationshipTemplateAllocation> Allocations { get; set; } = new();

    public void AllocateFor(IdentityAddress identity, DeviceId device)
    {
        if (identity == CreatedBy)
            return;

        if (Allocations.Any(x => x.AllocatedBy == identity))
            return;

        if (Allocations.Count == MaxNumberOfAllocations)
            throw new DomainException(DomainErrors.MaxNumberOfAllocationsExhausted());

        Allocations.Add(new RelationshipTemplateAllocation(Id, identity, device));
    }

    public void LoadContent(byte[] content)
    {
        if (Content != null)
            throw new Exception("Cannot change the content of a relationship template.");

        Content = content;
    }
}
