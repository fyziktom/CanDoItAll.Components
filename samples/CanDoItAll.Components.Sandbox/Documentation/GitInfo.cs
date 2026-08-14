using System.Diagnostics;

namespace CanDoItAll.Components.Sandbox.Documentation;

/// <summary>
/// Reports the git branch currently checked out in the repo the sandbox is running from, so
/// example pages can link to source files on the branch a developer is actually looking at
/// instead of a hardcoded <c>main</c>.
/// </summary>
public static class GitInfo
{
    private const string FallbackBranch = "main";

    private static readonly Lazy<string?> Root = new(() => FindRepoRoot(AppContext.BaseDirectory));

    private static readonly Lazy<string> Branch = new(DetectBranch);

    /// <summary>
    /// The root directory of the git checkout the sandbox is running from, or <c>null</c> when
    /// it isn't running from inside one. Computed once per process.
    /// </summary>
    public static string? RepoRoot => Root.Value;

    /// <summary>
    /// The current branch name, or <see cref="FallbackBranch"/> when it can't be determined
    /// (git isn't installed, the app isn't running from inside a git checkout, detached HEAD,
    /// and so on). Computed once per process; restart the sandbox after switching branches.
    /// </summary>
    public static string CurrentBranch => Branch.Value;

    private static string DetectBranch()
    {
        try
        {
            var repoRoot = RepoRoot;
            if (repoRoot is null)
            {
                return FallbackBranch;
            }

            var startInfo = new ProcessStartInfo("git", "rev-parse --abbrev-ref HEAD")
            {
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process is null)
            {
                return FallbackBranch;
            }

            var output = process.StandardOutput.ReadToEnd().Trim();
            var exited = process.WaitForExit(2000);

            if (!exited || process.ExitCode != 0 || string.IsNullOrEmpty(output) || output == "HEAD")
            {
                return FallbackBranch;
            }

            return output;
        }
        catch
        {
            return FallbackBranch;
        }
    }

    private static string? FindRepoRoot(string startDirectory)
    {
        for (var directory = new DirectoryInfo(startDirectory); directory is not null; directory = directory.Parent)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, ".git")))
            {
                return directory.FullName;
            }
        }

        return null;
    }
}
