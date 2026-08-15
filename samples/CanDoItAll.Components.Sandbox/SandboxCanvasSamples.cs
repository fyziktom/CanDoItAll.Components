using CanDoItAll.Components.CanvasLib;

namespace CanDoItAll.Components.Sandbox;

public static class SandboxCanvasSamples
{
    public const string InspectorWindowId = "sandbox-inspector";

    private static readonly DateTimeOffset AnchorDate = new(2026, 4, 6, 8, 0, 0, TimeSpan.Zero);

    public static CanvasWorkbenchUiState CreateWorkbenchUiState()
    {
        var state = new CanvasWorkbenchUiState
        {
            SelectedNodeIds = ["root"],
            ShowDiagnostics = true,
            ShowMinimap = true,
            Zoom = 0.92,
            PanX = 78,
            PanY = 96,
            ActiveInspectorTab = "metrics"
        };

        state.GroupFrames =
        [
            new CanvasWorkbenchGroupFrame
            {
                Id = "sandbox-proof-frame",
                Label = "Shared proof lane",
                Tone = "sky",
                AnchorNodeIds = ["foundations", "inputs", "feedback"]
            }
        ];

        return state;
    }

    public static CanvasWorkbenchWindowState CreateInspectorWindowState()
    {
        return CanvasWorkbenchWindowState.Normalize(new CanvasWorkbenchWindowState
        {
            IsVisible = true,
            IsMinimized = false,
            Width = 440,
            Height = 500
        });
    }

    public static CanvasWorkbenchSurface CreateWorkbenchSurface(
        CanvasWorkbenchUiState? uiState,
        CanvasWorkbenchWindowState inspectorWindowState)
    {
        var state = CloneUiState(uiState ?? CreateWorkbenchUiState());
        state.WindowStates = new Dictionary<string, CanvasWorkbenchWindowState>(StringComparer.Ordinal)
        {
            [InspectorWindowId] = CanvasWorkbenchWindowState.Normalize(inspectorWindowState)
        };

        return new CanvasWorkbenchSurface
        {
            SurfaceId = "sandbox-canvas:proof-workspace",
            Mode = "catalog",
            UiState = state,
            Chrome = CreateChrome(),
            Nodes = BuildNodes(),
            Links = BuildLinks()
        };
    }

    public static CanvasCalendarSurface CreateCalendarSurface()
    {
        var events = BuildEvents();
        var selectedEventId = events.FirstOrDefault()?.EventId ?? string.Empty;
        var selectedDate = events.FirstOrDefault()?.StartUtc?.ToString("yyyy-MM-dd") ?? AnchorDate.ToString("yyyy-MM-dd");
        const bool isReadOnly = false;

        return new CanvasCalendarSurface
        {
            SurfaceId = "sandbox-calendar:proof-schedule",
            Events = events,
            InitialView = "week",
            SelectedDate = selectedDate,
            SelectedEventId = selectedEventId,
            Timezone = "UTC",
            Locale = "en-US",
            WeekStartsOn = 1,
            SlotMinutes = 30,
            BusinessHoursStart = 7,
            BusinessHoursEnd = 21,
            MiniMonthCount = 2,
            AllowCreate = !isReadOnly,
            AllowEdit = !isReadOnly,
            AllowDelete = !isReadOnly,
            AllowDragDrop = !isReadOnly,
            AllowResize = !isReadOnly,
            EnableListExport = true,
            WorkspaceModal = true,
            EventTypes = ["Review", "Proof", "Release"],
            EventStatuses = ["Planned", "Ready", "Blocked"],
            TimeZoneOptions = ["UTC", "America/New_York", "Europe/Prague"]
        };
    }

    private static CanvasWorkbenchChrome CreateChrome()
    {
        return new CanvasWorkbenchChrome
        {
            HintText = "Use the shared workbench for denser proof reviews, floating inspector context, and typed navigation across the extracted component system.",
            EmptyStateKicker = "Canvas sandbox",
            EmptyStateTitle = "Inspect the shared canvas",
            EmptyStateDescription = "Use the same typed workbench surface, floating window, and preview cards that the runtime pages consume.",
            FocusActionLabel = "Focus root",
            ShowFocusAction = true,
            ShowQuickCreateRail = true,
            QuickCreateActions =
            [
                new CanvasWorkbenchAction
                {
                    ActionId = "create-note",
                    Label = "Note",
                    MenuLabel = "Note",
                    Description = "Create a quick review note.",
                    Tone = "accent"
                },
                new CanvasWorkbenchAction
                {
                    ActionId = "create-checklist",
                    Label = "Checklist",
                    MenuLabel = "Checklist",
                    Description = "Create a proof checklist item.",
                    Tone = "success"
                }
            ]
        };
    }

