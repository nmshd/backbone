using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Job.IdentityDeletion.Tests;

public class TestDataGenerator
{
    public static Identity CreateIdentity()
    {
        return new Identity(
            CreateRandomDeviceId(),
            CreateRandomIdentityAddress(),
            CreateRandomBytes(),
            TierId.Generate(),
            1,
            CommunicationLanguage.DEFAULT_LANGUAGE);
    }
}
