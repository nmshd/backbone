﻿// <auto-generated />
using System;
using System.Reflection;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database.ValueConverters;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using OpenIddict.EntityFrameworkCore.Models;

#pragma warning disable 219, 612, 618
#nullable enable

namespace Backbone.Modules.Devices.Infrastructure.CompiledModels.Postgres
{
    internal partial class CustomOpenIddictEntityFrameworkCoreApplicationEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType? baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "Backbone.Modules.Devices.Infrastructure.OpenIddict.CustomOpenIddictEntityFrameworkCoreApplication",
                typeof(CustomOpenIddictEntityFrameworkCoreApplication),
                baseEntityType);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(string),
                propertyInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                afterSaveBehavior: PropertySaveBehavior.Throw);

            var clientId = runtimeEntityType.AddProperty(
                "ClientId",
                typeof(string),
                propertyInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetProperty("ClientId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetField("<ClientId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 100);

            var clientSecret = runtimeEntityType.AddProperty(
                "ClientSecret",
                typeof(string),
                propertyInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetProperty("ClientSecret", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetField("<ClientSecret>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var concurrencyToken = runtimeEntityType.AddProperty(
                "ConcurrencyToken",
                typeof(string),
                propertyInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetProperty("ConcurrencyToken", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetField("<ConcurrencyToken>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                concurrencyToken: true,
                maxLength: 50);

            var consentType = runtimeEntityType.AddProperty(
                "ConsentType",
                typeof(string),
                propertyInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetProperty("ConsentType", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetField("<ConsentType>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 50);

            var createdAt = runtimeEntityType.AddProperty(
                "CreatedAt",
                typeof(DateTime),
                propertyInfo: typeof(CustomOpenIddictEntityFrameworkCoreApplication).GetProperty("CreatedAt", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CustomOpenIddictEntityFrameworkCoreApplication).GetField("<CreatedAt>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueConverter: new DateTimeValueConverter());

            var defaultTier = runtimeEntityType.AddProperty(
                "DefaultTier",
                typeof(TierId),
                propertyInfo: typeof(CustomOpenIddictEntityFrameworkCoreApplication).GetProperty("DefaultTier", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CustomOpenIddictEntityFrameworkCoreApplication).GetField("<DefaultTier>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false,
                valueConverter: new TierIdEntityFrameworkValueConverter());
            defaultTier.AddAnnotation("Relational:IsFixedLength", true);

            var displayName = runtimeEntityType.AddProperty(
                "DisplayName",
                typeof(string),
                propertyInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetProperty("DisplayName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetField("<DisplayName>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var displayNames = runtimeEntityType.AddProperty(
                "DisplayNames",
                typeof(string),
                propertyInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetProperty("DisplayNames", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetField("<DisplayNames>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var permissions = runtimeEntityType.AddProperty(
                "Permissions",
                typeof(string),
                propertyInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetProperty("Permissions", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetField("<Permissions>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var postLogoutRedirectUris = runtimeEntityType.AddProperty(
                "PostLogoutRedirectUris",
                typeof(string),
                propertyInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetProperty("PostLogoutRedirectUris", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetField("<PostLogoutRedirectUris>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var properties = runtimeEntityType.AddProperty(
                "Properties",
                typeof(string),
                propertyInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetProperty("Properties", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetField("<Properties>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var redirectUris = runtimeEntityType.AddProperty(
                "RedirectUris",
                typeof(string),
                propertyInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetProperty("RedirectUris", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetField("<RedirectUris>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var requirements = runtimeEntityType.AddProperty(
                "Requirements",
                typeof(string),
                propertyInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetProperty("Requirements", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetField("<Requirements>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var type = runtimeEntityType.AddProperty(
                "Type",
                typeof(string),
                propertyInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetProperty("Type", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>).GetField("<Type>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 50);

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { clientId },
                unique: true);

            var index0 = runtimeEntityType.AddIndex(
                new[] { defaultTier });

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("DefaultTier")! },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id")! })!,
                principalEntityType,
                deleteBehavior: DeleteBehavior.Restrict,
                required: true);

            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "OpenIddictApplications");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
