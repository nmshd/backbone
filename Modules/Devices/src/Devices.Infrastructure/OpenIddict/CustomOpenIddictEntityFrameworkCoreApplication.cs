﻿using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using OpenIddict.EntityFrameworkCore.Models;

namespace Backbone.Modules.Devices.Infrastructure.OpenIddict;

public class CustomOpenIddictEntityFrameworkCoreApplication : OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization,
    CustomOpenIddictEntityFrameworkCoreToken>
{
    public TierId DefaultTier { get; set; }

    public DateTime CreatedAt { get; set; }

    public OAuthClient ToModel()
    {
        return new OAuthClient(ClientId!, DisplayName!, DefaultTier, CreatedAt);
    }

    public void UpdateFromModel(OAuthClient client)
    {
        DefaultTier = client.DefaultTier;
    }
}
