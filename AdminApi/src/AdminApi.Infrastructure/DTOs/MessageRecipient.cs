namespace Backbone.AdminApi.Infrastructure.DTOs;

public class MessageRecipient
{
    public int Id { get; set; }
    public string Address { get; set; } = null!;
    public string MessageId { get; set; } = null!;
}
