using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Enmeshed.Tooling.Extensions;

namespace Devices.API.Certificates;

public static class Certificate
{
    public static X509Certificate2 Get(IConfiguration configuration)
    {
        var signingCertificateSource = configuration.GetJwtOptionsConfiguration().SigningCertificateSource;

        return signingCertificateSource switch
        {
            JwtOptionsConfiguration.SigningCertificateSourceEnum.Config => ReadCertificateFromConfiguration(configuration),
            JwtOptionsConfiguration.SigningCertificateSourceEnum.File => ReadCertificateFromFile(),
            _ => throw new Exception($"Signing certificate source {signingCertificateSource} is not supported.")
        };
    }

    private static X509Certificate2 ReadCertificateFromConfiguration(IConfiguration configuration)
    {
        var certificateFromConfig = configuration.GetJwtOptionsConfiguration().SigningCertificate;
        var privateKeyBytes = Convert.FromBase64String(certificateFromConfig);
        return new X509Certificate2(privateKeyBytes, (string) null);
    }

    private static X509Certificate2 ReadCertificateFromFile()
    {
        var assembly = typeof(Certificate).GetTypeInfo().Assembly;
        using var stream = assembly.GetManifestResourceStream("Devices.API.Certificates.idsrv3test.pfx");

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
