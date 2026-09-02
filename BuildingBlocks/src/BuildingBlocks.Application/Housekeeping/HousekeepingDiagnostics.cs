using System.Diagnostics;

namespace Backbone.BuildingBlocks.Application.Housekeeping;

public static class HousekeepingDiagnostics
{
    public const string ACTIVITY_SOURCE_NAME = "Backbone.Housekeeping";

    public static readonly ActivitySource ACTIVITY_SOURCE = new(ACTIVITY_SOURCE_NAME);
}
