﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Backbone.ConsumerApi.Tests.Integration.Helpers;

public static class ThrowHelpers
{
    public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
            throw new ArgumentNullException(paramName);
    }
}
