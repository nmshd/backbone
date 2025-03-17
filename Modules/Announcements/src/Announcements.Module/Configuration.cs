﻿using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database;

namespace Backbone.Modules.Announcements.Module;

public class InfrastructureConfiguration
{
    [Required]
    public IServiceCollectionExtensions.DbOptions SqlDatabase { get; set; } = new();
}
