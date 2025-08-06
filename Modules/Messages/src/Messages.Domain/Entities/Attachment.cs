using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Messages.Domain.Ids;

namespace Backbone.Modules.Messages.Domain.Entities;

public class Attachment : Entity
{
    // ReSharper disable once UnusedMember.Local
    protected Attachment()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        MessageId = null!;
    }

    public Attachment(FileId id)
    {
        Id = id;
        MessageId = null!; // we just assign null to satisfy the compiler; it will be set by EF Core
    }

    public FileId Id { get; set; }
    public MessageId MessageId { get; set; }
}
