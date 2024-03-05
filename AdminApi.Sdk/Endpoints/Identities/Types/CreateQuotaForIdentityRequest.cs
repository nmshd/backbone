﻿namespace Backbone.AdminApi.Sdk.Endpoints.Identities.Types;

public class CreateQuotaForIdentityRequest
{
    public required string MetricKey { get; set; }
    public required int Max { get; set; }
    public required string Period { get; set; }
}
