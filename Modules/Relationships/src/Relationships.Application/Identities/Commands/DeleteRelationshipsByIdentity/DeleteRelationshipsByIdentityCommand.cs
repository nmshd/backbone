using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Application.Identities.Commands.DeleteRelationshipsByIdentity;

public class DeleteRelationshipsByIdentityCommand(IdentityAddress identityAddress) : RequestWithIdentityAddress(identityAddress) { }
