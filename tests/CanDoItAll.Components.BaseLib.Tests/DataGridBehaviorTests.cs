using System.Collections;
using System.Reflection;
using Microsoft.AspNetCore.Components.Web;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class DataGridBehaviorTests
{
    [Fact]
    public void DataIsEnumeratedOncePerParameterUpdateAndPagingUsesCachedItems()
    {
        var source = new CountingEnumerable<int>([1, 2, 3, 4, 5]);
        var grid = new DataGrid<int>();
        SetParameter(grid, nameof(DataGrid<int>.Data), source);
        SetParameter(grid, nameof(DataGrid<int>.AllowPaging), true);
        SetParameter(grid, nameof(DataGrid<int>.PageSize), 2);

        Invoke(grid, "OnParametersSet");
        _ = ReadProperty<int>(grid, "TotalPages");
        _ = ReadProperty<IReadOnlyList<int>>(grid, "VisibleItems");
        _ = ReadProperty<IReadOnlyList<int>>(grid, "VisibleItems");

        Assert.Equal(1, source.EnumerationCount);
        Assert.Equal([1, 2], ReadProperty<IReadOnlyList<int>>(grid, "VisibleItems"));

        Invoke(grid, "NextPageAsync");

        Assert.Equal([3, 4], ReadProperty<IReadOnlyList<int>>(grid, "VisibleItems"));
        Assert.Equal(1, source.EnumerationCount);
    }

    [Theory]
    [InlineData("Enter")]
    [InlineData(" ")]
    public async Task EnterAndSpaceActivateRow(string key)
    {
        var selected = 0;
        var grid = new DataGrid<int>();
        SetParameter(
            grid,
            nameof(DataGrid<int>.RowSelect),
            Microsoft.AspNetCore.Components.EventCallback.Factory.Create<int>(this, value => selected = value));

        await (Task)grid.GetType()
            .GetMethod("OnRowKeyDownAsync", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(grid, [new KeyboardEventArgs { Key = key }, 42])!;

        Assert.Equal(42, selected);
    }

    private static void Invoke(object target, string methodName)
        => target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(target, null);

    private static T ReadProperty<T>(object target, string propertyName)
        => (T)target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(target)!;

    private static void SetParameter(object target, string propertyName, object? value)
        => target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)!.SetValue(target, value);

    private sealed class CountingEnumerable<T>(IReadOnlyList<T> items) : IEnumerable<T>
    {
        public int EnumerationCount { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            EnumerationCount++;
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
