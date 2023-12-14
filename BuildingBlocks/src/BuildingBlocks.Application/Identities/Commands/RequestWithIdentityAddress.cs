using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.BuildingBlocks.Application.Identities.Commands;
public class RequestWithIdentityAddress(IdentityAddress identityAddress) : IRequest
{
    public IdentityAddress IdentityAddress { get; set; } = identityAddress;
}
