using System.Text;

namespace Backbone.BuildingBlocks.Infrastructure.Tests;

public static class StringExtensions
{
    public static byte[] GetBytes(this string str)
    {
        return Encoding.ASCII.GetBytes(str);
    }
}
