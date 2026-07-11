using CanDoItAll.Components.FileBrowser.Core;

namespace CanDoItAll.Components.FileBrowser.BaseLib;

/// <summary>Available renderings for the same browser session snapshot.</summary>
public enum FileBrowserViewMode
{
    List,
    Cards
}

/// <summary>Reason an item was invoked rather than only selected.</summary>
public enum FileBrowserInvocationKind
{
    PointerDoubleClick,
    Keyboard,
    PrimaryAction
}

/// <summary>Host notification for a file or non-default item invocation.</summary>
public sealed record FileBrowserItemInvokedEventArgs(
    FileBrowserItem Item,
    FileBrowserInvocationKind Kind);

/// <summary>Host notification for a capability-driven item action.</summary>
public sealed record FileBrowserItemActionEventArgs(
    FileBrowserItem Item,
    string ActionId);

/// <summary>Coordinates an item overflow/context menu.</summary>
public sealed record FileBrowserActionMenuRequest(
    FileBrowserItem Item,
    double X,
    double Y);

/// <summary>A labeled value used by BaseLib dropdown controls.</summary>
public sealed record FileBrowserChoice<T>(string Text, T Value);

/// <summary>Formatting helpers shared by list and card projections.</summary>
public static class FileBrowserDisplayFormatter
{
    private static readonly string[] Units = ["B", "KB", "MB", "GB", "TB", "PB"];

    public static string FormatSize(long? bytes)
    {
        if (!bytes.HasValue)
        {
            return "—";
        }

        var value = (double)bytes.Value;
        var unit = 0;
        while (value >= 1024 && unit < Units.Length - 1)
        {
            value /= 1024;
            unit++;
        }

        return unit == 0
            ? $"{bytes.Value:N0} {Units[unit]}"
            : $"{value:0.#} {Units[unit]}";
    }

    public static string FormatDate(DateTimeOffset? value)
        => value?.ToLocalTime().ToString("yyyy-MM-dd HH:mm") ?? "—";

    public static string FormatType(FileBrowserItem item)
        => item.IsContainer
            ? "Folder"
            : item.MediaType ?? item.Category.ToString();
}

/// <summary>Maps provider-neutral categories and common extensions to Material icon tokens.</summary>
public static class FileBrowserIconResolver
{
    public static string Resolve(FileBrowserItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (item.IsContainer)
        {
            return item.Metadata.TryGetValue("icon", out var icon) && !string.IsNullOrWhiteSpace(icon)
                ? icon
                : "folder";
        }

        var extension = Path.GetExtension(item.Name).ToLowerInvariant();
        return item.Category switch
        {
            FileBrowserItemCategory.Image => "image",
            FileBrowserItemCategory.Video => "video_file",
            FileBrowserItemCategory.Audio => "audio_file",
            FileBrowserItemCategory.Archive => "folder_zip",
            FileBrowserItemCategory.Code => "code",
            FileBrowserItemCategory.Data => "dataset",
            FileBrowserItemCategory.Link => "link",
            FileBrowserItemCategory.Document when extension == ".pdf" => "picture_as_pdf",
            FileBrowserItemCategory.Document => "description",
            _ => "draft"
        };
    }
}
