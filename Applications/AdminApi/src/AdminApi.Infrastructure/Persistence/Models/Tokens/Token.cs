// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Tokens;

public class Token
{
    public required string Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string CreatedBy { get; init; }
    public required byte[] Content { get; init; }
    public required DateTime ExpiresAt { get; init; }
}
