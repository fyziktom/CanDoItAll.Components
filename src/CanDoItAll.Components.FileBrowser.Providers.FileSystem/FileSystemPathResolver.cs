using CanDoItAll.Components.FileBrowser.Core;

namespace CanDoItAll.Components.FileBrowser.Providers.FileSystem;

internal sealed record ResolvedFileSystemEntry(
    string LogicalPath,
    string RelativeKey,
    FileSystemInfo LogicalInfo,
    FileSystemInfo MetadataInfo,
    FileAttributes LogicalAttributes,
    bool IsReparsePoint,
    bool IsDirectory);

/// <summary>Owns occurrence-key normalization, root confinement, and reparse-point policy.</summary>
internal sealed class FileSystemPathResolver
{
    private readonly FileSystemFileBrowserOptions options;

    public FileSystemPathResolver(FileSystemFileBrowserOptions options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public FileBrowserItemKey RootKey => new(options.SourceId, ".");

    public ResolvedFileSystemEntry ResolveRoot()
    {
        var root = new DirectoryInfo(options.RootPath);
        root.Refresh();
        if (!root.Exists)
        {
            throw ProviderError(
                FileBrowserErrorCode.NotFound,
                "The configured filesystem source root is no longer available.");
        }

        return new ResolvedFileSystemEntry(
            root.FullName,
            ".",
            root,
            root,
            root.Attributes,
            IsReparsePoint: false,
            IsDirectory: true);
    }

    public IReadOnlyList<ResolvedFileSystemEntry> ResolvePath(FileBrowserItemKey itemKey)
    {
        ValidateKey(itemKey);
        var root = ResolveRoot();
        if (itemKey.Value == ".")
        {
            return [root];
        }

        var segments = ParseCanonicalSegments(itemKey.Value);
        var results = new List<ResolvedFileSystemEntry>(segments.Length + 1) { root };
        var currentPath = options.RootPath;

        for (var index = 0; index < segments.Length; index++)
        {
            currentPath = Path.Combine(currentPath, segments[index]);
            EnsureContained(currentPath);

            var entry = ResolveExistingEntry(currentPath, string.Join('/', segments.Take(index + 1)));
            if (!options.IncludeHidden && IsHidden(entry.LogicalInfo.Name, entry.LogicalAttributes))
            {
                throw ProviderError(
                    FileBrowserErrorCode.NotFound,
                    "The requested filesystem item is not available in this source.");
            }

            if (index < segments.Length - 1 && !entry.IsDirectory)
            {
                throw ProviderError(
                    entry.IsReparsePoint
                        ? FileBrowserErrorCode.Unsupported
                        : FileBrowserErrorCode.InvalidLocation,
                    entry.IsReparsePoint
                        ? "Browsing through directory reparse points is disabled for this source."
                        : "The requested filesystem path traverses a non-directory item.");
            }

            results.Add(entry);
        }

        return results;
    }

    public ResolvedFileSystemEntry ResolveChild(FileSystemInfo logicalInfo)
    {
        ArgumentNullException.ThrowIfNull(logicalInfo);
        var logicalPath = Path.GetFullPath(logicalInfo.FullName);
        EnsureContained(logicalPath);
        return ResolveExistingEntry(logicalPath, ToRelativeKey(logicalPath));
    }

    public bool IsVisible(ResolvedFileSystemEntry entry)
        => options.IncludeHidden || !IsHidden(entry.LogicalInfo.Name, entry.LogicalAttributes);

    public FileBrowserItemKey CreateKey(string relativeKey)
        => new(options.SourceId, relativeKey);

    public FileBrowserItemKey? CreateParentKey(string relativeKey)
    {
        if (relativeKey == ".")
        {
            return null;
        }

        var separatorIndex = relativeKey.LastIndexOf('/');
        return CreateKey(separatorIndex < 0 ? "." : relativeKey[..separatorIndex]);
    }

    public string ToRelativeKey(string fullPath)
    {
        var normalizedFullPath = Path.GetFullPath(fullPath);
        EnsureContained(normalizedFullPath);
        var relativePath = Path.GetRelativePath(options.RootPath, normalizedFullPath);
        if (relativePath == ".")
        {
            return ".";
        }

        return relativePath.Replace(Path.DirectorySeparatorChar, '/');
    }

    public void ValidateSource(FileBrowserItemKey itemKey)
        => ValidateKey(itemKey);

    private ResolvedFileSystemEntry ResolveExistingEntry(string logicalPath, string relativeKey)
    {
        FileAttributes logicalAttributes;
        try
        {
            logicalAttributes = File.GetAttributes(logicalPath);
        }
        catch (FileNotFoundException exception)
        {
            throw ProviderError(FileBrowserErrorCode.NotFound, "The requested filesystem item no longer exists.", exception);
        }
        catch (DirectoryNotFoundException exception)
        {
            throw ProviderError(FileBrowserErrorCode.NotFound, "The requested filesystem item no longer exists.", exception);
        }

        var logicalIsDirectory = logicalAttributes.HasFlag(FileAttributes.Directory);
        FileSystemInfo logicalInfo = logicalIsDirectory
            ? new DirectoryInfo(logicalPath)
            : new FileInfo(logicalPath);
        logicalInfo.Refresh();

        var isReparsePoint = logicalAttributes.HasFlag(FileAttributes.ReparsePoint);
        if (!isReparsePoint)
        {
            return new ResolvedFileSystemEntry(
                logicalPath,
                relativeKey,
                logicalInfo,
                logicalInfo,
                logicalAttributes,
                IsReparsePoint: false,
                IsDirectory: logicalIsDirectory);
        }

        if (!options.FollowDirectoryReparsePoints)
        {
            return new ResolvedFileSystemEntry(
                logicalPath,
                relativeKey,
                logicalInfo,
                logicalInfo,
                logicalAttributes,
                IsReparsePoint: true,
                IsDirectory: false);
        }

        var target = logicalInfo.ResolveLinkTarget(returnFinalTarget: true);
        if (target is null)
        {
            throw ProviderError(
                FileBrowserErrorCode.NotFound,
                "The requested filesystem reparse point has no available target.");
        }

        var targetPath = Path.GetFullPath(target.FullName);
        EnsureContained(targetPath, isReparseTarget: true);
        var targetAttributes = File.GetAttributes(targetPath);
        var targetIsDirectory = targetAttributes.HasFlag(FileAttributes.Directory);
        FileSystemInfo metadataInfo = targetIsDirectory
            ? new DirectoryInfo(targetPath)
            : new FileInfo(targetPath);
        metadataInfo.Refresh();

        return new ResolvedFileSystemEntry(
            logicalPath,
            relativeKey,
            logicalInfo,
            metadataInfo,
            logicalAttributes,
            IsReparsePoint: true,
            IsDirectory: targetIsDirectory);
    }

    private void ValidateKey(FileBrowserItemKey itemKey)
    {
        if (itemKey.SourceId != options.SourceId)
        {
            throw ProviderError(
                FileBrowserErrorCode.InvalidLocation,
                "The requested filesystem item belongs to a different source.");
        }

        if (itemKey.Revision is not null)
        {
            throw ProviderError(
                FileBrowserErrorCode.InvalidLocation,
                "Local filesystem occurrence keys do not support revisions.");
        }

        if (itemKey.Value == ".")
        {
            return;
        }

        var segments = ParseCanonicalSegments(itemKey.Value);
        var candidate = Path.GetFullPath(Path.Combine([options.RootPath, .. segments]));
        EnsureContained(candidate);
        var canonicalKey = ToRelativeKey(candidate);
        if (!string.Equals(canonicalKey, itemKey.Value, StringComparison.Ordinal))
        {
            throw ProviderError(
                FileBrowserErrorCode.InvalidLocation,
                "The filesystem occurrence key is not in canonical root-relative form.");
        }
    }

    private static string[] ParseCanonicalSegments(string value)
    {
        if (string.IsNullOrWhiteSpace(value)
            || Path.IsPathRooted(value)
            || Path.IsPathFullyQualified(value)
            || value.StartsWith("/", StringComparison.Ordinal)
            || value.EndsWith("/", StringComparison.Ordinal))
        {
            throw ProviderError(
                FileBrowserErrorCode.InvalidLocation,
                "The filesystem occurrence key must be a normalized root-relative path.");
        }

        var segments = value.Split('/');
        if (segments.Length == 0
            || segments.Any(segment => string.IsNullOrWhiteSpace(segment) || segment is "." or ".."))
        {
            throw ProviderError(
                FileBrowserErrorCode.InvalidLocation,
                "The filesystem occurrence key contains an invalid or traversal segment.");
        }

        var invalidFileNameCharacters = Path.GetInvalidFileNameChars();
        foreach (var segment in segments)
        {
            if (segment.IndexOfAny(invalidFileNameCharacters) >= 0
                || Path.IsPathRooted(segment)
                || OperatingSystem.IsWindows() && segment.Contains('\\'))
            {
                throw ProviderError(
                    FileBrowserErrorCode.InvalidLocation,
                    "The filesystem occurrence key contains an invalid path segment.");
            }
        }

        return segments;
    }

    private void EnsureContained(string candidatePath, bool isReparseTarget = false)
    {
        var relativePath = Path.GetRelativePath(options.RootPath, Path.GetFullPath(candidatePath));
        var escapesRoot = Path.IsPathRooted(relativePath)
            || relativePath == ".."
            || relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
            || relativePath.StartsWith($"..{Path.AltDirectorySeparatorChar}", StringComparison.Ordinal);
        if (!escapesRoot)
        {
            return;
        }

        throw ProviderError(
            isReparseTarget ? FileBrowserErrorCode.Forbidden : FileBrowserErrorCode.InvalidLocation,
            isReparseTarget
                ? "The filesystem reparse point resolves outside the configured source root."
                : "The requested filesystem path escapes the configured source root.");
    }

    private static bool IsHidden(string name, FileAttributes attributes)
        => attributes.HasFlag(FileAttributes.Hidden)
           || name.StartsWith(".", StringComparison.Ordinal);

    private static FileBrowserProviderException ProviderError(
        FileBrowserErrorCode code,
        string message,
        Exception? innerException = null)
        => new(new FileBrowserError(code, message, technicalDetail: innerException?.Message), innerException);
}
