using Backbone.BuildingBlocks.Application.Identities.Commands;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DeleteRelationshipTemplatesByIdentity;

public class DeleteRelationshipTemplatesByIdentityCommand(IdentityAddress identityAddress) : RequestWithIdentityAddress(identityAddress) { }
