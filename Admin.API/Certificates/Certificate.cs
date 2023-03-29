using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Admin.API.Configuration;
using Enmeshed.Tooling.Extensions;

namespace Admin.API.Certificates;

public static class Certificate
{
    public static X509Certificate2 Get(AdminConfiguration.AuthenticationConfiguration configuration)
    {
        var signingCertificateSource = configuration.JwtSigningCertificateSource;

        return signingCertificateSource.ToLower() switch
        {
            "config" => ParseCertificateString(configuration.JwtSigningCertificate),
            "file" => ReadCertificateFromFile(),
            _ => throw new Exception($"Signing certificate source {signingCertificateSource} is not supported.")
        };
    }

    private static X509Certificate2 ParseCertificateString(string certificate)
    {
        var privateKeyBytes = Convert.FromBase64String(certificate);
        return new X509Certificate2(privateKeyBytes, (string?)null);
    }

    private static X509Certificate2 ReadCertificateFromFile()
    {
        var assembly = typeof(Certificate).GetTypeInfo().Assembly;
        using var stream = assembly.GetManifestResourceStream("Admin.API.Certificates.jwt-signing-certificate.pfx")!;

        var bytes = ReadStream(stream);
        return new X509Certificate2(bytes, "idsrv3test");
    }

    private static byte[] ReadStream(Stream input)
    {
        var buffer = new byte[16.Kibibytes()];

        using var ms = new MemoryStream();

        int read;
        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            ms.Write(buffer, 0, read);
        }

        return ms.ToArray();
    }
}
