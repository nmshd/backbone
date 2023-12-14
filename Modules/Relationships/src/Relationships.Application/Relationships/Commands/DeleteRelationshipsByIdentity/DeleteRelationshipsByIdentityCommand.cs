using Backbone.BuildingBlocks.Application.Identities.Commands;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DeleteRelationshipsByIdentity;

public class DeleteRelationshipsByIdentityCommand(IdentityAddress identityAddress) : RequestWithIdentityAddress(identityAddress) { }
