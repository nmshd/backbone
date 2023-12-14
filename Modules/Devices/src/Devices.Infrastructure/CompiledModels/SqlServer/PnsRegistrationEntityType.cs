﻿// <auto-generated />
using System;
using System.Reflection;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.ValueConverters;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
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
    internal partial class PnsRegistrationEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.PnsRegistration",
                typeof(PnsRegistration),
                baseEntityType);

            var deviceId = runtimeEntityType.AddProperty(
                "DeviceId",
                typeof(DeviceId),
                propertyInfo: typeof(PnsRegistration).GetProperty("DeviceId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PnsRegistration).GetField("<DeviceId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                maxLength: 20,
                unicode: false,
                valueConverter: new DeviceIdValueConverter());
            deviceId.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<DeviceId>(
                    (DeviceId v1, DeviceId v2) => object.Equals(v1, v2),
                    (DeviceId v) => v.GetHashCode(),
                    (DeviceId v) => v),
                keyComparer: new ValueComparer<DeviceId>(
                    (DeviceId v1, DeviceId v2) => object.Equals(v1, v2),
                    (DeviceId v) => v.GetHashCode(),
                    (DeviceId v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "char(20)",
                    size: 20,
                    dbType: System.Data.DbType.AnsiStringFixedLength),
                converter: new ValueConverter<DeviceId, string>(
                    (DeviceId id) => id.StringValue,
                    (string value) => DeviceId.Parse(value)),
                jsonValueReaderWriter: new JsonConvertedValueReaderWriter<DeviceId, string>(
                    JsonStringReaderWriter.Instance,
                    new ValueConverter<DeviceId, string>(
                        (DeviceId id) => id.StringValue,
                        (string value) => DeviceId.Parse(value))));
            deviceId.AddAnnotation("Relational:IsFixedLength", true);
            deviceId.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var appId = runtimeEntityType.AddProperty(
                "AppId",
                typeof(string),
                propertyInfo: typeof(PnsRegistration).GetProperty("AppId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PnsRegistration).GetField("<AppId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            appId.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                keyComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "nvarchar(max)",
                    dbType: System.Data.DbType.String),
                storeTypePostfix: StoreTypePostfix.None);
            appId.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var devicePushIdentifier = runtimeEntityType.AddProperty(
                "DevicePushIdentifier",
                typeof(DevicePushIdentifier),
                propertyInfo: typeof(PnsRegistration).GetProperty("DevicePushIdentifier", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PnsRegistration).GetField("<DevicePushIdentifier>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false,
                valueConverter: new DevicePushIdentifierEntityFrameworkValueConverter());
            devicePushIdentifier.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<DevicePushIdentifier>(
                    (DevicePushIdentifier v1, DevicePushIdentifier v2) => object.Equals(v1, v2),
                    (DevicePushIdentifier v) => v.GetHashCode(),
                    (DevicePushIdentifier v) => v),
                keyComparer: new ValueComparer<DevicePushIdentifier>(
                    (DevicePushIdentifier v1, DevicePushIdentifier v2) => object.Equals(v1, v2),
                    (DevicePushIdentifier v) => v.GetHashCode(),
                    (DevicePushIdentifier v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "char(20)",
                    size: 20,
                    dbType: System.Data.DbType.AnsiStringFixedLength),
                converter: new ValueConverter<DevicePushIdentifier, string>(
                    (DevicePushIdentifier dpi) => dpi.StringValue,
                    (string value) => DevicePushIdentifier.Parse(value)),
                jsonValueReaderWriter: new JsonConvertedValueReaderWriter<DevicePushIdentifier, string>(
                    JsonStringReaderWriter.Instance,
                    new ValueConverter<DevicePushIdentifier, string>(
                        (DevicePushIdentifier dpi) => dpi.StringValue,
                        (string value) => DevicePushIdentifier.Parse(value))));
            devicePushIdentifier.AddAnnotation("Relational:IsFixedLength", true);
            devicePushIdentifier.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var environment = runtimeEntityType.AddProperty(
                "Environment",
                typeof(PushEnvironment),
                propertyInfo: typeof(PnsRegistration).GetProperty("Environment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PnsRegistration).GetField("<Environment>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd);
            environment.TypeMapping = IntTypeMapping.Default.Clone(
                comparer: new ValueComparer<PushEnvironment>(
                    (PushEnvironment v1, PushEnvironment v2) => object.Equals((object)v1, (object)v2),
                    (PushEnvironment v) => v.GetHashCode(),
                    (PushEnvironment v) => v),
                keyComparer: new ValueComparer<PushEnvironment>(
                    (PushEnvironment v1, PushEnvironment v2) => object.Equals((object)v1, (object)v2),
                    (PushEnvironment v) => v.GetHashCode(),
                    (PushEnvironment v) => v),
                providerValueComparer: new ValueComparer<int>(
                    (int v1, int v2) => v1 == v2,
                    (int v) => v,
                    (int v) => v),
                converter: new ValueConverter<PushEnvironment, int>(
                    (PushEnvironment value) => (int)value,
                    (int value) => (PushEnvironment)value),
                jsonValueReaderWriter: new JsonConvertedValueReaderWriter<PushEnvironment, int>(
                    JsonInt32ReaderWriter.Instance,
                    new ValueConverter<PushEnvironment, int>(
                        (PushEnvironment value) => (int)value,
                        (int value) => (PushEnvironment)value)));
            environment.SetSentinelFromProviderValue(0);
            environment.AddAnnotation("Relational:DefaultValue", PushEnvironment.Production);
            environment.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var handle = runtimeEntityType.AddProperty(
                "Handle",
                typeof(PnsHandle),
                propertyInfo: typeof(PnsRegistration).GetProperty("Handle", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PnsRegistration).GetField("<Handle>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 200,
                unicode: true,
                valueConverter: new PnsHandleEntityFrameworkValueConverter());
            handle.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<PnsHandle>(
                    (PnsHandle v1, PnsHandle v2) => v1 == null && v2 == null || v1 != null && v2 != null && v1.Equals(v2),
                    (PnsHandle v) => v.GetHashCode(),
                    (PnsHandle v) => v),
                keyComparer: new ValueComparer<PnsHandle>(
                    (PnsHandle v1, PnsHandle v2) => v1 == null && v2 == null || v1 != null && v2 != null && v1.Equals(v2),
                    (PnsHandle v) => v.GetHashCode(),
                    (PnsHandle v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "nvarchar(200)",
                    size: 200,
                    dbType: System.Data.DbType.String),
                converter: new ValueConverter<PnsHandle, string>(
                    (PnsHandle pnsHandle) => PnsHandleEntityFrameworkValueConverter.SerializeHandle(pnsHandle),
                    (string value) => PnsHandleEntityFrameworkValueConverter.DeserializeHandle(value)),
                jsonValueReaderWriter: new JsonConvertedValueReaderWriter<PnsHandle, string>(
                    JsonStringReaderWriter.Instance,
                    new ValueConverter<PnsHandle, string>(
                        (PnsHandle pnsHandle) => PnsHandleEntityFrameworkValueConverter.SerializeHandle(pnsHandle),
                        (string value) => PnsHandleEntityFrameworkValueConverter.DeserializeHandle(value))));
            handle.AddAnnotation("Relational:IsFixedLength", false);
            handle.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var identityAddress = runtimeEntityType.AddProperty(
                "IdentityAddress",
                typeof(IdentityAddress),
                propertyInfo: typeof(PnsRegistration).GetProperty("IdentityAddress", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PnsRegistration).GetField("<IdentityAddress>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 36,
                unicode: false,
                valueConverter: new IdentityAddressValueConverter());
            identityAddress.TypeMapping = SqlServerStringTypeMapping.Default.Clone(
                comparer: new ValueComparer<IdentityAddress>(
                    (IdentityAddress v1, IdentityAddress v2) => v1 == null && v2 == null || v1 != null && v2 != null && v1.Equals(v2),
                    (IdentityAddress v) => v.GetHashCode(),
                    (IdentityAddress v) => v),
                keyComparer: new ValueComparer<IdentityAddress>(
                    (IdentityAddress v1, IdentityAddress v2) => v1 == null && v2 == null || v1 != null && v2 != null && v1.Equals(v2),
                    (IdentityAddress v) => v.GetHashCode(),
                    (IdentityAddress v) => v),
                providerValueComparer: new ValueComparer<string>(
                    (string v1, string v2) => v1 == v2,
                    (string v) => v.GetHashCode(),
                    (string v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "char(36)",
                    size: 36,
                    dbType: System.Data.DbType.AnsiStringFixedLength),
                converter: new ValueConverter<IdentityAddress, string>(
                    (IdentityAddress id) => id.StringValue,
                    (string value) => IdentityAddress.ParseUnsafe(value.Trim())),
                jsonValueReaderWriter: new JsonConvertedValueReaderWriter<IdentityAddress, string>(
                    JsonStringReaderWriter.Instance,
                    new ValueConverter<IdentityAddress, string>(
                        (IdentityAddress id) => id.StringValue,
                        (string value) => IdentityAddress.ParseUnsafe(value.Trim()))));
            identityAddress.AddAnnotation("Relational:IsFixedLength", true);
            identityAddress.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updatedAt = runtimeEntityType.AddProperty(
                "UpdatedAt",
                typeof(DateTime),
                propertyInfo: typeof(PnsRegistration).GetProperty("UpdatedAt", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PnsRegistration).GetField("<UpdatedAt>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueConverter: new DateTimeValueConverter());
            updatedAt.TypeMapping = SqlServerDateTimeTypeMapping.Default.Clone(
                comparer: new ValueComparer<DateTime>(
                    (DateTime v1, DateTime v2) => v1.Equals(v2),
                    (DateTime v) => v.GetHashCode(),
                    (DateTime v) => v),
                keyComparer: new ValueComparer<DateTime>(
                    (DateTime v1, DateTime v2) => v1.Equals(v2),
                    (DateTime v) => v.GetHashCode(),
                    (DateTime v) => v),
                providerValueComparer: new ValueComparer<DateTime>(
                    (DateTime v1, DateTime v2) => v1.Equals(v2),
                    (DateTime v) => v.GetHashCode(),
                    (DateTime v) => v),
                converter: new ValueConverter<DateTime, DateTime>(
                    (DateTime v) => v.ToUniversalTime(),
                    (DateTime v) => DateTime.SpecifyKind(v, DateTimeKind.Utc)),
                jsonValueReaderWriter: new JsonConvertedValueReaderWriter<DateTime, DateTime>(
                    JsonDateTimeReaderWriter.Instance,
                    new ValueConverter<DateTime, DateTime>(
                        (DateTime v) => v.ToUniversalTime(),
                        (DateTime v) => DateTime.SpecifyKind(v, DateTimeKind.Utc))));
            updatedAt.SetSentinelFromProviderValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            updatedAt.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var key = runtimeEntityType.AddKey(
                new[] { deviceId });
            runtimeEntityType.SetPrimaryKey(key);

            return runtimeEntityType;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "PnsRegistrations");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
