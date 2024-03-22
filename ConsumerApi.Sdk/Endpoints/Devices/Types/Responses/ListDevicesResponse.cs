﻿using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Devices.Types.Responses;

public class ListDevicesResponse(IEnumerable<Device> items) : EnumerableResponseBase<Device>(items);
