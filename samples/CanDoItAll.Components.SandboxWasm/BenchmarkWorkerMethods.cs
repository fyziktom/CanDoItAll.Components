using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Text.Json;

namespace CanDoItAll.Components.SandboxWasm;

[SupportedOSPlatform("browser")]
public static partial class BenchmarkWorkerMethods
{
    private static readonly string[] Fruits = ["Apple", "Banana", "Cherry", "Mango", "Pear"];

    [JSExport]
    public static string AggregateFruitCounts(int itemCount)
    {
        var selections = CreateSelections(itemCount);
        var start = Stopwatch.GetTimestamp();
        var counts = Fruits.ToDictionary(fruit => fruit, _ => 0, StringComparer.Ordinal);

        foreach (var selection in selections)
        {
            counts[selection]++;
        }

        return JsonSerializer.Serialize(new Result(Stopwatch.GetElapsedTime(start).TotalMilliseconds, counts));
    }

    private static string[] CreateSelections(int count)
    {
        var selections = new string[count];
        uint state = 0x00C0FFEE;

        for (var index = 0; index < selections.Length; index++)
        {
            state = unchecked((state * 1_664_525u) + 1_013_904_223u);
            selections[index] = Fruits[(int)((state >> 16) % (uint)Fruits.Length)];
        }

        return selections;
    }

    private sealed record Result(double ElapsedMilliseconds, Dictionary<string, int> Counts);
}
