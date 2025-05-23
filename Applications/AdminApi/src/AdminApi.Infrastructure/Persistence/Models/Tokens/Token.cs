namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Tokens;

public class Token
{
    public string Id { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
    public byte[] Content { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
