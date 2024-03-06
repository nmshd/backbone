using Backbone.Modules.Messages.Domain.Ids;

namespace Backbone.Modules.Messages.Domain.Entities;

public class Attachment
{
    public Attachment(FileId id)
    {
        Id = id;
        MessageId = null!; // we just assign null to satisfy the compiler; it will be set by EF Core
    }

    public FileId Id { get; set; }
    public MessageId MessageId { get; set; }
}
