using Messages.Domain.Ids;

namespace Messages.Domain.Entities;

public class Attachment
{
#pragma warning disable CS8618
    public Attachment(FileId id)
#pragma warning restore CS8618
    {
        Id = id;
    }

    public FileId Id { get; set; }
    public MessageId MessageId { get; set; }
    public Message Message { get; set; }
}
