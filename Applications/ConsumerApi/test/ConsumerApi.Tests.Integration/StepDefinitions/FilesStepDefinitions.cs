using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Requests;
using Backbone.ConsumerApi.Tests.Integration.Contexts;
using Backbone.ConsumerApi.Tests.Integration.Helpers;
using Backbone.Modules.Files.Domain.Entities;

namespace Backbone.ConsumerApi.Tests.Integration.StepDefinitions;

[Binding]
internal class FilesStepDefinitions
{
    private readonly ResponseContext _responseContext;
    private readonly FilesContext _filesContext;

    private readonly OwnershipTokensContext _fileOwnershipTokensContext;
    private readonly ClientPool _clientPool;
    private ApiResponse<string>? _resetOwnershipTokenResponse;
    private ApiResponse<bool> _validateOwnershipTokenResponse = null!;
    private ApiResponse<string>? _claimFileOwnershipResponse;

    public FilesStepDefinitions(ResponseContext responseContext, FilesContext filesContext, ClientPool clientPool, OwnershipTokensContext fileOwnershipTokensContext)
    {
        _responseContext = responseContext;
        _filesContext = filesContext;
        _clientPool = clientPool;
        _fileOwnershipTokensContext = fileOwnershipTokensContext;
    }

    #region Given

    [Given($"File {RegexFor.SINGLE_THING} created by {RegexFor.SINGLE_THING}")]
    public async Task GivenFileCreatedByI(string fileName, string identityName)
    {
        var client = _clientPool.FirstForIdentityName(identityName);
        var createResult = await Utils.CreateFile(client);
        var metadata = await client.Files.GetFileMetadata(createResult.Id);
        _filesContext.Files[fileName] = metadata.Result!;
        _fileOwnershipTokensContext.FileNameToOwnershipToken[fileName] = createResult.OwnershipToken;
    }

    [Given($"{RegexFor.SINGLE_THING} tries to claim {RegexFor.SINGLE_THING} with a wrong token")]
    public void GivenITriesToClaimFWithAWrongToken(string userName, string fileName)
    {
        var client = _clientPool.FirstForIdentityName(userName);
        var file = _filesContext.Files[fileName];
        var request = new ClaimFileOwnershipRequest { FileOwnershipToken = "wrongTokenXXXXXXXXXX" };
        _responseContext.WhenResponse = client.Files.ClaimFileOwnership(file.Id, request).Result;
    }

    #endregion

    #region When

    [When($"{RegexFor.SINGLE_THING} sends a POST request to the /Files endpoint")]
    public async Task WhenIdentitySendsAPostRequestToTheFilesEndpoint(string identityName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);

        var createFileRequest = new CreateFileRequest
        {
            Content = new MemoryStream("content"u8.ToArray()),
            Owner = identity.IdentityData!.Address,
            OwnerSignature = TestData.SOME_BASE64_STRING,
            CipherHash = TestData.SOME_BASE64_STRING,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            EncryptedProperties = TestData.SOME_BASE64_STRING
        };

