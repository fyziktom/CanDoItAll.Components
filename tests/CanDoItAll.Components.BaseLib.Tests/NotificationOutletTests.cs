namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class NotificationOutletTests {
    [Fact]
    public async Task Moving_between_outlets_keeps_one_message_and_its_original_expiry() {
        var clock = new ControlledTime();
        using var service = new NotificationService(clock);
        var root = Guid.NewGuid();
        var modal = Guid.NewGuid();
        var nested = Guid.NewGuid();
        var closed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        service.RegisterRoot(root, NotificationPosition.BottomCenter);
        var message = service.Notify(NotificationSeverity.Warning, "Review needed", duration: 10_000,
            close: _ => closed.TrySetResult());
        service.SetActiveModal(modal);
        service.SetActiveModal(nested);
        service.SetActiveModal(modal);
        service.SetActiveModal(null);
        Assert.Equal(root, service.ActiveOutlet);
        Assert.Same(message, Assert.Single(service.Snapshot()));
        Assert.Equal(1, clock.CreatedTimers);
        Assert.Equal(TimeSpan.FromSeconds(10), clock.Timer!.DueTime);
        clock.Timer.Fire();
        await closed.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Empty(service.Snapshot());
    }

    [Fact]
    public void Outlet_changes_do_not_duplicate_accessibility_announcements() {
        using var service = new NotificationService();
        var root = Guid.NewGuid();
        var modal = Guid.NewGuid();
        service.RegisterRoot(root, NotificationPosition.TopRight);
        service.Notify(new NotificationMessage { Summary = "First", Duration = 0 });
        Assert.True(service.HasNewAnnouncement);
        service.Presented(root, service.AnnouncementVersion);
        service.SetActiveModal(modal);
        Assert.False(service.HasNewAnnouncement);
        service.Presented(root, long.MaxValue);
        service.Notify(new NotificationMessage { Summary = "Second", Duration = 0 });
        Assert.True(service.HasNewAnnouncement);
        service.Presented(modal, service.AnnouncementVersion);
        service.SetActiveModal(null);
        Assert.False(service.HasNewAnnouncement);
        Assert.Equal(NotificationPosition.TopRight, service.DefaultPosition);
    }

    [Fact]
    public void Separate_host_scopes_never_share_messages_or_modal_state() {
        using var first = new NotificationService();
        using var second = new NotificationService();
        var firstRoot = Guid.NewGuid();
        var secondRoot = Guid.NewGuid();
        first.RegisterRoot(firstRoot, NotificationPosition.BottomLeft);
        second.RegisterRoot(secondRoot, NotificationPosition.BottomRight);
        first.SetActiveModal(Guid.NewGuid());
        first.Notify(new NotificationMessage { Summary = "Local", Duration = 0 });
        Assert.Equal(secondRoot, second.ActiveOutlet);
        Assert.Empty(second.Snapshot());
        Assert.Single(first.Snapshot());
    }

    [Fact]
    public void Dismiss_and_scope_disposal_release_owned_timers_once() {
        var clock = new ControlledTime();
        var service = new NotificationService(clock);
        var closes = 0;
        var message = service.Notify(NotificationSeverity.Info, "Bounded", duration: 1000, close: _ => closes++);
        Assert.True(service.Dismiss(message));
        Assert.False(service.Dismiss(message));
        Assert.True(clock.Timer!.Disposed);
        Assert.Equal(1, closes);
        service.Notify(NotificationSeverity.Info, "Scope end", duration: 1000);
        service.Dispose();
        service.Dispose();
        Assert.True(clock.Timer!.Disposed);
        Assert.Throws<ObjectDisposedException>(() => service.Info("After dispose"));
    }

    private sealed class ControlledTime : TimeProvider {
        public int CreatedTimers { get; private set; }
        public ControlledTimer? Timer { get; private set; }

        public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period) {
            CreatedTimers++;
            return Timer = new(callback, state, dueTime);
        }
    }

    private sealed class ControlledTimer(TimerCallback callback, object? state, TimeSpan dueTime) : ITimer {
        public TimeSpan DueTime { get; private set; } = dueTime;
        public bool Disposed { get; private set; }

        public bool Change(TimeSpan due, TimeSpan period) {
            DueTime = due;
            return !Disposed;
        }

        public void Fire() {
            if (!Disposed) {
                callback(state);
            }
        }

        public void Dispose() => Disposed = true;

        public ValueTask DisposeAsync() {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
