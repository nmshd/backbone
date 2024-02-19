﻿using System.ComponentModel.DataAnnotations;
using Backbone.Tooling.Extensions;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;

public class DirectPnsCommunicationOptions
{
    [Required]
    public required FcmOptions Fcm { get; set; } = null!;

    [Required]
    public required ApnsOptions Apns { get; set; } = null!;

    public class FcmOptions
    {
        [Required]
        [MinLength(1)]
        public required Dictionary<string, ServiceAccount> ServiceAccounts { get; set; } = [];

        [Required]
        [MinLength(1)]
        public required Dictionary<string, ServiceAccountInformation> Apps { get; set; } = [];

        public class ServiceAccountInformation
        {
            [Required]
            [MinLength(1)]
            public string ServiceAccountName { get; set; } = string.Empty;
        }

        public bool HasConfigForAppId(string appId)
        {
            var app = Apps.GetValueOrDefault(appId);

            if (app == null) return false;

            var serviceAccountForServiceAccountNameExists = ServiceAccounts.ContainsKey(app.ServiceAccountName);

            return serviceAccountForServiceAccountNameExists;
        }

        public string? GetServiceAccountForAppId(string appId)
        {
            var app = Apps.GetValueOrDefault(appId);

            if (app == null)
                return null;

            var serviceAccount = ServiceAccounts.GetValueOrDefault(app.ServiceAccountName);

            return serviceAccount?.ServiceAccountJson;
        }

        public List<string> GetSupportedAppIds()
        {
            return Apps.Where(app => HasConfigForAppId(app.Key)).Select(app => app.Key).ToList();
        }

        public class ServiceAccount
        {
            [Required]
            [MinLength(1)]
            public required string ServiceAccountJson { get; set; } = string.Empty;
        }
    }

    public class ApnsOptions
    {
        [Required]
        [MinLength(1)]
        public required Dictionary<string, Key> Keys { get; set; } = new();

        [Required]
        [MinLength(1)]
        public required Dictionary<string, Bundle> Bundles { get; set; } = new();

        public bool HasConfigForBundleId(string bundleId)
        {
            var bundle = Bundles.GetValueOrDefault(bundleId);
            return bundle != null && !bundle.KeyName.IsNullOrEmpty() && Keys.ContainsKey(bundle.KeyName) && !Keys[bundle.KeyName].PrivateKey.IsNullOrEmpty();
        }

        public Key GetKeyInformationForBundleId(string bundleId)
        {
            var bundle = Bundles.GetValueOrDefault(bundleId);
            return bundle == null ? throw new Exception($"No bundle configuration for bundle id '{bundleId}' was found.") : Keys[bundle.KeyName];
        }

        public Bundle GetBundleById(string bundleId)
        {
            return Bundles[bundleId];
        }

        public List<string> GetSupportedBundleIds()
        {
            return Bundles.Where(bundle => HasConfigForBundleId(bundle.Key)).Select(bundle => bundle.Key).ToList();
        }

        public class Bundle
        {
            [Required]
            [MinLength(1)]
            public required string KeyName { get; set; }
        }

        public class Key
        {
            [Required]
            [MinLength(1)]
            public required string TeamId { get; set; } = string.Empty;

            [Required]
            [MinLength(1)]
            public required string KeyId { get; set; } = string.Empty;

            [Required]
            [MinLength(1)]
            public required string PrivateKey { get; set; } = string.Empty;
        }
    }
}
