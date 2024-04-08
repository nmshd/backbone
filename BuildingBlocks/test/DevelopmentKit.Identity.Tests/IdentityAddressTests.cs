using Backbone.DevelopmentKit.Identity.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Backbone.DevelopmentKit.Identity.Tests;

public class IdentityAddressTests
{
    [Theory]
    [InlineData("fj0o9eOiPRswTZL6j9lE9TRvpDDnPRMF0gJeahz/W2c=", "did:web:prod.enmeshed.eu:dids:DdxuctYSJmWu7LHx73GF3EULbrVKf7p9u")]
    [InlineData("jRxGfZtQ8a90TmKCGk+dhuX1CBjgoXuldhNPwrjpWsw=", "did:web:prod.enmeshed.eu:dids:5zVGACUqWzohe9gmsHFTwFPFUPFqoZeKo")]
    [InlineData("PEODpwvi7KxIVa4qeUXia9apMFvPMktdDHiDitlfbjE=", "did:web:prod.enmeshed.eu:dids:4CzeVnFPPJh2u5MEcfe4ENEnubMoSDK6z")]
    [InlineData("mJGmNbxiVZAPToRuk9O3NvdfsWl6V+7wzIc+/57bU08=", "did:web:prod.enmeshed.eu:dids:Kkce9CYCVRYVxm8aXjwRHPW2WGp4GgGaE")]
    [InlineData("l68K/zdNp1VLoswcHAqN6QUFwCMU6Yvzf7XiW2m1hRY=", "did:web:prod.enmeshed.eu:dids:Pv6RRdoY48HrZKrUtmbhby3PtQXaAjJyK")]
    [InlineData("Gl8XTo8qFuUM+ksXixwp4g/jf3H/hU1F8ETuYaHCM5I=", "did:web:prod.enmeshed.eu:dids:8XovpZKTVYWjvaeZniPQJDitwJwpcA3oH")]
    [InlineData("rIS4kAzHXT7GgCA6Qm1ANlwM3x12QMSkeprHb6tjPyc=", "did:web:prod.enmeshed.eu:dids:A8kDLHmFmoidbHvSAuuSLNgmao8F3V3CL")]
    [InlineData("hg/cbeBvfNrMiJ0dW1AtWC4IQwG4gkuhzG2+z6bAoRU=", "did:web:prod.enmeshed.eu:dids:PyvoZbDu2GSxvvvziiJF4iHUK6pzqtU25")]
    [InlineData("kId+qWen/lKeTdyxcIQhkzvvvTU8wIJECfWUWbmRQRY=", "did:web:prod.enmeshed.eu:dids:6p64qm2VdqwADRPpp1vs5Tvz8gyM8fMTe")]
    [InlineData("NcqlzTEpSlKX9gmNBv41EjPRHpaNYwt0bxqh1bgyJzA=", "did:web:prod.enmeshed.eu:dids:MTkcyw1T29xwRqHjSsAMrY4HvjHFALPfJ")]
    [InlineData("49fWA+kzWNdCFdo92imTiQ4vUUJsPPLNlcB9udC4ooE=", "did:web:prod.enmeshed.eu:dids:H4Qxm4Q2yA7yzNMAM3cB52T6AdYmRUdzo")]
    public void AddressIsCreatedCorrectly2(string publicKey, string expectedAddress)
    {
        var address = IdentityAddress.Create(Convert.FromBase64String(publicKey), "prod.enmeshed.eu");

        address.StringValue.Should().Be(expectedAddress);
    }

    [Fact]
    public void AddressIsCreatedCorrectly()
    {
        var testData = TestData.Valid();
        var address = IdentityAddress.Create(testData.PublicKey, testData.InstanceUrl);

        address.Should().Be(testData.Address);
    }

    [Fact]
    public void EfConverterWorksCorrectly()
    {
        var testData = TestData.Valid();
        var address = IdentityAddress.Create(testData.PublicKey, testData.InstanceUrl);

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
    public required string InstanceUrl { get; set; }
    public required byte[] PublicKey { get; set; }

    public required string Address { get; set; }

    public required string Checksum { get; set; }
    public required string MainPart { get; set; }

    public static TestData Valid()
    {
        return new TestData
        {
            Address = "did:web:prod.enmeshed.eu:dids:CjRABNgfk4bhiu1CkceD5VgRhSsYbE4g8",
            PublicKey = Convert.FromBase64String("tB9KFp/YqHrom3m5qUuZsd6l30DkaNjN14SxRw7YZuI="),
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
            Checksum = "iott",
            MainPart = "8uSgVGTSNqECvt1DJM3bZg6U8p6RS",
            InstanceUrl = "prod.enmeshed.eu"
        };
    }
}
