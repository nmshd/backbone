using System.Diagnostics;
using System.Reflection;
using System.Resources;
using Backbone.BuildingBlocks.Domain.PushNotifications;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Xunit;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests;
public class Translations
{
    [Fact]
    public void TranslationsTest()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddLocalization(o =>
        {
            o.ResourcesPath = """..\..\..\src\Devices.Infrastructure\Translations""";
        });

        var app = builder.Build();

        var rmg = Infrastructure.Translations.resources.resourceMan;
        //var str1 = rmg.GetString("DeletionProcessApprovedNotification.Title");   // resxTxt is any string in your resx file.


        // var localizer = app.Services.CreateScope().ServiceProvider.GetService<IStringLocalizer<IPushNotification>>();
        // Console.WriteLine(localizer?.GetAllStrings().Count());

        var assemblyName = Assembly.GetExecutingAssembly().GetReferencedAssemblies().First(a => a.Name!.Contains(".Infrastructure"));
        var assembly = Assembly.LoadFrom(assemblyName.Name!+".dll");
        var rm = new ResourceManager("PushNotifications.DirectPush.IPushNotification", assembly);


        //  Console.WriteLine(rm.ToString());
    }
}

internal class E : IPushNotification { }
