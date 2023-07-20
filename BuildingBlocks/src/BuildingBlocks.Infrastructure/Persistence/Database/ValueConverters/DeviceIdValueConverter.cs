﻿using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database.ValueConverters;

public class DeviceIdValueConverter : ValueConverter<DeviceId, string>
{
    public DeviceIdValueConverter() : this(null)
    {
    }

    public DeviceIdValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id.StringValue,
            value => DeviceId.Parse(value),
            mappingHints
        )
    {
    }
}

public class NullableDeviceIdValueConverter : ValueConverter<DeviceId?, string?>
{
    public NullableDeviceIdValueConverter() : this(null)
    {
    }

    public NullableDeviceIdValueConverter(ConverterMappingHints? mappingHints)
        : base(
            id => id == null ? null : id.StringValue,
            value => value == null ? null : DeviceId.Parse(value),
            mappingHints
        )
    {
    }
}
