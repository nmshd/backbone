using Shouldly;

namespace Backbone.UnitTestTools.Shouldly.Extensions;

[ShouldlyMethods]
public static class IEnumerableExtensions
{
    public static void ShouldHaveCount<T>(this IEnumerable<T> enumerable, int count, string? customMessage = null)
    {
        var actual = enumerable.Count();
        actual.AssertAwesomely(v => v == count, actual, count, customMessage);
    }

    public static void ShouldContain<T>(this IEnumerable<T> enumerable, IEnumerable<T> expected, string? customMessage = null)
    {
        var contains = expected.Select(enumerable.Contains);
        if (contains.Any(v => v == false))
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, enumerable, customMessage).ToString());
    }

    public static void ShouldContainSingle<T>(this IEnumerable<T> enumerable, T expected, string? customMessage = null)
    {
        var count = enumerable.Count(v => Equals(v, expected));
        if (count != 1)
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, count, customMessage).ToString());
    }

    public static void ShouldContainSingle<T>(this IEnumerable<T> enumerable, Func<T, bool> expected, string? customMessage = null)
    {
        var count = enumerable.Count(expected);
        if (count != 1)
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, count, customMessage).ToString());
    }

    public static void ShouldContainInOrder<T>(this IEnumerable<T> enumerable, IEnumerable<T> expected, string? customMessage = null)
    {
        var list = expected.ToList();
        var foundIndices = list.Select(enumerable.FindAllIndices).ToList();

        foreach (var startIndex in foundIndices[0])
        {
            var lastIndex = startIndex;
            for (var offset = 1; offset < list.Count; offset++)
            {
                lastIndex = foundIndices[offset].FindIndex(v => v > lastIndex);
                if (lastIndex == -1) break;

                if (offset == list.Count - 1) return; // When one complete sublist is found (this index is reached here), return
            }
        }

        throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, enumerable, customMessage).ToString());
    }

    public static void ShouldNotBeNullOrEmpty<T>(this IEnumerable<T>? actual, string? customMessage = null)
    {
        if (actual is null || !actual.Any())
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, customMessage).ToString());
    }

    private static List<int> FindAllIndices<T>(this IEnumerable<T> enumerable, T element)
    {
        var list = enumerable.ToList();
        var ret = new List<int>();

        var startIndex = 0;
        while (startIndex < list.Count)
        {
            var foundIndex = list.IndexOf(element, startIndex);
            if (foundIndex == -1) break;

            ret.Add(foundIndex);
            startIndex = foundIndex + 1;
        }

        return ret;
    }
}
