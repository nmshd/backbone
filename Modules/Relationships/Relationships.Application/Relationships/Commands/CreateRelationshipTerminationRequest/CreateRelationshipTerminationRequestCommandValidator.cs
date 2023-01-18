using Enmeshed.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Relationships.Application.Relationships.Commands.CreateRelationshipTerminationRequest;

// ReSharper disable once UnusedMember.Global
public class CreateRelationshipTerminationRequestCommandValidator : AbstractValidator<CreateRelationshipTerminationRequestCommand>
{
    public CreateRelationshipTerminationRequestCommandValidator()
    {
        RuleFor(c => c.Id).DetailedNotNull();
    }
}
