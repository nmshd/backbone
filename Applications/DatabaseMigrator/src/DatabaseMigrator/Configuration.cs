﻿using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

namespace Backbone.DatabaseMigrator;

public class Configuration
{
    [Required]
    public required InfrastructureConfiguration Infrastructure { get; init; }
}

public class InfrastructureConfiguration
{
    [Required]
    public required DatabaseConfiguration SqlDatabase { get; init; }
}