        _responseContext.WhenResponse = await identity.Files.UploadFile(createFileRequest);
    }

    [When($"{RegexFor.SINGLE_THING} sends a DELETE request to the /Files/{RegexFor.SINGLE_THING}.Id endpoint")]
    public async Task WhenISendsADeleteRequestToTheFilesIdEndpoint(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var file = _filesContext.Files[fileName];

        _responseContext.WhenResponse = await identity.Files.DeleteFile(file.Id);
    }

    [When($"{RegexFor.SINGLE_THING} sends a GET request to the /Files/{RegexFor.SINGLE_THING}.Id endpoint")]
    public async Task WhenISendsAGetRequestToTheFilesIdEndpoint(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var file = _filesContext.Files[fileName];

        _responseContext.WhenResponse = await identity.Files.GetFileMetadata(file.Id);
    }

    [When($"{RegexFor.SINGLE_THING} sends a PATCH request to the /Files/{RegexFor.SINGLE_THING}.Id/ClaimFileOwnership with token {RegexFor.SINGLE_THING}.OwnershipToken")]
    public async Task WhenISendsAPatchRequestToTheFilesFIdClaimFileOwnershipFOwnershipTokenEndpoint(string identityName, string fileName, string fileName2)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var fileId = _filesContext.Files.FirstOrDefault(f => f.Key == fileName).Value?.Id ?? fileName;
        var token = _fileOwnershipTokensContext.FileNameToOwnershipToken.FirstOrDefault(t => t.Key == fileName2).Value ?? "NonExistingTokenXXXX";
        var request = new ClaimFileOwnershipRequest { FileOwnershipToken = token };
        _responseContext.WhenResponse = _claimFileOwnershipResponse = await identity.Files.ClaimFileOwnership(fileId, request);

        //reload the file into the context - to resemble the currently stored stage
        var file = await identity.Files.GetFileMetadata(fileId);
        _filesContext.Files[fileName] = file.Result!;
        _fileOwnershipTokensContext.FileNameToOwnershipToken[fileName] = _claimFileOwnershipResponse.Result!;
    }

    [When($"{RegexFor.SINGLE_THING} sends a PATCH request to the /Files/{RegexFor.SINGLE_THING}.Id/ClaimFileOwnership with a malformed token")]
    public void WhenISendsApatchRequestToTheFilesFIdClaimFileOwnershipWithAMalformedToken(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var fileId = _filesContext.Files.FirstOrDefault(f => f.Key == fileName).Value?.Id ?? fileName;
        //more than 20 characters
        var request = new ClaimFileOwnershipRequest { FileOwnershipToken = "malformedTokenXXXXXXXXXXX" };
        _responseContext.WhenResponse = _claimFileOwnershipResponse = identity.Files.ClaimFileOwnership(fileId, request).Result;
    }

    [When($"{RegexFor.SINGLE_THING} sends a PATCH request to the /Files/{RegexFor.SINGLE_THING}.Id/RegenerateOwnershipToken endpoint")]
    public void WhenISendsAPatchRequestToTheFilesFIdRegenerateOwnershipTokenEndpoint(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var fileId = _filesContext.Files.FirstOrDefault(f => f.Key == fileName).Value?.Id ?? fileName;
        _responseContext.WhenResponse = _resetOwnershipTokenResponse = identity.Files.RegenerateFileOwnershipToken(fileId).Result;
        _fileOwnershipTokensContext.FileNameToOwnershipToken[fileName] = _resetOwnershipTokenResponse.Result ?? "";
    }

    [When($"{RegexFor.SINGLE_THING} sends a Post request to the /Files/{RegexFor.SINGLE_THING}.Id/ValidateOwnershipToken with token {RegexFor.SINGLE_THING}.OwnershipToken")]
    public async Task WhenISendsAPostRequestToTheFilesFIdValidateOwnershipTokenFOwnershipTokenWithTokenFOwnershipTokenEndpoint(string identityName, string fileName, string fileName2)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        var fileId = _filesContext.Files.FirstOrDefault(f => f.Key == fileName).Value?.Id ?? fileName;
        var token = _fileOwnershipTokensContext.FileNameToOwnershipToken.FirstOrDefault(t => t.Key == fileName2).Value ?? "NonExistingTokenXXXX";
        var request = new ValidateFileOwnershipTokenRequest { FileOwnershipToken = token };
        _responseContext.WhenResponse = _validateOwnershipTokenResponse = await identity.Files.ValidateFileOwnershipToken(fileId, request);
    }

    #endregion

    #region Then

    [Then($"the response contains the new OwnershipToken of {RegexFor.SINGLE_THING}")]
    public void ThenTheResponseContainsTheNewOwnershipTokenOfF(string fileName)
    {
        var responseToken = "";
        if (_claimFileOwnershipResponse != null)
        {
            FileOwnershipToken.IsValid(_claimFileOwnershipResponse.Result!).Should().BeTrue();
            responseToken = _claimFileOwnershipResponse.Result!;
        }
        else if (_resetOwnershipTokenResponse != null)
        {
            FileOwnershipToken.IsValid(_resetOwnershipTokenResponse.Result!).Should().BeTrue();
            responseToken = _resetOwnershipTokenResponse.Result!;
        }
        else
            Assert.Fail("No OwnershipToken was returned");

        _fileOwnershipTokensContext.FileNameToOwnershipToken[fileName].Should().BeEquivalentTo(responseToken);
    }

    [Then($"the ValidateOwnershipTokenResponse is (true|false)")]
    public void ThenTheValidateOwnershipTokenResponseIsTrue(string expected)
    {
        _validateOwnershipTokenResponse.Should().NotBeNull();
        _validateOwnershipTokenResponse.Result.Should().Be(expected == "true");
    }

    [Then($"{RegexFor.SINGLE_THING} is the new owner of {RegexFor.SINGLE_THING}")]
    public async Task ThenIIsTheNewOwnerOfF(string identityName, string fileName)
    {
        var identity = _clientPool.FirstForIdentityName(identityName);
        //the file needs to be reloaded to get the latest owner
        var file = _filesContext.Files[fileName];
        var reloadedFile = await identity.Files.GetFileMetadata(file.Id);
        _filesContext.Files[fileName] = reloadedFile.Result!;

        identity.IdentityData.Should().NotBeNull();
        identity.IdentityData!.Address.Should().NotBeNullOrEmpty();
        identity.IdentityData!.Address.Should().Be(_filesContext.Files[fileName].Owner);
    }

    [Then($"it is (true|false), that the file {RegexFor.SINGLE_THING} has a locked ownership")]
    public void ThenTheFileFIsBlockedForOwnershipClaimsIsTrue(string fileName, string expected)
    {
        var file = _filesContext.Files[fileName] ?? null;
        file.Should().NotBeNull();
        file!.FileOwnershipIsLocked.Should().Be(expected == "true");
    }

    #endregion
}
