using Enmeshed.BuildingBlocks.Application.FluentValidation;
using Enmeshed.Tooling.Extensions;
using FluentValidation;

namespace Relationships.Application.Relationships.Commands.RejectRelationshipChangeRequest;

// ReSharper disable once UnusedMember.Global
public class RejectRelationshipChangeRequestCommandValidator : AbstractValidator<RejectRelationshipChangeRequestCommand>
{
    public RejectRelationshipChangeRequestCommandValidator()
    {
        RuleFor(c => c.Id).DetailedNotNull();
        RuleFor(c => c.ResponseContent).NumberOfBytes(0, 10.Mebibytes());
    }
}
