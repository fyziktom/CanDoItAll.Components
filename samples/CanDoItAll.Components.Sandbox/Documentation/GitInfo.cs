namespace CanDoItAll.Components.Sandbox.Documentation;

/// <summary>
/// Provides the public branch used by Sandbox source links. This must stay browser-safe because
/// the same catalog runs as a standalone WebAssembly app.
/// </summary>
public static class GitInfo
{
    private const string FallbackBranch = "main";

    /// <summary>
    /// The standalone catalog never reads a local checkout.
    /// </summary>
    public static string? RepoRoot => null;

    /// <summary>
    /// The branch used by generated source links.
    /// </summary>
    public static string CurrentBranch => FallbackBranch;
}
