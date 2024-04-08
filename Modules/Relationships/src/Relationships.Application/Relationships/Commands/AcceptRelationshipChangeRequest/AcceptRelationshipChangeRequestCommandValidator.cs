using Backbone.BuildingBlocks.Application.FluentValidation;
using Backbone.Tooling.Extensions;
using FluentValidation;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.AcceptRelationshipChangeRequest;

// ReSharper disable once UnusedMember.Global
public class AcceptRelationshipChangeRequestCommandValidator : AbstractValidator<AcceptRelationshipChangeRequestCommand>
{
    public AcceptRelationshipChangeRequestCommandValidator()
    {
        RuleFor(c => c.Id).DetailedNotNull();
        RuleFor(c => c.ResponseContent).NumberOfBytes(0, 10.Mebibytes());
    }
}
