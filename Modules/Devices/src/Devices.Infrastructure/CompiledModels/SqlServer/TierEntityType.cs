﻿// <auto-generated />
using System;
using System.Reflection;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#pragma warning disable 219, 612, 618
#nullable disable

namespace Backbone.Modules.Devices.Infrastructure.CompiledModels.SqlServer
{
    internal partial class TierEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "Backbone.Modules.Devices.Domain.Aggregates.Tier.Tier",
                typeof(Tier),
                baseEntityType);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(TierId),
                propertyInfo: typeof(Tier).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tier).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                maxLength: 20,
                unicode: false,
                valueConverter: new TierIdEntityFrameworkValueConverter());
            id.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<TierId>(
                    (TierId v1, TierId v2) => v1 == null && v2 == null || v1 != null && v2 != null && v1.Equals(v2),
                    (TierId v) => v.GetHashCode(),
                    (TierId v) => v),
                keyComparer: new ValueComparer<TierId>(
                    (TierId v1, TierId v2) => v1 == null && v2 == null || v1 != null && v2 != null && v1.Equals(v2),
                    (TierId v) => v.GetHashCode(),
                    (TierId v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "char(20)",
                    size: 20,
                    dbType: System.Data.DbType.AnsiStringFixedLength),
                converter: new ValueConverter<TierId, string>(
                    (TierId id) => id.Value,
                    (string value) => TierId.Create(value).Value),
                jsonValueReaderWriter: new JsonConvertedValueReaderWriter<TierId, string>(
                    JsonStringReaderWriter.Instance,
                    new ValueConverter<TierId, string>(
                        (TierId id) => id.Value,
                        (string value) => TierId.Create(value).Value)));
            id.AddAnnotation("Relational:IsFixedLength", true);
            id.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var name = runtimeEntityType.AddProperty(
                "Name",
                typeof(TierName),
                propertyInfo: typeof(Tier).GetProperty("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Tier).GetField("<Name>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 30,
                unicode: true,
                valueConverter: new TierNameEntityFrameworkValueConverter());
            name.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<TierName>(
                    (TierName v1, TierName v2) => v1 == null && v2 == null || v1 != null && v2 != null && v1.Equals(v2),
                    (TierName v) => v.GetHashCode(),
                    (TierName v) => v),
                keyComparer: new ValueComparer<TierName>(
                    (TierName v1, TierName v2) => v1 == null && v2 == null || v1 != null && v2 != null && v1.Equals(v2),
                    (TierName v) => v.GetHashCode(),
                    (TierName v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "nvarchar(30)",
                    size: 30,
                    dbType: System.Data.DbType.String),
                converter: new ValueConverter<TierName, string>(
                    (TierName id) => id.Value,
                    (string value) => TierName.Create(value).Value),
                jsonValueReaderWriter: new JsonConvertedValueReaderWriter<TierName, string>(
                    JsonStringReaderWriter.Instance,
                    new ValueConverter<TierName, string>(
                        (TierName id) => id.Value,
                        (string value) => TierName.Create(value).Value)));
            name.AddAnnotation("Relational:IsFixedLength", false);
            name.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { name },
                unique: true);

            return runtimeEntityType;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "Tiers");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
