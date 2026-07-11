using CanDoItAll.Components.FileBrowser.Core;

namespace CanDoItAll.Components.FileBrowser.Sandbox.Providers;

internal static class DemoFileBrowserSources
{
    public static InMemoryFileBrowserProvider CreateProjects()
    {
        var sourceId = new FileBrowserSourceId("projects");
        var tree = new DemoTreeBuilder(sourceId, "CanDoItAll projects", "/projects", "workspaces");

        var web = tree.Folder(tree.Root, "CanDoItAll.Web", owner: "Platform team");
        var modules = tree.Folder(web, "modules", owner: "Platform team");
        var projectStructure = tree.Folder(web, "project-structure", owner: "Agents");
        var resources = tree.Folder(web, "resources", owner: "Content ops");
        tree.File(web, "README.md", FileBrowserItemCategory.Document, 18_420, "text/markdown", "Platform team");
        tree.File(web, "CanDoItAll.Web.slnx", FileBrowserItemCategory.Code, 3_842, "application/xml", "Platform team");
        tree.File(modules, "ProjectsModule.cs", FileBrowserItemCategory.Code, 42_118, "text/x-csharp", "Platform team");
        tree.File(modules, "ResourcesModule.cs", FileBrowserItemCategory.Code, 31_710, "text/x-csharp", "Platform team");
        tree.File(projectStructure, "project-index.json", FileBrowserItemCategory.Data, 284_190, "application/json", "Agents");
        tree.File(resources, "agent-handbook.pdf", FileBrowserItemCategory.Document, 2_840_100, "application/pdf", "Content ops");
        tree.File(resources, "architecture-map.svg", FileBrowserItemCategory.Image, 448_230, "image/svg+xml", "Design systems");

        var ipfs = tree.Folder(tree.Root, "CanDoItAll.IPFS", owner: "Storage team");
        var engine = tree.Folder(ipfs, "src · Engine", owner: "Storage team");
        var nodeControl = tree.Folder(ipfs, "src · NodeControl", owner: "Storage team");
        var ipfsResources = tree.Folder(ipfs, "IPFS resources", owner: "Storage team", icon: "deployed_code");
        tree.File(ipfs, "CanDoItAll.IPFS.slnx", FileBrowserItemCategory.Code, 5_231, "application/xml", "Storage team");
        tree.File(engine, "FileSystemApi.cs", FileBrowserItemCategory.Code, 27_800, "text/x-csharp", "Storage team");
        tree.File(nodeControl, "NodeExplorerWorkflowService.cs", FileBrowserItemCategory.Code, 49_610, "text/x-csharp", "Storage team");
        tree.File(ipfsResources, "QmProductManual", FileBrowserItemCategory.Link, 0, "application/vnd.ipld.raw", "Storage team", cid: "bafybeigdyrzt5sfp7udm7hu76uh7y26nf3efuylqabf3oclvt7kni5j2ma");

        AddCompactProject(tree, "CanDoItAll.Economy", "Simulation team", "EconomyEngine.cs", 63_144);
        AddCompactProject(tree, "CanDoItAll.Agents", "Agent platform", "AgentRuntime.cs", 71_330);
        AddCompactProject(tree, "CanDoItAll.Components", "Design systems", "Directory.Build.props", 4_210);
        AddCompactProject(tree, "CanDoItAll.Processes", "Workflow team", "ProcessCoordinator.cs", 38_810);
        AddCompactProject(tree, "CanDoItAll.CognitiveMemory", "Memory team", "MemoryProfile.cs", 22_904);
        AddCompactProject(tree, "CanDoItAll.Workflows", "Workflow team", "WorkflowDefinition.cs", 44_190);
        AddCompactProject(tree, "CanDoItAll.CodeAnalytics", "Developer tools", "SnapshotService.cs", 58_004);
        AddCompactProject(tree, "CanDoItAll.Chat", "Collaboration team", "Conversation.cs", 19_800);
        AddCompactProject(tree, "CanDoItAll.Providers", "Integrations", "ProviderCatalog.cs", 16_522);
        tree.Folder(
            tree.Root,
            "New project (empty)",
            owner: "Unassigned",
            icon: "create_new_folder",
            hasChildren: false);

        return new InMemoryFileBrowserProvider(
            new FileBrowserSourceDescriptor(
                sourceId,
                "Projects",
                icon: "account_tree",
                description: "Projects, subprojects, and attached resources",
                capabilities: FileBrowserSourceCapabilities.PagedBrowse
                    | FileBrowserSourceCapabilities.RecursiveBrowse
                    | FileBrowserSourceCapabilities.NativeSearch,
                recommendedPageSize: 8,
                maximumPageSize: 100,
                supportedSearchScopes:
                [
                    FileBrowserSearchScope.LoadedFolder,
                    FileBrowserSearchScope.LoadedDescendants,
                    FileBrowserSearchScope.Provider,
                    FileBrowserSearchScope.Progressive
                ],
                metadata: new Dictionary<string, string> { ["kind"] = "project-catalog" }),
            tree.Items);
    }

