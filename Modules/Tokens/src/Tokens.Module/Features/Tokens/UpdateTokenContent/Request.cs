namespace Backbone.Modules.Tokens.Module.Features.Tokens.UpdateTokenContent;

public class UpdateTokenContentRequest
{
    public required byte[] NewContent { get; init; }
    public byte[]? Password { get; init; }
}
