using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.SendDeletionProcessGracePeriodReminders;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("SendDeletionProcessGracePeriodRemindersCommand")]
public class Command : IRequest;
