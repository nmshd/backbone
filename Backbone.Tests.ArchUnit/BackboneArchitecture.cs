﻿using System.Reflection;
using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using Assembly = System.Reflection.Assembly;

namespace Backbone.Tests.ArchUnit;
public static class Backbone
{
    public static readonly Architecture Architecture =
        new ArchLoader()
            .LoadAssemblies(GetSolutionAssemblies())
            .Build();

    private static Assembly[] GetSolutionAssemblies()
    {
        var assemblies = Directory
            .GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
            .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x)))
            .Where(x =>
                x.FullName.StartsWith("Backbone") ||
                x.FullName.StartsWith("BuildingBlocks"));
        return assemblies.ToArray();
    }
}
