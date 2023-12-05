using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.DeleteRelationshipTemplatesByIdentity;

public class DeleteRelationshipTemplatesByIdentityCommand(IdentityAddress identityAddress) : RequestWithIdentityAddress(identityAddress) { }