    public static InMemoryFileBrowserProvider CreateIpfs()
    {
        var sourceId = new FileBrowserSourceId("ipfs");
        var tree = new DemoTreeBuilder(sourceId, "IPFS node", "/ipfs", "cloud");
        var pinned = tree.Folder(tree.Root, "Pinned collections", owner: "Local node", icon: "keep");
        var product = tree.Folder(pinned, "Product knowledge", owner: "Knowledge team", icon: "deployed_code");
        var releases = tree.Folder(pinned, "Release artifacts", owner: "Build automation", icon: "deployed_code");
        var imports = tree.Folder(tree.Root, "Recent imports", owner: "Local node", icon: "move_to_inbox");
        var shared = tree.Folder(tree.Root, "Shared through IPNS", owner: "Remote peers", icon: "public");

        tree.File(product, "agent-handbook.pdf", FileBrowserItemCategory.Document, 2_840_100, "application/pdf", "Knowledge team", cid: "bafybeigdyrzt5sfp7udm7hu76uh7y26nf3efuylqabf3oclvt7kni5j2ma");
        tree.File(product, "architecture-map.svg", FileBrowserItemCategory.Image, 448_230, "image/svg+xml", "Knowledge team", cid: "bafybeif7z5sq4bd3hzlqphqmqx3tctk65d6gt4opzwnxg5nd25xtxqvs7a");
        tree.File(product, "catalog.json", FileBrowserItemCategory.Data, 84_220, "application/json", "Knowledge team", cid: "bafkreibwifsnlt5we2gsvf76yq7xi5zq34e6w3cfym46q7f4mksvuj7v5u");
        tree.File(releases, "components-10.0.4.nupkg", FileBrowserItemCategory.Archive, 7_880_220, "application/zip", "Build automation", cid: "bafybeia6yq7cmx3j2f6wpp6ph2a3flvxmj7vwqf4qjfdmuw7jfn2v3xp3q");
        tree.File(releases, "checksums.sha256", FileBrowserItemCategory.Data, 1_944, "text/plain", "Build automation", cid: "bafkreie5nqvazsh6h7rh3fdj4k5xbrfqlrnfj7pw2a7vyqtbzxv5g2u4je");
        tree.File(imports, "field-recording.mp4", FileBrowserItemCategory.Video, 48_882_110, "video/mp4", "Andrej", cid: "bafybeihq2zjvazs6u3lkmv57ehhqbfl5jkaoy4xgl4fj5v4l7c6qlg7tve");
        tree.File(imports, "office-photo.jpg", FileBrowserItemCategory.Image, 3_440_870, "image/jpeg", "Andrej", cid: "bafkreid4jsoab2wzq6lmqz5tbd7nfkygsv7h4g3jfd3xjhm4j2tnv5mti4");
        tree.File(shared, "roadmap.md", FileBrowserItemCategory.Document, 28_410, "text/markdown", "Remote peer", cid: "bafkreib2ltbvzp7b5c55mr2fwmc3dqc5fk4v2v6xvxx6zuut5dssu42lem");

        // The same immutable content appears in another hierarchy occurrence on purpose.
        tree.File(imports, "agent-handbook-copy.pdf", FileBrowserItemCategory.Document, 2_840_100, "application/pdf", "Andrej", cid: "bafybeigdyrzt5sfp7udm7hu76uh7y26nf3efuylqabf3oclvt7kni5j2ma");

        return new InMemoryFileBrowserProvider(
            new FileBrowserSourceDescriptor(
                sourceId,
                "IPFS node",
                icon: "cloud",
                description: "Shallow DAG occurrences backed by CIDs",
                capabilities: FileBrowserSourceCapabilities.PagedBrowse
                    | FileBrowserSourceCapabilities.NativeSearch,
                recommendedPageSize: 8,
                maximumPageSize: 100,
                supportedSearchScopes:
                [
                    FileBrowserSearchScope.LoadedFolder,
                    FileBrowserSearchScope.LoadedDescendants,
                    FileBrowserSearchScope.Provider,
                    FileBrowserSearchScope.Progressive
                ],
                metadata: new Dictionary<string, string> { ["kind"] = "ipfs-demo" }),
            tree.Items,
            TimeSpan.FromMilliseconds(160));
    }

    public static IFileBrowserProvider CreateRecoveryDemo()
    {
        var sourceId = new FileBrowserSourceId("recovery");
        var tree = new DemoTreeBuilder(sourceId, "Recovery lab", "/recovery", "cloud_off");
        tree.File(
            tree.Root,
            "provider-recovered.txt",
            FileBrowserItemCategory.Document,
            1_024,
            "text/plain",
            "Sandbox");
        var inner = new InMemoryFileBrowserProvider(
            new FileBrowserSourceDescriptor(
                sourceId,
                "Recovery demo",
                icon: "cloud_off",
                description: "Retryable first request, successful retry",
                capabilities: FileBrowserSourceCapabilities.PagedBrowse,
                recommendedPageSize: 8,
                maximumPageSize: 100,
                supportedSearchScopes:
                [
                    FileBrowserSearchScope.LoadedFolder,
                    FileBrowserSearchScope.LoadedDescendants,
                    FileBrowserSearchScope.Progressive
                ],
                metadata: new Dictionary<string, string> { ["kind"] = "failure-demo" }),
            tree.Items,
            TimeSpan.FromMilliseconds(180));
        return new RetryOnceFileBrowserProvider(inner);
    }

