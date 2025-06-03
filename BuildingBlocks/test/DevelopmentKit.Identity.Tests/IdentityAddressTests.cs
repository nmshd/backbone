using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.DevelopmentKit.Identity.Tests;

public class IdentityAddressTests : AbstractTestsBase
{
    [Theory]
    // ReSharper disable StringLiteralTypo
    [InlineData("49fWA+kzWNdCFdo92imTiQ4vUUJsPPLNlcB9udC4ooE=", "did:e:prod.enmeshed.eu:dids:06a391378e5df5c1399f77")]
    [InlineData("fj0o9eOiPRswTZL6j9lE9TRvpDDnPRMF0gJeahz/W2c=", "did:e:prod.enmeshed.eu:dids:fef1992c5e529adc413288")]
    [InlineData("Gl8XTo8qFuUM+ksXixwp4g/jf3H/hU1F8ETuYaHCM5I=", "did:e:prod.enmeshed.eu:dids:01f4bab09d757578bb4994")]
    [InlineData("hg/cbeBvfNrMiJ0dW1AtWC4IQwG4gkuhzG2+z6bAoRU=", "did:e:prod.enmeshed.eu:dids:ab7475ba4070f29ce286fd")]
    [InlineData("jRxGfZtQ8a90TmKCGk+dhuX1CBjgoXuldhNPwrjpWsw=", "did:e:prod.enmeshed.eu:dids:b9d25bd0a2bbd3aa48437c")]
    [InlineData("kId+qWen/lKeTdyxcIQhkzvvvTU8wIJECfWUWbmRQRY=", "did:e:prod.enmeshed.eu:dids:4664f42d7ca6480db07fdb")]
    [InlineData("l68K/zdNp1VLoswcHAqN6QUFwCMU6Yvzf7XiW2m1hRY=", "did:e:prod.enmeshed.eu:dids:5845cf29fbda2897892a9a")]
    [InlineData("mJGmNbxiVZAPToRuk9O3NvdfsWl6V+7wzIc+/57bU08=", "did:e:prod.enmeshed.eu:dids:e2208784ee2769c5d9684d")]
    [InlineData("NcqlzTEpSlKX9gmNBv41EjPRHpaNYwt0bxqh1bgyJzA=", "did:e:prod.enmeshed.eu:dids:60326ff5075e0d7378990c")]
    [InlineData("PEODpwvi7KxIVa4qeUXia9apMFvPMktdDHiDitlfbjE=", "did:e:prod.enmeshed.eu:dids:d459ff2144f0eac7aff554")]
    [InlineData("rIS4kAzHXT7GgCA6Qm1ANlwM3x12QMSkeprHb6tjPyc=", "did:e:prod.enmeshed.eu:dids:ee5966a158f1dc4de5bd5c")]
    // ReSharper enable StringLiteralTypo
    public void AddressIsCreatedCorrectly2(string publicKey, string expectedAddress)
    {
        var address = IdentityAddress.Create(Convert.FromBase64String(publicKey), "prod.enmeshed.eu");

        address.Value.ShouldBe(expectedAddress);
    }

    [Theory]
    // ReSharper disable StringLiteralTypo
    [InlineData("did:e:enmeshedeu:dids:06a391378e5df5c1399f77")]
    [InlineData("did:e:prod.ENMESHED.eu:dids:fef1992c5e529adc413288")]
    [InlineData("ID154565465468435134684648ffef1992ca5e529adc413288")]
    [InlineData("dod:e:prod.enmeshed.eu:dids:ee5966a158f1dc4de5bd5c")]
    [InlineData("did:e:prod.enmeshed.eu:dids:nonhexchars11")]
    [InlineData("did:e:prod.enmeshed.eu:dids:eCa432178Ca417a1311")] // capital letters
    [InlineData("did:e:prod.enmeshed.eu:dids:ee5966a158f1dc4de5bd5cee5966a158f1dc4de5bd5c")]
    // ReSharper enable StringLiteralTypo
    public void IsValidReturnsFalseForInvalidAddress(string identityAddress)
    {
        IdentityAddress.IsValid(identityAddress).ShouldBeFalse();
    }

    [Fact]
    public void AddressIsCreatedCorrectly()
    {
        var testData = TestData.Valid();
        var address = IdentityAddress.Create(testData.PublicKey, testData.DidDomainName);

        address.Value.ShouldBe(testData.Address);
    }

    [Fact]
    public void EfConverterWorksCorrectly()
    {
        var testData = TestData.Valid();
        var address = IdentityAddress.Create(testData.PublicKey, testData.DidDomainName);

        address.Value.ShouldBe(testData.Address);
    }

    [Fact]
    public void ValidAddressesAreAccepted()
    {
        var testData = TestData.Valid();
        var isValid = IdentityAddress.IsValid(testData.Address);

        isValid.ShouldBeTrue();
    }

    [Fact]
    public void DashesAreAllowedCharactersInDomainNames()
    {
        var isValid = IdentityAddress.IsValid("did:e:bkb-nmshd-preprod.nbpdev.de:dids:a75bf465d17e972367a986");

        isValid.ShouldBeTrue();
    }

    [Fact]
    public void AddressesWithInvalidChecksumAreDeclined()
    {
        var testData = TestData.WithInvalidChecksum();
        var isValid = IdentityAddress.IsValid(testData.Address);

        isValid.ShouldBeFalse();
    }

    [Fact]
    public void AddressesWithInvalidMainPartAreDeclined()
    {
        var testData = TestData.WithInvalidMainPart();
        var isValid = IdentityAddress.IsValid(testData.Address);

        isValid.ShouldBeFalse();
    }
}

internal class TestData
{
    public required string DidDomainName { get; set; }
    public required byte[] PublicKey { get; set; }

    public required string Address { get; set; }

    public required string Checksum { get; set; }

    public static TestData Valid()
    {
        return new TestData
        {
            Address = "did:e:prod.enmeshed.eu:dids:56b3f2a0c202e27229aa87",
            PublicKey = Convert.FromBase64String("tB9KFp/YqHrom3m5qUuZsd6l30DkaNjN14SxRw7YZuI="),
            Checksum = "87",
            DidDomainName = "prod.enmeshed.eu"
        };
    }

    public static TestData WithInvalidMainPart()
    {
        return new TestData
        {
            Address = "did:e:prod.enmeshed.eu:dids:56b3f2a0c202e27d39aa87",
            PublicKey = Convert.FromBase64String("tB9KFp/YqHrom3m5qUuZsd6l30DkaNjN14SxRw7YZuI="),
            Checksum = "87",
            DidDomainName = "prod.enmeshed.eu"
        };
    }

    public static TestData WithInvalidChecksum()
    {
        return new TestData
        {
            Address = "did:e:prod.enmeshed.eu:dids:56b3f2a0c202e27229aa55",
            PublicKey = Convert.FromBase64String("tB9KFp/YqHrom3m5qUuZsd6l30DkaNjN14SxRw7YZuI="),
            Checksum = "55",
            DidDomainName = "prod.enmeshed.eu"
        };
    }
}
