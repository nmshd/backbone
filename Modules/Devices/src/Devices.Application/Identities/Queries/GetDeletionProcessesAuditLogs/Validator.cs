using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessesAuditLogs;

public class Validator : AbstractValidator<GetDeletionProcessesAuditLogsQuery>
{
    public Validator()
    {
        RuleFor(x => x.IdentityAddress).ValidId<GetDeletionProcessesAuditLogsQuery, IdentityAddress>();
    }
}
