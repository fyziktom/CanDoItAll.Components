using System.Globalization;
using CanDoItAll.Components.FileBrowser.Core;

namespace CanDoItAll.Components.FileBrowser.BaseLib.Tests;

public sealed class FileBrowserDisplayFormatterTests
{
    [Theory]
    [InlineData(null, "—")]
    [InlineData(0L, "0 B")]
    [InlineData(1L, "1 B")]
    [InlineData(1_000L, "1,000 B")]
    [InlineData(1_023L, "1,023 B")]
    [InlineData(1_024L, "1 KB")]
    [InlineData(1_536L, "1.5 KB")]
    [InlineData(1_048_576L, "1 MB")]
    [InlineData(1_073_741_824L, "1 GB")]
    [InlineData(1_099_511_627_776L, "1 TB")]
    [InlineData(1_125_899_906_842_624L, "1 PB")]
    [InlineData(long.MaxValue, "8192 PB")]
    public void FormatSize_UsesCompactBinaryUnits(long? bytes, string expected)
    {
        using var _ = new CultureScope(CultureInfo.InvariantCulture);

        Assert.Equal(expected, FileBrowserDisplayFormatter.FormatSize(bytes));
    }

    [Fact]
    public void FormatDate_NullValueUsesPlaceholder()
    {
        Assert.Equal("—", FileBrowserDisplayFormatter.FormatDate(null));
    }

    [Fact]
    public void FormatDate_UsesLocalTimeAndStableMinutePrecision()
    {
        using var _ = new CultureScope(CultureInfo.InvariantCulture);
        var value = new DateTimeOffset(2026, 7, 10, 12, 34, 56, TimeSpan.FromHours(3));

        var result = FileBrowserDisplayFormatter.FormatDate(value);

        Assert.Equal(value.ToLocalTime().ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), result);
        Assert.DoesNotContain("56", result, StringComparison.Ordinal);
    }

    [Fact]
    public void FormatType_ContainerAlwaysUsesFolderLabel()
    {
        var folder = TestFileBrowserItemFactory.Folder();

        Assert.Equal("Folder", FileBrowserDisplayFormatter.FormatType(folder));
    }

    [Fact]
    public void FormatType_FilePrefersProviderMediaType()
    {
        var item = TestFileBrowserItemFactory.File(
            "diagram.svg",
            FileBrowserItemCategory.Image,
            mediaType: "image/svg+xml");

        Assert.Equal("image/svg+xml", FileBrowserDisplayFormatter.FormatType(item));
    }

    [Fact]
    public void FormatType_FileFallsBackToProviderNeutralCategory()
    {
        var item = TestFileBrowserItemFactory.File("archive.car", FileBrowserItemCategory.Data);

        Assert.Equal("Data", FileBrowserDisplayFormatter.FormatType(item));
    }

    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo originalCulture = CultureInfo.CurrentCulture;
        private readonly CultureInfo originalUiCulture = CultureInfo.CurrentUICulture;

        public CultureScope(CultureInfo culture)
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        public void Dispose()
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }
}