    private static List<CanvasWorkbenchNode> BuildNodes()
    {
        const bool isReadOnly = false;
        const string rootTitle = "Shared component migration workspace with extended validation narrative";
        const string feedbackTitle = "Feedback, status, and notification surfaces with longer descriptive copy";

        var nodes = new List<CanvasWorkbenchNode>
        {
            new()
            {
                Id = "root",
                Family = "root",
                Kind = "workspace",
                Icon = "dashboard",
                Title = rootTitle,
                Subtitle = "Catalog root",
                LeadText = "Shared workbench surfaces prove that extracted canvas primitives still support authoring and review flows.",
                Status = "Ready",
                StatusPill = "Catalog",
                AccentColor = "#0f172a",
                PaletteKey = "neutral",
                IsRequired = true,
                IsReadOnly = isReadOnly,
                IsCollapsible = true,
                X = 180,
                Y = 170,
                Chips =
                [
                    new CanvasWorkbenchChip { Text = "Shared", Tone = "neutral" },
                    new CanvasWorkbenchChip { Text = "Typed", Tone = "success" }
                ]
            },
            new()
            {
                Id = "foundations",
                ParentId = "root",
                Family = "group",
                Kind = "group",
                Icon = "palette",
                Title = "Foundations",
                Subtitle = "Type, spacing, icon rhythm",
                LeadText = "The first shared group proves the visual language before more complex interaction layers join the page.",
                Status = "Ready",
                StatusPill = "Baseline",
                AccentColor = "#7c3aed",
                PaletteKey = "violet",
                IsCollapsible = true,
                IsReadOnly = isReadOnly,
                X = 520,
                Y = 60,
                Chips =
                [
                    new CanvasWorkbenchChip { Text = "Typography", Tone = "accent" }
                ]
            },
            new()
            {
                Id = "inputs",
                ParentId = "root",
                Family = "group",
                Kind = "group",
                Icon = "input",
                Title = "Inputs",
                Subtitle = "Entry and review",
                LeadText = "Field state, helper copy, and disabled behavior stay explicit inside the shared system.",
                Status = "Active",
                StatusPill = "Editable",
                AccentColor = "#2563eb",
                PaletteKey = "sky",
                IsCollapsible = true,
                IsReadOnly = isReadOnly,
                X = 520,
                Y = 250,
                Chips =
                [
                    new CanvasWorkbenchChip { Text = "Forms", Tone = "accent" }
                ]
            },
            new()
            {
                Id = "feedback",
                ParentId = "root",
                Family = "item",
                Kind = "surface",
                Icon = "feedback",
                Title = feedbackTitle,
                Subtitle = "Status surfaces",
                LeadText = "Alerts, empty states, and toasts stay specific and calm instead of feeling theatrical.",
                Status = "Review",
                StatusPill = "Dense pass",
                AccentColor = "#0f766e",
                PaletteKey = "mint",
                IsReadOnly = isReadOnly,
                X = 900,
                Y = 80,
                Chips =
                [
                    new CanvasWorkbenchChip { Text = "Alerts", Tone = "success" },
                    new CanvasWorkbenchChip { Text = "Toasts", Tone = "info" }
                ]
            },
            new()
            {
                Id = "canvas",
                ParentId = "root",
                Family = "special",
                Kind = "runtime",
                Icon = "draw",
                Title = "Canvas runtime",
                Subtitle = "Workbench + calendar",
                LeadText = "The runtime workbench stays focused on authoring while preview assets and boundary cards live in the sandbox.",
                Status = "Shared",
                StatusPill = "Canvas",
                AccentColor = "#d97706",
                PaletteKey = "warn",
                IsReadOnly = isReadOnly,
                X = 900,
                Y = 260,
                Chips =
                [
                    new CanvasWorkbenchChip { Text = "Workbench", Tone = "warning" },
                    new CanvasWorkbenchChip { Text = "Calendar", Tone = "warning" }
                ]
            }
        };

        nodes.Add(new CanvasWorkbenchNode
        {
            Id = "proof",
            ParentId = "canvas",
            Family = "item",
            Kind = "proof",
            Icon = "photo_camera",
            Title = "Dense desktop and mobile capture",
            Subtitle = "Proof queue",
            LeadText = "Screenshot proof stays connected to the same shared catalog model instead of living in a separate document.",
            Status = "Pending",
            StatusPill = "Capture",
            AccentColor = "#e11d48",
            PaletteKey = "rose",
            IsReadOnly = isReadOnly,
            X = 1220,
            Y = 180,
            Chips =
            [
                new CanvasWorkbenchChip { Text = "Desktop", Tone = "danger" },
                new CanvasWorkbenchChip { Text = "Mobile", Tone = "danger" }
            ]
        });

        return nodes;
    }

