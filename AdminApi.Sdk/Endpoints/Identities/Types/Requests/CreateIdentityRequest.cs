﻿using Backbone.AdminApi.Sdk.Endpoints.Challenges.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Identities.Types.Requests;

public class CreateIdentityRequest
{
    public required string ClientId { get; set; }
    public required byte[] IdentityPublicKey { get; set; }
    public required string DevicePassword { get; set; }
    public required string DeviceCommunicationLanguage { get; set; }
    public required byte IdentityVersion { get; set; }
    public required SignedChallenge SignedChallenge { get; set; }
}
