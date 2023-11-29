using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CreateAuditLog;
public class DeleteIdentityCommand(string identityAddress) : IRequest
{
    public string IdentityAddress { get; set; } = identityAddress;
}
