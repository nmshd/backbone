﻿using Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;

namespace Backbone.AdminCli.Commands.Database.Model;

public class DeviceExport
{
    public required string DeviceId { get; set; } = null!;
    public required DateTime? LastLoginAt { get; set; }
    public required string IdentityAddress { get; set; } = null!;
    public required DateTime CreatedAt { get; set; }
    public required string Tier { get; set; } = null!;
    public required IdentityStatus IdentityStatus { get; set; }
    public required DateTime? IdentityDeletionGracePeriodEndsAt { get; set; }
    public required string? ClientId { get; set; }
    public required string? ClientName { get; set; }
    public required PushNotificationPlatform? Platform { get; set; } = null!;
}
