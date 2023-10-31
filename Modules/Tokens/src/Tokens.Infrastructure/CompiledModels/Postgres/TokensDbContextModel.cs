﻿// <auto-generated />

using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable enable

namespace Backbone.Modules.Tokens.Infrastructure.CompiledModels.Postgres
{
    [DbContext(typeof(TokensDbContext))]
    public partial class TokensDbContextModel : RuntimeModel
    {
        static TokensDbContextModel()
        {
            var model = new TokensDbContextModel();
            model.Initialize();
            model.Customize();
            _instance = model;
        }

        private static TokensDbContextModel _instance;
        public static IModel Instance => _instance;

        partial void Initialize();

        partial void Customize();
    }
}
