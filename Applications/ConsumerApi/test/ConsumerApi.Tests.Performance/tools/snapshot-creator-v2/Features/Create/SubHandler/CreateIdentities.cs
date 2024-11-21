﻿using System.Diagnostics;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.Tooling;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public abstract record CreateIdentities
{
    public record Command(
        List<IdentityPoolConfiguration> IdentityPoolConfigurations,
        string BaseUrlAddress,
        ClientCredentials ClientCredentials) : IRequest<List<DomainIdentity>>;

    // ReSharper disable once UnusedMember.Global - Invoked via IMediator 
    public record CommandHandler(ILogger<CreateIdentities> Logger) : IRequestHandler<Command, List<DomainIdentity>>
    {
        public async Task<List<DomainIdentity>> Handle(Command request, CancellationToken cancellationToken)
        {
            var identities = new List<DomainIdentity>();

            var identityConfigurations = request.IdentityPoolConfigurations
                .SelectMany(identityPoolConfiguration => identityPoolConfiguration.Identities)
                .ToList();

            var totalIdentities = identityConfigurations.Count;
            var numberOfCreatedIdentities = 0;

            foreach (var identityConfiguration in identityConfigurations)
            {
                Stopwatch stopwatch = new();
                stopwatch.Start();
                var createdIdentity = await CreateIdentity(request, identityConfiguration);
                stopwatch.Stop();

                numberOfCreatedIdentities++;
                Logger.LogDebug("Created {CreatedIdentities}/{TotalIdentities} identities. Identity {Address}/{ConfigurationAddress}/{Pool} added in {ElapsedMilliseconds} ms",
                    numberOfCreatedIdentities,
                    totalIdentities,
                    createdIdentity.IdentityAddress,
                    createdIdentity.ConfigurationIdentityAddress,
                    createdIdentity.PoolAlias,
                    stopwatch.ElapsedMilliseconds);

                identities.Add(createdIdentity);
            }

            return identities;
        }

        private static async Task<DomainIdentity> CreateIdentity(Command request, IdentityConfiguration identityConfiguration)
        {
            var sdkClient = await Client.CreateForNewIdentity(request.BaseUrlAddress, request.ClientCredentials, PasswordHelper.GeneratePassword(18, 24));

            if (sdkClient.DeviceData is null)
                throw new InvalidOperationException(
                    $"The sdkClient.DeviceData is null. Could not be used to create a new database Identity for config {identityConfiguration.Address}/{identityConfiguration.PoolAlias} [IdentityAddress/Pool]");

            var createdIdentity = new DomainIdentity(
                sdkClient.DeviceData.UserCredentials,
                sdkClient.IdentityData,
                identityConfiguration.Address,
                identityConfiguration.NumberOfDevices,
                identityConfiguration.NumberOfRelationshipTemplates,
                identityConfiguration.IdentityPoolType,
                identityConfiguration.NumberOfChallenges,
                identityConfiguration.PoolAlias,
                identityConfiguration.NumberOfDatawalletModifications);

            if (string.IsNullOrWhiteSpace(sdkClient.DeviceData.DeviceId))
            {
                throw new InvalidOperationException(
                    $"The sdkClient.DeviceData.DeviceId is null or empty. Could not be used to create a new database Device for config {identityConfiguration.Address}/{identityConfiguration.PoolAlias} [IdentityAddress/Pool]");
            }

            createdIdentity.AddDevice(sdkClient.DeviceData.DeviceId);
            return createdIdentity;
        }
    }
}
