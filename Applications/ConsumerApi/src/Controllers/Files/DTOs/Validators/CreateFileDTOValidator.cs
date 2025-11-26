using Backbone.Tooling.Extensions;
using FluentValidation;

namespace Backbone.ConsumerApi.Controllers.Files.DTOs.Validators;

public class CreateFileDTOValidator : AbstractValidator<CreateFileDTO>
{
    public CreateFileDTOValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(f => f.Content).NotNull();
        RuleFor(f => f.Content.Length).InclusiveBetween(1, 10.Mebibytes()).WithName("Content Length");
    }
}
