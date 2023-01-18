using Enmeshed.BuildingBlocks.Application.FluentValidation;
using Enmeshed.Tooling.Extensions;
using FluentValidation;

namespace Relationships.Application.Relationships.Commands.RevokeRelationshipChangeRequest;

// ReSharper disable once UnusedMember.Global
public class RevokeRelationshipChangeRequestCommandValidator : AbstractValidator<RevokeRelationshipChangeRequestCommand>
{
    public RevokeRelationshipChangeRequestCommandValidator()
    {
        RuleFor(c => c.Id).DetailedNotNull();
        RuleFor(c => c.ResponseContent).NumberOfBytes(0, 10.Mebibytes());
    }
}