    private static List<CanvasWorkbenchLink> BuildLinks()
    {
        var links = new List<CanvasWorkbenchLink>
        {
            new() { SourceId = "root", TargetId = "foundations", Kind = "contains" },
            new() { SourceId = "root", TargetId = "inputs", Kind = "contains" },
            new() { SourceId = "root", TargetId = "feedback", Kind = "contains" },
            new() { SourceId = "root", TargetId = "canvas", Kind = "contains" },
            new() { SourceId = "foundations", TargetId = "feedback", Kind = "supports" },
            new() { SourceId = "inputs", TargetId = "canvas", Kind = "feeds" }
        };

        links.Add(new CanvasWorkbenchLink
        {
            SourceId = "canvas",
            TargetId = "proof",
            Kind = "requires",
            IsUserAuthored = true
        });

        return links;
    }

    private static List<CanvasCalendarEvent> BuildEvents()
    {
        const string kickoffTitle = "Shared component rollout review with extended acceptance notes";

        var events = new List<CanvasCalendarEvent>
        {
            new()
            {
                Id = "review-kickoff",
                EventId = "review-kickoff",
                Title = kickoffTitle,
                Description = "Review the extracted shared components and confirm the acceptance checklist.",
                StartUtc = AnchorDate.AddHours(1),
                EndUtc = AnchorDate.AddHours(2.5),
                Timezone = "UTC",
                TimezoneName = "UTC",
                LocationLabel = "Design systems room",
                Category = "Review",
                Color = "#2563eb",
                EventType = "Review",
                Status = "Ready",
                Notes = "Primary review window for the migration bundle.",
                LinkedPlaylists =
                [
                    new CanvasCalendarPlaylist
                    {
                        PlaylistId = "playlist-review",
                        Title = "Acceptance evidence",
                        Subtitle = "Desktop and mobile proof",
                        Purpose = "Drive the validation pass",
                        Status = "Ready"
                    }
                ],
                ChecklistRows =
                [
                    new CanvasCalendarChecklistRow { Label = "Desktop capture", Status = "Done", Note = "Base route is ready." },
                    new CanvasCalendarChecklistRow { Label = "Mobile capture", Status = "Pending", Note = "Dense pass still needs inspection." }
                ],
                LinkedPlaylistCount = 1,
                ChecklistItemCount = 2
            },
            new()
            {
                Id = "canvas-proof",
                EventId = "canvas-proof",
                Title = "Canvas boundary proof",
                Description = "Capture workbench, calendar, and preview-card evidence for the extracted canvas library.",
                StartUtc = AnchorDate.AddHours(4),
                EndUtc = AnchorDate.AddHours(5),
                Timezone = "UTC",
                TimezoneName = "UTC",
                LocationLabel = "Canvas review",
                Category = "Proof",
                Color = "#d97706",
                EventType = "Proof",
                Status = "Ready",
                Notes = "Calendar boundary previews should remain connected to the shared surface contract.",
                ChecklistRows =
                [
                    new CanvasCalendarChecklistRow { Label = "Selection panel", Status = "Done", Note = "Snapshot confirmed." },
                    new CanvasCalendarChecklistRow { Label = "Export boundary", Status = "Done", Note = "Formats aligned." }
                ],
                ChecklistItemCount = 2
            }
        };

        events.Add(new CanvasCalendarEvent
        {
            Id = "release-window",
            EventId = "release-window",
            Title = "Release window",
            Description = "Ship the shared component extraction once proof and MCP indexing are complete.",
            StartUtc = AnchorDate.AddHours(7),
            EndUtc = AnchorDate.AddHours(8),
            Timezone = "UTC",
            TimezoneName = "UTC",
            LocationLabel = "Deployment lane",
            Category = "Release",
            Color = "#0f766e",
            EventType = "Release",
            Status = "Blocked",
            Notes = "This remains blocked until dense and empty-state screenshots are reviewed.",
            ChecklistRows =
            [
                new CanvasCalendarChecklistRow { Label = "Dense proof", Status = "Pending", Note = "Awaiting visual review." },
                new CanvasCalendarChecklistRow { Label = "MCP validation", Status = "Pending", Note = "Tool contracts still need testing." }
            ],
            ChecklistItemCount = 2
        });

        return events;
    }

    private static CanvasWorkbenchUiState CloneUiState(CanvasWorkbenchUiState state)
    {
        return CanvasWorkbenchUiState.Parse(state.ToJson());
    }
}