    private static void AddCompactProject(
        DemoTreeBuilder tree,
        string name,
        string owner,
        string primaryFile,
        long size)
    {
        var project = tree.Folder(tree.Root, name, owner);
        var src = tree.Folder(project, "src", owner);
        tree.File(project, "README.md", FileBrowserItemCategory.Document, 12_400, "text/markdown", owner);
        tree.File(src, primaryFile, FileBrowserItemCategory.Code, size, "text/x-csharp", owner);
    }

    private sealed class DemoTreeBuilder
    {
        private readonly List<FileBrowserItem> items = [];
        private readonly FileBrowserSourceId sourceId;
        private readonly DateTimeOffset baseline = new(2026, 7, 10, 14, 30, 0, TimeSpan.Zero);
        private int sequence;

        public DemoTreeBuilder(
            FileBrowserSourceId sourceId,
            string rootName,
            string rootPath,
            string rootIcon)
        {
            this.sourceId = sourceId;
            Root = new FileBrowserItemKey(sourceId, rootPath);
            items.Add(new FileBrowserItem(
                Root,
                null,
                rootName,
                FileBrowserItemKind.Container,
                FileBrowserItemCategory.Folder,
                rootPath,
                FileBrowserChildState.HasChildren,
                capabilities: ContainerCapabilities,
                metadata: new Dictionary<string, string> { ["icon"] = rootIcon }));
        }

        public FileBrowserItemKey Root { get; }

        public IReadOnlyList<FileBrowserItem> Items => items;

        public FileBrowserItemKey Folder(
            FileBrowserItemKey parent,
            string name,
            string owner,
            string? icon = null,
            bool hasChildren = true)
        {
            var path = Combine(parent.Value, name);
            var key = new FileBrowserItemKey(sourceId, path);
            items.Add(new FileBrowserItem(
                key,
                parent,
                name,
                FileBrowserItemKind.Container,
                FileBrowserItemCategory.Folder,
                path,
                hasChildren ? FileBrowserChildState.HasChildren : FileBrowserChildState.Empty,
                owner: owner,
                modifiedAt: NextTimestamp(),
                capabilities: ContainerCapabilities,
                metadata: icon is null ? null : new Dictionary<string, string> { ["icon"] = icon }));
            return key;
        }

        public void File(
            FileBrowserItemKey parent,
            string name,
            FileBrowserItemCategory category,
            long size,
            string mediaType,
            string owner,
            string? cid = null)
        {
            var path = Combine(parent.Value, name);
            var contentIdentity = cid is null
                ? new FileBrowserContentIdentity("sha256", $"demo-{Math.Abs(path.GetHashCode(StringComparison.Ordinal)):x8}")
                : new FileBrowserContentIdentity("cid", cid);
            items.Add(new FileBrowserItem(
                new FileBrowserItemKey(sourceId, path),
                parent,
                name,
                category == FileBrowserItemCategory.Link ? FileBrowserItemKind.Link : FileBrowserItemKind.File,
                category,
                path,
                FileBrowserChildState.Empty,
                size: size,
                mediaType: mediaType,
                owner: owner,
                createdAt: baseline.AddDays(-30),
                modifiedAt: NextTimestamp(),
                contentIdentity: contentIdentity,
                capabilities: FileCapabilities
                    | (cid is null ? FileBrowserItemCapabilities.None : FileBrowserItemCapabilities.CopyContentIdentity)));
        }

        private DateTimeOffset NextTimestamp() => baseline.AddHours(-sequence++ * 5);

        private static string Combine(string parent, string name)
            => $"{parent.TrimEnd('/')}/{name}";

        private const FileBrowserItemCapabilities ContainerCapabilities =
            FileBrowserItemCapabilities.Select
            | FileBrowserItemCapabilities.Navigate
            | FileBrowserItemCapabilities.Open
            | FileBrowserItemCapabilities.OpenInNewTab
            | FileBrowserItemCapabilities.DownloadDirectory
            | FileBrowserItemCapabilities.CopyPath;

        private const FileBrowserItemCapabilities FileCapabilities =
            FileBrowserItemCapabilities.Select
            | FileBrowserItemCapabilities.Open
            | FileBrowserItemCapabilities.OpenInNewTab
            | FileBrowserItemCapabilities.DownloadFile
            | FileBrowserItemCapabilities.CopyPath
            | FileBrowserItemCapabilities.Preview;
    }
}
