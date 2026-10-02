using Backbone.Tooling.Extensions;
using FluentValidation;

namespace Backbone.Modules.Files.Module.Features.Files.CreateFile.Http;

public class RequestFormParamsValidator : AbstractValidator<RequestFormParams>
{
    private const string MIME_TYPE = "application/octet-stream";

    public RequestFormParamsValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(f => f.Content).NotNull();
        RuleFor(f => f.Content.Length).InclusiveBetween(1, 10.Mebibytes()).WithName("Content Length");
    }
}
