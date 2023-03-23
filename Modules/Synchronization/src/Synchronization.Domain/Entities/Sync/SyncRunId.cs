﻿using System.ComponentModel;
using System.Globalization;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.BuildingBlocks.Domain.StronglyTypedIds.Classes;

namespace Backbone.Modules.Synchronization.Domain.Entities.Sync;

[Serializable]
[TypeConverter(typeof(SyncRunIdTypeConverter))]
public class SyncRunId : StronglyTypedId
{
    public const int MAX_LENGTH = DEFAULT_MAX_LENGTH;
    private const string PREFIX = "SYR";
    private static readonly StronglyTypedIdHelpers Utils = new(PREFIX, DefaultValidChars, MAX_LENGTH);

    private SyncRunId(string stringValue) : base(stringValue) { }

    public static SyncRunId Parse(string stringValue)
    {
        if (!IsValid(stringValue))
            throw new InvalidIdException($"'{stringValue}' is not a valid {nameof(SyncRunId)}.");

        return new SyncRunId(stringValue);
    }

    public static bool IsValid(string stringValue)
    {
        return Utils.IsValid(stringValue);
    }

    public static SyncRunId New()
    {
        var challengeIdAsString = StringUtils.Generate(DefaultValidChars, DEFAULT_MAX_LENGTH_WITHOUT_PREFIX);
        return new SyncRunId(PREFIX + challengeIdAsString);
    }

    public class SyncRunIdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            var stringValue = value as string;

            return !string.IsNullOrEmpty(stringValue) ? Parse(stringValue) : base.ConvertFrom(context, culture, value);
        }
    }
}
