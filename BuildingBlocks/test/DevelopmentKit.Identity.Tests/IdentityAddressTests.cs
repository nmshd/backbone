using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Backbone.DevelopmentKit.Identity.Tests;

public class IdentityAddressTests
{
    [Theory]
    [InlineData("fj0o9eOiPRswTZL6j9lE9TRvpDDnPRMF0gJeahz/W2c=", "did:web:prod.enmeshed.eu:dids:id1QF24Gk2DfqCywRS7NpeH5iu7D4xvu6qv1")]
    [InlineData("jRxGfZtQ8a90TmKCGk+dhuX1CBjgoXuldhNPwrjpWsw=", "did:web:prod.enmeshed.eu:dids:id1HwY1TuyVBp3CmY3h18yTt1CKyu5qwB9wj")]
    [InlineData("PEODpwvi7KxIVa4qeUXia9apMFvPMktdDHiDitlfbjE=", "did:web:prod.enmeshed.eu:dids:id1LMp4k1XwxZ3WFXdAn9y12tv1ofe5so4kM")]
    [InlineData("mJGmNbxiVZAPToRuk9O3NvdfsWl6V+7wzIc+/57bU08=", "did:web:prod.enmeshed.eu:dids:id1McegXycvRoiJppS2LG25phn3jNveckFUL")]
    [InlineData("l68K/zdNp1VLoswcHAqN6QUFwCMU6Yvzf7XiW2m1hRY=", "did:web:prod.enmeshed.eu:dids:id193k6K5cJr94WJEWYb6Kei8zp5CGPyrQLS")]
    [InlineData("Gl8XTo8qFuUM+ksXixwp4g/jf3H/hU1F8ETuYaHCM5I=", "did:web:prod.enmeshed.eu:dids:id1BLrHAgDpimtLcGJGssMSm7bJHsvVe7CN")]
    [InlineData("rIS4kAzHXT7GgCA6Qm1ANlwM3x12QMSkeprHb6tjPyc=", "did:web:prod.enmeshed.eu:dids:id1NjGvLfWPrQ34PXWRBNiTfXv9DFiDQHExx")]
    [InlineData("hg/cbeBvfNrMiJ0dW1AtWC4IQwG4gkuhzG2+z6bAoRU=", "did:web:prod.enmeshed.eu:dids:id1Gda4aTXiBX9Pyc8UnmLaG44cX46umjnea")]
    [InlineData("kId+qWen/lKeTdyxcIQhkzvvvTU8wIJECfWUWbmRQRY=", "did:web:prod.enmeshed.eu:dids:id17RDEphijMPFGLbhqLWWgJfatBANMruC8f")]
    [InlineData("NcqlzTEpSlKX9gmNBv41EjPRHpaNYwt0bxqh1bgyJzA=", "did:web:prod.enmeshed.eu:dids:id19meHs4Di7JYNXoRPx9bFD6FUcpHFo3mBi")]
    [InlineData("49fWA+kzWNdCFdo92imTiQ4vUUJsPPLNlcB9udC4ooE=", "did:web:prod.enmeshed.eu:dids:id1c711BBi4yqV9wrLBVKxRSNFayfAm3Eib")]
    public void AddressIsCreatedCorrectly2(string publicKey, string expectedAddress)
    {
        var address = IdentityAddress.Create(Convert.FromBase64String(publicKey), "id1", "prod.enmeshed.eu");

        address.StringValue.Should().Be(expectedAddress);
    }

    [Fact]
    public void AddressIsCreatedCorrectly()
    {
        var testData = TestData.Valid();
        var address = IdentityAddress.Create(testData.PublicKey, testData.Realm, testData.InstanceUrl);

        address.Should().Be(testData.Address);
    }

    [Fact]
    public void EfConverterWorksCorrectly()
    {
        var testData = TestData.Valid();
        var address = IdentityAddress.Create(testData.PublicKey, testData.Realm, testData.InstanceUrl);

        address.Should().Be(testData.Address);
    }

    [Fact]
    public void ValidAddressesAreAccepted()
    {
        var testData = TestData.Valid();
        var isValid = IdentityAddress.IsValid(testData.Address);

        isValid.Should().BeTrue();
    }

    [Fact]
    public void AddressesWithInvalidRealmAreDeclined()
    {
        var testData = TestData.WithInvalidRealm();
        var isValid = IdentityAddress.IsValid(testData.Address);

        isValid.Should().BeFalse();
    }

    [Fact]
    public void AddressesWithInvalidChecksumAreDeclined()
    {
        var testData = TestData.WithInvalidChecksum();
        var isValid = IdentityAddress.IsValid(testData.Address);

        isValid.Should().BeFalse();
    }

    [Fact]
    public void AddressesWithInvalidMainPartAreDeclined()
    {
        var testData = TestData.WithInvalidMainPart();
        var isValid = IdentityAddress.IsValid(testData.Address);

        isValid.Should().BeFalse();
    }
}

internal class TestData
{
    public required string Realm { get; set; }
    public required string InstanceUrl { get; set; }
    public required byte[] PublicKey { get; set; }

    public required string Address { get; set; }

    public required string Checksum { get; set; }
    public required string MainPart { get; set; }

    public static TestData Valid()
    {
        return new TestData
        {
            Address = "did:web:prod.enmeshed.eu:dids:id18uSgVGTSNqECvt1DJM3bZg6U8p6RSjott",
            PublicKey = Convert.FromBase64String("tB9KFp/YqHrom3m5qUuZsd6l30DkaNjN14SxRw7YZuI="),
            Realm = "id1",
            Checksum = "jott",
            MainPart = "8uSgVGTSNqECvt1DJM3bZg6U8p6RS",
            InstanceUrl = "prod.enmeshed.eu"
        };
    }

    public static TestData WithInvalidRealm()
    {
        return new TestData
        {
            Address = "did:web:prod.enmeshed.eu:dids:id08uSgVGTSNqECvt1DJM3bZg6U8p6RSjott",
            PublicKey = Convert.FromBase64String("tB9KFp/YqHrom3m5qUuZsd6l30DkaNjN14SxRw7YZuI="),
            Realm = "id0",
            Checksum = "jott",
            MainPart = "8uSgVGTSNqECvt1DJM3bZg6U8p6RS",
            InstanceUrl = "prod.enmeshed.eu"
        };
    }

    public static TestData WithInvalidMainPart()
    {
        return new TestData
        {
            Address = "did:web:prod.enmeshed.eu:dids:id07uSgVGTSNqECvt1DJM3bZg6U8p6RSjott",
            PublicKey = Convert.FromBase64String("tB9KFp/YqHrom3m5qUuZsd6l30DkaNjN14SxRw7YZuI="),
            Realm = "id0",
            Checksum = "jott",
            MainPart = "7uSgVGTSNqECvt1DJM3bZg6U8p6RS",
            InstanceUrl = "prod.enmeshed.eu"
        };
    }

    public static TestData WithInvalidChecksum()
    {
        return new TestData
        {
            Address = "did:web:prod.enmeshed.eu:dids:id08uSgVGTSNqECvt1DJM3bZg6U8p6RSiott",
            PublicKey = Convert.FromBase64String("tB9KFp/YqHrom3m5qUuZsd6l30DkaNjN14SxRw7YZuI="),
            Realm = "id0",
            Checksum = "iott",
            MainPart = "8uSgVGTSNqECvt1DJM3bZg6U8p6RS",
            InstanceUrl = "prod.enmeshed.eu"
        };
    }
}
