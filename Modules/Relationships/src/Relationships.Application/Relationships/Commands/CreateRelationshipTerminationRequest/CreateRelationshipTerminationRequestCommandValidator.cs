using Enmeshed.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationshipTerminationRequest;

// ReSharper disable once UnusedMember.Global
public class CreateRelationshipTerminationRequestCommandValidator : AbstractValidator<CreateRelationshipTerminationRequestCommand>
{
    public CreateRelationshipTerminationRequestCommandValidator()
    {
        RuleFor(c => c.Id).DetailedNotNull();
    }
}
