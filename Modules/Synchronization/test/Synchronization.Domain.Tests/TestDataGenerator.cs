using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.Modules.Synchronization.Domain.Tests;

public static class TestDataGenerator
{
    public static Datawallet CreateDatawallet()
    {
        return new Datawallet(new Datawallet.DatawalletVersion(1), CreateRandomIdentityAddress());
    }

    public static Datawallet CreateDatawallet(Datawallet.DatawalletVersion version)
    {
        return new Datawallet(version, CreateRandomIdentityAddress());
    }
}
