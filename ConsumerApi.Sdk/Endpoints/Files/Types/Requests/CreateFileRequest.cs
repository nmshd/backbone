using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Files.Types.Requests;

public class CreateFileRequest
{
    public required FileData Content { get; set; }
    public required string Owner { get; set; }
    public required string OwnerSignature { get; set; }

    public required string CipherHash { get; set; }

    public required DateTime ExpiresAt { get; set; }

    public required string EncryptedProperties { get; set; }

    public MultipartFormDataContent ToMultipartContent()
    {
        MultipartFormDataContent content = [];

        if (Content.IsRealFile) content.Add(new StreamContent(Content.ReadStream), "content", Content.FilePath);
        else content.Add(new StreamContent(Content.ReadStream), "content");

        content.Add(new StringContent(Owner), "owner");
        content.Add(new StringContent(OwnerSignature), "ownerSignature");
        content.Add(new StringContent(CipherHash), "cipherHash");
        content.Add(new StringContent(ExpiresAt.ToString(CultureInfo.InvariantCulture)), "expiresAt"); //TODO: Specify Date
        content.Add(new StringContent(EncryptedProperties), "encryptedProperties");

        return content;
    }
}

public class FileData
{
    private readonly string? _filePath;
    private readonly Stream? _stream;

    public FileData(string filePath)
    {
        _filePath = filePath;
        _stream = null;
    }

    public FileData(Stream stream)
    {
        _filePath = null;
        _stream = stream;
    }

    [MemberNotNullWhen(true, nameof(_filePath))]
    [MemberNotNullWhen(false, nameof(_stream))]
    public bool IsRealFile => _filePath != null;

    [MemberNotNullWhen(false, nameof(_filePath))]
    [MemberNotNullWhen(true, nameof(_stream))]
    public bool IsStream => !IsRealFile;

    public Stream ReadStream => IsStream ? _stream : File.OpenRead(_filePath);
    public string FilePath => _filePath ?? "";
}
