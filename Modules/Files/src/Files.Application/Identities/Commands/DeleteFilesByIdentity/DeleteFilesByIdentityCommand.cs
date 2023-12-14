using Backbone.BuildingBlocks.Application.Identities.Commands;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Files.Application.Identities.Commands.DeleteFilesByIdentity;

public class DeleteFilesByIdentityCommand(IdentityAddress identityAddress) : RequestWithIdentityAddress(identityAddress);
