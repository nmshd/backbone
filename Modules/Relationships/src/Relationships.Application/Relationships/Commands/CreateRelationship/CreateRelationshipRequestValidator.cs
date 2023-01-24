using Enmeshed.BuildingBlocks.Application.FluentValidation;
using Enmeshed.Tooling.Extensions;
using FluentValidation;

namespace Relationships.Application.Relationships.Commands.CreateRelationship;

// ReSharper disable once UnusedMember.Global
public class CreateRelationshipCommandValidator : AbstractValidator<CreateRelationshipCommand>
{
    public CreateRelationshipCommandValidator()
    {
        RuleFor(c => c.RelationshipTemplateId).DetailedNotEmpty();
        RuleFor(c => c.Content).NumberOfBytes(0, 10.Mebibytes());
    }
}
