using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Application.Identities.Commands.DeleteRelationships;

public class DeleteRelationshipsCommand(IdentityAddress identityAddress) : RequestWithIdentityAddress(identityAddress) { }
