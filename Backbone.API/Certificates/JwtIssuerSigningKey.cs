using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Backbone.API.Configuration;
using Enmeshed.Tooling.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace Backbone.API.Certificates;

public class JwtIssuerSigningKey
{
    public static SecurityKey Get(BackboneConfiguration.AuthenticationConfiguration configuration, IHostEnvironment env)
    {
        X509Certificate2 certificate;

        if (env.IsLocal())
        {
            certificate = ReadCertificateFromFile();
        }
        else
        {
            var signingCertificate = configuration.JwtSigningCertificate;

            if (string.IsNullOrEmpty(signingCertificate))
                throw new Exception("JWT Signing Key not found.");

            certificate = ParseCertificateBase64(signingCertificate);
        }

        return new X509SecurityKey(certificate);
    }

    private static X509Certificate2 ParseCertificateBase64(string base64Certificate)
    {
        var privateKeyBytes = Convert.FromBase64String(base64Certificate);
        return new X509Certificate2(privateKeyBytes);
    }

    private static X509Certificate2 ReadCertificateFromFile()
    {
        var assembly = typeof(JwtIssuerSigningKey).GetTypeInfo().Assembly;
        using var stream =
            assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Certificates.jwt-issuer-signing-key.cer");

        if (stream == null)
            throw new Exception("JWT Signing Certificate not found.");

        var bytes = ReadStream(stream);
        return new X509Certificate2(bytes);
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