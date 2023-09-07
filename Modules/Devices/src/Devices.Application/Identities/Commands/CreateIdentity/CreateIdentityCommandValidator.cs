﻿using Backbone.Modules.Devices.Application.Devices.DTOs.Validators;
using Enmeshed.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CreateIdentity;

// ReSharper disable once UnusedMember.Global
public class CreateIdentityCommandValidator : AbstractValidator<CreateIdentityCommand>
{
    public CreateIdentityCommandValidator()
    {
        RuleFor(c => c.DefaultTier).DetailedNotEmpty();
        RuleFor(c => c.IdentityPublicKey).DetailedNotEmpty();
        RuleFor(c => c.DevicePassword).DetailedNotEmpty();
        RuleFor(c => c.SignedChallenge).DetailedNotEmpty().SetValidator(new SignedChallengeDTOValidator());
    }
}
