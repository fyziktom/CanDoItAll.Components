namespace CanDoItAll.Components.FileBrowser.BaseLib;

internal readonly record struct FileBrowserInteractionStamp(long SessionVersion, long SnapshotVersion);

/// <summary>Coordinates asynchronous UI work so stale session, search, and menu completions are ignored.</summary>
internal sealed class FileBrowserInteractionGuard
{
    private readonly object gate = new();
    private long sessionVersion;
    private long snapshotVersion;
    private long searchEditVersion;
    private long? activeSearchEditVersion;

    public FileBrowserInteractionStamp ChangeSession()
    {
        lock (gate)
        {
            sessionVersion++;
            snapshotVersion++;
            searchEditVersion++;
            activeSearchEditVersion = null;
            return new FileBrowserInteractionStamp(sessionVersion, snapshotVersion);
        }
    }

    public FileBrowserInteractionStamp AcceptSnapshot()
    {
        lock (gate)
        {
            snapshotVersion++;
            return new FileBrowserInteractionStamp(sessionVersion, snapshotVersion);
        }
    }

    public FileBrowserInteractionStamp Capture()
    {
        lock (gate)
        {
            return new FileBrowserInteractionStamp(sessionVersion, snapshotVersion);
        }
    }

    public bool IsCurrent(FileBrowserInteractionStamp stamp)
    {
        lock (gate)
        {
            return stamp.SessionVersion == sessionVersion
                && stamp.SnapshotVersion == snapshotVersion;
        }
    }

    public bool IsCurrentSession(long expectedSessionVersion)
    {
        lock (gate)
        {
            return expectedSessionVersion == sessionVersion;
        }
    }

    public long BeginSearchEdit()
    {
        lock (gate)
        {
            var version = ++searchEditVersion;
            activeSearchEditVersion = version;
            return version;
        }
    }

    public bool TryCompleteSearchEdit(long version)
    {
        lock (gate)
        {
            if (activeSearchEditVersion != version)
            {
                return false;
            }

            activeSearchEditVersion = null;
            return true;
        }
    }

    public bool CanSynchronizeCommittedSearch
    {
        get
        {
            lock (gate)
            {
                return activeSearchEditVersion is null;
            }
        }
    }
}
