﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable enable

namespace AdminUi.Infrastructure.CompiledModels.SqlServer
{
    public partial class AdminUiDbContextModel
    {
        partial void Initialize()
        {
            var identityOverview = IdentityOverviewEntityType.Create(this);
            var tierOverview = TierOverviewEntityType.Create(this);

            IdentityOverviewEntityType.CreateAnnotations(identityOverview);
            TierOverviewEntityType.CreateAnnotations(tierOverview);

            AddAnnotation("ProductVersion", "7.0.10");
            AddAnnotation("Relational:MaxIdentifierLength", 128);
            AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }
    }
}
