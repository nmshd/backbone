﻿using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Responses;

public class GetDatawalletModificationsResponse(IEnumerable<DatawalletModification> items) : EnumerableResponseBase<DatawalletModification>(items);
