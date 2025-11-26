using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Messages.Domain.Ids;

namespace Backbone.Modules.Messages.Domain.Entities;

public class Attachment : Entity
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected Attachment()
    {
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
