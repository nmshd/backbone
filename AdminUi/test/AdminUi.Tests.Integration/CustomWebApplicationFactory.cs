using Backbone.Crypto.Abstractions;
using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Tooling.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backbone.AdminUi.Tests.Integration;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            if (Environment.GetEnvironmentVariable("CI").IsNullOrEmpty())
                config.AddJsonFile("api.appsettings.local.override.json");
        });

        builder.ConfigureServices(services =>
        {
            var challengeValidator = services.SingleOrDefault(s => s.ServiceType == typeof(ChallengeValidator));
            services.Remove(challengeValidator!);

            services.AddScoped<ChallengeValidator, CustomChallengeValidator>();
        });

        return base.CreateHost(builder);
    }
}

public class CustomChallengeValidator : ChallengeValidator
{
    public CustomChallengeValidator(ISignatureHelper signatureHelper, IChallengesRepository challengesRepository) : base(signatureHelper, challengesRepository)
    {
    }

    public override Task Validate(SignedChallengeDTO signedChallenge, PublicKey publicKey)
    {
        return Task.CompletedTask;
    }
}
