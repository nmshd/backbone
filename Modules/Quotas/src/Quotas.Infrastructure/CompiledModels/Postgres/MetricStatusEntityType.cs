﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable enable

namespace Backbone.Modules.Quotas.Infrastructure.CompiledModels.Postgres
{
    internal partial class MetricStatusEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType? baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "Backbone.Modules.Quotas.Domain.Aggregates.Identities.MetricStatus",
                typeof(MetricStatus),
                baseEntityType);

            var owner = runtimeEntityType.AddProperty(
                "Owner",
                typeof(string),
                propertyInfo: typeof(MetricStatus).GetProperty("Owner", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MetricStatus).GetField("<Owner>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);

            var metricKey = runtimeEntityType.AddProperty(
                "MetricKey",
                typeof(MetricKey),
                propertyInfo: typeof(MetricStatus).GetProperty("MetricKey", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MetricStatus).GetField("<MetricKey>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                maxLength: 50,
                unicode: true,
                valueConverter: new MetricKeyEntityFrameworkValueConverter());
            metricKey.AddAnnotation("Relational:IsFixedLength", false);

            var isExhaustedUntil = runtimeEntityType.AddProperty(
                "IsExhaustedUntil",
                typeof(ExhaustionDate),
                propertyInfo: typeof(MetricStatus).GetProperty("IsExhaustedUntil", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MetricStatus).GetField("<IsExhaustedUntil>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueConverter: new ExhaustionDateValueConverter());

            var key = runtimeEntityType.AddKey(
                new[] { owner, metricKey });
            runtimeEntityType.SetPrimaryKey(key);

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("Owner")! },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Address")! })!,
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var metricStatuses = principalEntityType.AddNavigation("MetricStatuses",
                runtimeForeignKey,
                onDependent: false,
                typeof(IReadOnlyCollection<MetricStatus>),
                propertyInfo: typeof(Identity).GetProperty("MetricStatuses", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Identity).GetField("_metricStatuses", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "MetricStatuses");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
