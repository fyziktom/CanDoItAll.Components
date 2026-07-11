using System.Collections.ObjectModel;
using CanDoItAll.Components.FileBrowser.Core;

namespace CanDoItAll.Components.FileBrowser.Providers.FileSystem;

internal sealed record FileSystemEntrySnapshot(
    ResolvedFileSystemEntry Entry,
    long? Size,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? ModifiedAt,
    FileBrowserItemCategory Category,
    string? MediaType,
    string? LinkTarget);

/// <summary>Maps BCL filesystem metadata into provider-owned core records.</summary>
internal sealed class FileSystemItemFactory
{
    private static readonly IReadOnlyDictionary<string, (FileBrowserItemCategory Category, string MediaType)> KnownTypes
        = new ReadOnlyDictionary<string, (FileBrowserItemCategory, string)>(
            new Dictionary<string, (FileBrowserItemCategory, string)>(StringComparer.OrdinalIgnoreCase)
            {
                [".7z"] = (FileBrowserItemCategory.Archive, "application/x-7z-compressed"),
                [".aac"] = (FileBrowserItemCategory.Audio, "audio/aac"),
                [".avi"] = (FileBrowserItemCategory.Video, "video/x-msvideo"),
                [".bmp"] = (FileBrowserItemCategory.Image, "image/bmp"),
                [".bz2"] = (FileBrowserItemCategory.Archive, "application/x-bzip2"),
                [".c"] = (FileBrowserItemCategory.Code, "text/x-c"),
                [".cpp"] = (FileBrowserItemCategory.Code, "text/x-c++"),
                [".cs"] = (FileBrowserItemCategory.Code, "text/x-csharp"),
                [".cshtml"] = (FileBrowserItemCategory.Code, "text/html"),
                [".css"] = (FileBrowserItemCategory.Code, "text/css"),
                [".csv"] = (FileBrowserItemCategory.Data, "text/csv"),
                [".doc"] = (FileBrowserItemCategory.Document, "application/msword"),
                [".docx"] = (FileBrowserItemCategory.Document, "application/vnd.openxmlformats-officedocument.wordprocessingml.document"),
                [".flac"] = (FileBrowserItemCategory.Audio, "audio/flac"),
                [".fs"] = (FileBrowserItemCategory.Code, "text/x-fsharp"),
                [".gif"] = (FileBrowserItemCategory.Image, "image/gif"),
                [".go"] = (FileBrowserItemCategory.Code, "text/x-go"),
                [".gz"] = (FileBrowserItemCategory.Archive, "application/gzip"),
                [".h"] = (FileBrowserItemCategory.Code, "text/x-c"),
                [".htm"] = (FileBrowserItemCategory.Code, "text/html"),
                [".html"] = (FileBrowserItemCategory.Code, "text/html"),
                [".ico"] = (FileBrowserItemCategory.Image, "image/x-icon"),
                [".java"] = (FileBrowserItemCategory.Code, "text/x-java-source"),
                [".jpeg"] = (FileBrowserItemCategory.Image, "image/jpeg"),
                [".jpg"] = (FileBrowserItemCategory.Image, "image/jpeg"),
                [".js"] = (FileBrowserItemCategory.Code, "text/javascript"),
                [".json"] = (FileBrowserItemCategory.Data, "application/json"),
                [".jsx"] = (FileBrowserItemCategory.Code, "text/javascript"),
                [".m4a"] = (FileBrowserItemCategory.Audio, "audio/mp4"),
                [".md"] = (FileBrowserItemCategory.Document, "text/markdown"),
                [".mkv"] = (FileBrowserItemCategory.Video, "video/x-matroska"),
                [".mov"] = (FileBrowserItemCategory.Video, "video/quicktime"),
                [".mp3"] = (FileBrowserItemCategory.Audio, "audio/mpeg"),
                [".mp4"] = (FileBrowserItemCategory.Video, "video/mp4"),
                [".ogg"] = (FileBrowserItemCategory.Audio, "audio/ogg"),
                [".pdf"] = (FileBrowserItemCategory.Document, "application/pdf"),
                [".png"] = (FileBrowserItemCategory.Image, "image/png"),
                [".ppt"] = (FileBrowserItemCategory.Document, "application/vnd.ms-powerpoint"),
                [".pptx"] = (FileBrowserItemCategory.Document, "application/vnd.openxmlformats-officedocument.presentationml.presentation"),
                [".ps1"] = (FileBrowserItemCategory.Code, "text/x-powershell"),
                [".py"] = (FileBrowserItemCategory.Code, "text/x-python"),
                [".rar"] = (FileBrowserItemCategory.Archive, "application/vnd.rar"),
                [".razor"] = (FileBrowserItemCategory.Code, "text/x-csharp"),
                [".rs"] = (FileBrowserItemCategory.Code, "text/x-rust"),
                [".scss"] = (FileBrowserItemCategory.Code, "text/x-scss"),
                [".sh"] = (FileBrowserItemCategory.Code, "application/x-sh"),
                [".sql"] = (FileBrowserItemCategory.Data, "application/sql"),
                [".sqlite"] = (FileBrowserItemCategory.Data, "application/vnd.sqlite3"),
                [".svg"] = (FileBrowserItemCategory.Image, "image/svg+xml"),
                [".tar"] = (FileBrowserItemCategory.Archive, "application/x-tar"),
                [".tgz"] = (FileBrowserItemCategory.Archive, "application/gzip"),
                [".tif"] = (FileBrowserItemCategory.Image, "image/tiff"),
                [".tiff"] = (FileBrowserItemCategory.Image, "image/tiff"),
                [".ts"] = (FileBrowserItemCategory.Code, "text/typescript"),
                [".tsx"] = (FileBrowserItemCategory.Code, "text/typescript"),
                [".txt"] = (FileBrowserItemCategory.Document, "text/plain"),
                [".vb"] = (FileBrowserItemCategory.Code, "text/x-vb"),
                [".wav"] = (FileBrowserItemCategory.Audio, "audio/wav"),
                [".webm"] = (FileBrowserItemCategory.Video, "video/webm"),
                [".webp"] = (FileBrowserItemCategory.Image, "image/webp"),
                [".xls"] = (FileBrowserItemCategory.Document, "application/vnd.ms-excel"),
                [".xlsx"] = (FileBrowserItemCategory.Document, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
                [".xml"] = (FileBrowserItemCategory.Data, "application/xml"),
                [".yaml"] = (FileBrowserItemCategory.Data, "application/yaml"),
                [".yml"] = (FileBrowserItemCategory.Data, "application/yaml"),
                [".zip"] = (FileBrowserItemCategory.Archive, "application/zip")
            });

    private readonly FileSystemFileBrowserOptions options;
    private readonly FileSystemPathResolver pathResolver;

    public FileSystemItemFactory(
        FileSystemFileBrowserOptions options,
        FileSystemPathResolver pathResolver)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.pathResolver = pathResolver ?? throw new ArgumentNullException(nameof(pathResolver));
    }

