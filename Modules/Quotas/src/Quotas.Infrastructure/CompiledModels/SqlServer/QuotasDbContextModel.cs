﻿// <auto-generated />
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable enable

namespace Backbone.Modules.Quotas.Infrastructure.CompiledModels.SqlServer
{
    [DbContext(typeof(QuotasDbContext))]
    public partial class QuotasDbContextModel : RuntimeModel
    {
        static QuotasDbContextModel()
        {
            var model = new QuotasDbContextModel();
            model.Initialize();
            model.Customize();
            _instance = model;
        }

        private static QuotasDbContextModel _instance;
        public static IModel Instance => _instance;

        partial void Initialize();

        partial void Customize();
    }
}
