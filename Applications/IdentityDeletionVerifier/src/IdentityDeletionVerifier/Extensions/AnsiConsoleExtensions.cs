using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.Tooling.Extensions;
using Spectre.Console;

namespace Backbone.IdentityDeletionVerifier.Extensions;

public static class AnsiConsoleExtensions
{
    public static bool WriteResult(this IAnsiConsole console, params IResponse[] responses)
    {
        var success = responses.All(r => r.IsSuccess);
        console.WriteLine(success ? Emoji.Known.CheckMarkButton : Emoji.Known.CrossMark);

        return success;
    }
}