    public FileSystemEntrySnapshot Capture(ResolvedFileSystemEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        if (entry.IsReparsePoint && !options.FollowDirectoryReparsePoints)
        {
            return new FileSystemEntrySnapshot(
                entry,
                Size: null,
                CreatedAt: null,
                ModifiedAt: null,
                FileBrowserItemCategory.Link,
                MediaType: null,
                LinkTarget: entry.LogicalInfo.LinkTarget);
        }

        entry.MetadataInfo.Refresh();
        long? size = entry.IsDirectory
            ? null
            : entry.MetadataInfo is FileInfo file ? file.Length : null;
        var (category, mediaType) = ResolveType(entry);

        return new FileSystemEntrySnapshot(
            entry,
            size,
            ToDateTimeOffset(entry.MetadataInfo.CreationTimeUtc),
            ToDateTimeOffset(entry.MetadataInfo.LastWriteTimeUtc),
            category,
            mediaType,
            entry.LogicalInfo.LinkTarget);
    }

    public FileBrowserItem CreateRoot(
        FileSystemEntrySnapshot snapshot,
        FileBrowserMetadataRequest metadata)
        => Create(snapshot, parentKey: null, metadata, options.DisplayName);

    public FileBrowserItem Create(
        FileSystemEntrySnapshot snapshot,
        FileBrowserItemKey? parentKey,
        FileBrowserMetadataRequest metadata,
        string? nameOverride = null)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(metadata);

