﻿using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.PushDatawalletModifications;

public class PushDatawalletModificationsResponse
{
    public long NewIndex { get; set; }

    public IEnumerable<PushDatawalletModificationsResponseItem> Modifications { get; set; }
}

public class PushDatawalletModificationsResponseItem : IMapTo<DatawalletModification>
{
    public DatawalletModificationId Id { get; set; }
    public long Index { get; set; }
    public DateTime CreatedAt { get; set; }
}
