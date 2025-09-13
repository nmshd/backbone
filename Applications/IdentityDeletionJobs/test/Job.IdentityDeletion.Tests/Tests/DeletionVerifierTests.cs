using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Job.IdentityDeletion.IdentityDeletionVerifier;
using Backbone.Job.IdentityDeletion.Tests.Tests.DummyClasses;

namespace Backbone.Job.IdentityDeletion.Tests.Tests;

public class DeletionVerifierTests : AbstractTestsBase
{
    [Fact]
    public async Task Empty_db_succeeds()
    {
        // Arrange
        var identity = CreateRandomIdentityAddress();
        var deletionVerifier = CreateDeletionVerifier([]);

        // Act
        var result = await deletionVerifier.VerifyDeletion([identity], CancellationToken.None);

        // Assert
        result.Success.ShouldBeTrue();
    }

    [Fact]
    public async Task Addresses_to_verify_not_found_succeeds()
    {
        // Arrange
        List<string> identitiesToVerify = [CreateRandomIdentityAddress(), CreateRandomIdentityAddress()];
        var deletionVerifier = CreateDeletionVerifier([
            Line.Single(CreateRandomIdentityAddress()),
            Line.Concatenated(CreateRandomIdentityAddress(), CreateRandomIdentityAddress()),
            Line.DataSingle(CreateRandomIdentityAddress()),
            Line.DataConcatenated(CreateRandomIdentityAddress(), CreateRandomIdentityAddress())
        ]);

        // Act
        var result = await deletionVerifier.VerifyDeletion(identitiesToVerify, CancellationToken.None);

        // Assert
        result.Success.ShouldBeTrue();
    }

    [Fact]
    public async Task A_simple_found_identity_to_verify_fails()
    {
        // Arrange
        var identityToVerify = CreateRandomIdentityAddress();
        var deletionVerifier = CreateDeletionVerifier([
            Line.Single(identityToVerify)
        ]);

        // Act
        var result = await deletionVerifier.VerifyDeletion([identityToVerify], CancellationToken.None);

        // Assert
        result.Success.ShouldBeFalse();
        result.NumberOfOccurrences.ShouldBe(1);
        result.FoundOccurrences.ShouldContainKey(DummySqlExtractor.TEST_ID);
        result.FoundOccurrences[DummySqlExtractor.TEST_ID].ShouldContainKeyAndValue(identityToVerify, 1);
    }

    [Fact]
    public async Task A_concatenated_found_identity_to_verify_fails()
    {
        // Arrange
        var identityToVerify = CreateRandomIdentityAddress();
        var deletionVerifier = CreateDeletionVerifier([
            Line.Concatenated(identityToVerify, CreateRandomIdentityAddress())
        ]);

        // Act
        var result = await deletionVerifier.VerifyDeletion([identityToVerify], CancellationToken.None);

        // Assert
        result.Success.ShouldBeFalse();
        result.NumberOfOccurrences.ShouldBe(1);
        result.FoundOccurrences.ShouldContainKey(DummySqlExtractor.TEST_ID);
        result.FoundOccurrences[DummySqlExtractor.TEST_ID].ShouldContainKeyAndValue(identityToVerify, 1);
    }

    [Fact]
    public async Task A_found_identity_to_verify_in_a_data_string_fails()
    {
        // Arrange
        var identityToVerify = CreateRandomIdentityAddress();
        var deletionVerifier = CreateDeletionVerifier([
            Line.DataSingle(identityToVerify)
        ]);

        // Act
        var result = await deletionVerifier.VerifyDeletion([identityToVerify], CancellationToken.None);

        // Assert
        result.Success.ShouldBeFalse();
        result.NumberOfOccurrences.ShouldBe(1);
        result.FoundOccurrences.ShouldContainKey(DummySqlExtractor.TEST_ID);
        result.FoundOccurrences[DummySqlExtractor.TEST_ID].ShouldContainKeyAndValue(identityToVerify, 1);
    }

    private static DeletionVerifier CreateDeletionVerifier(List<string> lines) => new(new DummyDbExporter(), new DummySqlExtractor(lines));
}

file static class Line
{
    public static string Single(IdentityAddress address) => address.Value;
    public static string Concatenated(IdentityAddress a, IdentityAddress b) => $"{a.Value}{b.Value}";
    public static string DataSingle(IdentityAddress address) => $"0\tabc\t\\N\t\\\\x41414141\t{address.Value}\t0";
    public static string DataConcatenated(IdentityAddress a, IdentityAddress b) => $"0\tabc\t\\N\t\\\\x41414141\t{a.Value}{b.Value}\t0";
}