        var requested = metadata.Fields;
        var entry = snapshot.Entry;
        var kind = GetKind(entry);
        var childState = kind == FileBrowserItemKind.Container
            ? FileBrowserChildState.Unknown
            : FileBrowserChildState.Empty;
        var capabilities = FileBrowserItemCapabilities.Select;
        if (kind == FileBrowserItemKind.Container)
        {
            capabilities |= FileBrowserItemCapabilities.Navigate;
        }

        var includeDisplayPath = requested.HasFlag(FileBrowserMetadataFields.DisplayPath);
        var includeSize = requested.HasFlag(FileBrowserMetadataFields.Size);
        var includeMediaType = requested.HasFlag(FileBrowserMetadataFields.MediaType);
        var includeTimestamps = requested.HasFlag(FileBrowserMetadataFields.Timestamps);
        var includeCustom = requested.HasFlag(FileBrowserMetadataFields.Custom);

        if (includeDisplayPath)
        {
            capabilities |= FileBrowserItemCapabilities.CopyPath;
        }

        var exactFields = FileBrowserMetadataFields.Name | FileBrowserMetadataFields.Kind;
        var approximateFields = FileBrowserMetadataFields.None;
        if (kind != FileBrowserItemKind.Container)
        {
            exactFields |= FileBrowserMetadataFields.ChildState;
        }

        if (includeDisplayPath)
        {
            exactFields |= FileBrowserMetadataFields.DisplayPath;
        }

        if (includeSize && snapshot.Size.HasValue)
        {
            exactFields |= FileBrowserMetadataFields.Size;
        }

        if (includeMediaType && snapshot.MediaType is not null)
        {
            approximateFields |= FileBrowserMetadataFields.MediaType;
        }

        if (includeTimestamps && (snapshot.CreatedAt.HasValue || snapshot.ModifiedAt.HasValue))
        {
            exactFields |= FileBrowserMetadataFields.Timestamps;
        }

        IReadOnlyDictionary<string, string>? customMetadata = null;
        if (includeCustom)
        {
            exactFields |= FileBrowserMetadataFields.Custom;
            customMetadata = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["attributes"] = entry.LogicalAttributes.ToString(),
                ["is-reparse-point"] = entry.IsReparsePoint ? "true" : "false",
                ["relative-path"] = entry.RelativeKey
            };
        }

        var suppliedFields = exactFields | approximateFields;
        var completeness = (suppliedFields & requested) == requested
            ? FileBrowserCompleteness.Complete
            : FileBrowserCompleteness.Partial;

        return new FileBrowserItem(
            pathResolver.CreateKey(entry.RelativeKey),
            parentKey,
            nameOverride ?? entry.LogicalInfo.Name,
            kind,
            snapshot.Category,
            displayPath: includeDisplayPath ? entry.LogicalPath : null,
            childState,
            childCount: null,
            size: includeSize ? snapshot.Size : null,
            mediaType: includeMediaType ? snapshot.MediaType : null,
            owner: null,
            createdAt: includeTimestamps ? snapshot.CreatedAt : null,
            modifiedAt: includeTimestamps ? snapshot.ModifiedAt : null,
            contentIdentity: null,
            new FileBrowserMetadataState(
                exactFields,
                approximateFields,
                completeness: completeness),
            capabilities,
            openUri: null,
            downloadUri: null,
            customMetadata);
    }

    private FileBrowserItemKind GetKind(ResolvedFileSystemEntry entry)
    {
        if (entry.IsReparsePoint && !options.FollowDirectoryReparsePoints)
        {
            return FileBrowserItemKind.Link;
        }

        return entry.IsDirectory
            ? FileBrowserItemKind.Container
            : FileBrowserItemKind.File;
    }

    private (FileBrowserItemCategory Category, string? MediaType) ResolveType(
        ResolvedFileSystemEntry entry)
    {
        if (entry.IsReparsePoint && !options.FollowDirectoryReparsePoints)
        {
            return (FileBrowserItemCategory.Link, null);
        }

        if (entry.IsDirectory)
        {
            return (FileBrowserItemCategory.Folder, null);
        }

        var extension = Path.GetExtension(entry.LogicalInfo.Name);
        return KnownTypes.TryGetValue(extension, out var known)
            ? known
            : (FileBrowserItemCategory.Other, null);
    }

    private static DateTimeOffset? ToDateTimeOffset(DateTime value)
        => value == DateTime.MinValue
            ? null
            : new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc));
}
