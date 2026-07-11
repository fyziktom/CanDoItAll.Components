using CanDoItAll.Components.FileBrowser.Core;

namespace CanDoItAll.Components.FileBrowser.Providers.FileSystem;

/// <summary>Controls whether hidden filesystem entries are exposed by a configured source.</summary>
public enum FileSystemHiddenItemPolicy
{
    /// <summary>Exclude entries marked hidden and dot-prefixed entries.</summary>
    Exclude,

    /// <summary>Include hidden entries.</summary>
    Include
}

/// <summary>Validated configuration for one root-confined local filesystem source.</summary>
public sealed record FileSystemFileBrowserOptions
{
    /// <summary>Creates validated options for one absolute, existing directory root.</summary>
    public FileSystemFileBrowserOptions(
        FileBrowserSourceId sourceId,
        string rootPath,
        string? displayName = null,
        bool includeHidden = false,
        bool followDirectoryReparsePoints = false,
        int recommendedPageSize = 50,
        int maximumPageSize = 250)
    {
        if (string.IsNullOrWhiteSpace(sourceId.Value))
        {
            throw new ArgumentException("A source identifier is required.", nameof(sourceId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(rootPath);
        if (!Path.IsPathFullyQualified(rootPath))
        {
            throw new ArgumentException("The filesystem browser root must be an absolute path.", nameof(rootPath));
        }

        if (recommendedPageSize is < 1 or > 1000)
        {
            throw new ArgumentOutOfRangeException(nameof(recommendedPageSize));
        }

        if (maximumPageSize is < 1 or > 1000 || maximumPageSize < recommendedPageSize)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumPageSize));
        }

        var normalizedRoot = NormalizeExistingRoot(rootPath, followDirectoryReparsePoints);
        var fallbackDisplayName = new DirectoryInfo(normalizedRoot).Name;

        SourceId = sourceId;
        RootPath = normalizedRoot;
        DisplayName = string.IsNullOrWhiteSpace(displayName)
            ? string.IsNullOrWhiteSpace(fallbackDisplayName) ? normalizedRoot : fallbackDisplayName
            : displayName.Trim();
        HiddenItemPolicy = includeHidden
            ? FileSystemHiddenItemPolicy.Include
            : FileSystemHiddenItemPolicy.Exclude;
        FollowDirectoryReparsePoints = followDirectoryReparsePoints;
        RecommendedPageSize = recommendedPageSize;
        MaximumPageSize = maximumPageSize;
    }

    /// <summary>Gets the configured source identifier.</summary>
    public FileBrowserSourceId SourceId { get; }

    /// <summary>Gets the normalized absolute root path.</summary>
    public string RootPath { get; }

    /// <summary>Gets the source display name.</summary>
    public string DisplayName { get; }

    /// <summary>Gets the hidden-entry policy.</summary>
    public FileSystemHiddenItemPolicy HiddenItemPolicy { get; }

    /// <summary>Gets whether hidden entries are included.</summary>
    public bool IncludeHidden => HiddenItemPolicy == FileSystemHiddenItemPolicy.Include;

    /// <summary>Gets whether directory reparse points may be followed when their targets remain inside the root.</summary>
    public bool FollowDirectoryReparsePoints { get; }

    /// <summary>Gets the recommended browse page size.</summary>
    public int RecommendedPageSize { get; }

    /// <summary>Gets the maximum browse page size.</summary>
    public int MaximumPageSize { get; }

    private static string NormalizeExistingRoot(string rootPath, bool followDirectoryReparsePoints)
    {
        var fullPath = Path.TrimEndingDirectorySeparator(Path.GetFullPath(rootPath));
        if (string.IsNullOrEmpty(fullPath))
        {
            fullPath = Path.GetPathRoot(Path.GetFullPath(rootPath))
                ?? throw new ArgumentException("The filesystem browser root is invalid.", nameof(rootPath));
        }

        var root = new DirectoryInfo(fullPath);
        root.Refresh();
        if (!root.Exists)
        {
            throw new DirectoryNotFoundException($"The filesystem browser root '{fullPath}' does not exist.");
        }

        if (!root.Attributes.HasFlag(FileAttributes.ReparsePoint))
        {
            return root.FullName;
        }

        if (!followDirectoryReparsePoints)
        {
            throw new ArgumentException(
                "The filesystem browser root cannot be a reparse point unless following reparse points is enabled.",
                nameof(rootPath));
        }

        var resolvedRoot = root.ResolveLinkTarget(returnFinalTarget: true) as DirectoryInfo;
        if (resolvedRoot is null)
        {
            throw new DirectoryNotFoundException($"The filesystem browser root reparse point '{fullPath}' has no directory target.");
        }

        resolvedRoot.Refresh();
        if (!resolvedRoot.Exists)
        {
            throw new DirectoryNotFoundException($"The filesystem browser root target '{resolvedRoot.FullName}' does not exist.");
        }

        return Path.TrimEndingDirectorySeparator(resolvedRoot.FullName);
    }
}
