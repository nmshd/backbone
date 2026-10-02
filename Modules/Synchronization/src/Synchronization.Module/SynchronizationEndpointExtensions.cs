using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Synchronization.Module.Features.Datawallets.GetDatawallet;
using Backbone.Modules.Synchronization.Module.Features.Datawallets.ListModifications;
using Backbone.Modules.Synchronization.Module.Features.Datawallets.PushDatawalletModifications;
using Backbone.Modules.Synchronization.Module.Features.SyncRuns.FinalizeDatawalletVersionUpgrade;
using Backbone.Modules.Synchronization.Module.Features.SyncRuns.FinalizeExternalEventSync;
using Backbone.Modules.Synchronization.Module.Features.SyncRuns.GetSyncRunById;
using Backbone.Modules.Synchronization.Module.Features.SyncRuns.ListExternalEventsOfSyncRun;
using Backbone.Modules.Synchronization.Module.Features.SyncRuns.RefreshExpirationTimeOfSyncRun;
using Backbone.Modules.Synchronization.Module.Features.SyncRuns.StartSyncRun;
using Microsoft.AspNetCore.Routing;
using OpenIddict.Validation.AspNetCore;

namespace Backbone.Modules.Synchronization.Module;

public static class SynchronizationEndpointExtensions
{
    public static IEndpointRouteBuilder MapSynchronizationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var datawallet = endpoints.MapVersionedEndpointGroup("Datawallet", "Datawallet", 2, OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        datawallet.MapGetDatawalletEndpoint();
        datawallet.MapListModificationsEndpoint();
        datawallet.MapPushDatawalletModificationsEndpoint();

        var syncRuns = endpoints.MapVersionedEndpointGroup("SyncRuns", "SyncRuns", 2, OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        syncRuns.MapStartSyncRunEndpoint();
        syncRuns.MapFinalizeExternalEventSyncEndpoint();
        syncRuns.MapFinalizeDatawalletVersionUpgradeEndpoint();
        syncRuns.MapListExternalEventsOfSyncRunEndpoint();
        syncRuns.MapGetSyncRunByIdEndpoint();
        syncRuns.MapRefreshExpirationTimeOfSyncRunEndpoint();

        return endpoints;
    }
}
