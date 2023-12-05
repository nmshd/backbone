﻿using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Messages.Application.Messages.Commands.DeleteMessagesByIdentity;

// TODO this command does not actually delete the messages, it changes the author/recipients to a dummy one.
// it might be interesting to add a backlog task to delete a message when all related Identities have been deleted.
public class DeleteMessagesByIdentityCommand(IdentityAddress identityAddress) : RequestWithIdentityAddress(identityAddress)
{
}
