using System.Linq;
using CanDoItAll.Components.BaseLib;
using CanDoItAll.Components.CanvasLib;
using CanDoItAll.Components.Charts;
using CanDoItAll.Components.Gantt;
using CanDoItAll.Components.Mermaid;
using CanDoItAll.Components.OverlayLib;
using CanDoItAll.Components.QRCode.Components;
using CanDoItAll.Components.Sandbox.Components;

namespace CanDoItAll.Components.Sandbox;

public enum SandboxGroupKey
{
    Typography,
    Identity,
    Buttons,
    Badges,
    Forms,
    Cards,
    Lists,
    Navigation,
    Layout,
    Feedback,
    Modals,
    DataDisplay,
    DataVisualization,
    Storage,
    Charts,
    Mermaid,
    QrCode,
    Gantt,
    Overlays,
    Canvas,
    Benchmark,
    Transitions
}

public enum SandboxFramePreset
{
    LiveViewport,
    Tablet,
    Mobile
}

public sealed record SandboxGroupDefinition(
    SandboxGroupKey Key,
    string Title,
    string Route,
    string Summary,
    IReadOnlyList<string> FocusAreas,
    IReadOnlyList<string> ProofNotes)
{
    // public bool IsStandardProofGroup => Key is not SandboxGroupKey.Canvas;
    public bool IsStandardProofGroup => true;

    public string ProofScope => IsStandardProofGroup ? "Standard" : "Deferred";
}

/// <summary>A named, anchored component section within a catalog page (e.g. the &lt;Button&gt; section on Actions).</summary>
public sealed record SandboxPageSection(string Anchor, string ComponentName, Type ComponentType);

public sealed record SandboxExampleDefinition(
    string Id,
    SandboxGroupKey GroupKey,
    string Title,
    string Route,
    string Summary,
    IReadOnlyList<string> Tags,
    IReadOnlyList<string> ComponentNames);

public static class SandboxCatalogRegistry
{
    public static IReadOnlyList<SandboxGroupDefinition> Groups { get; } =
    [
        new(
            SandboxGroupKey.Typography,
            "Typography",
            "/typography",
            "Type scale, monospace values, and supporting copy treatments for the shared component language.",
            ["TextBlock", "Header", "Divider"],
            ["Readability first.", "Type should stay readable before color or decoration does any work.", "Long text cannot cause accidental narrow columns or broken alignment."]),
        new(
            SandboxGroupKey.Identity,
            "Identity",
            "/identity",
            "Avatar and icon primitives that establish who or what a surface represents.",
            ["Icon", "Avatar"],
            ["Icons should support scanning instead of decoration.", "Avatar fallbacks must stay legible without a photo.", "Identity treatments should stay consistent across generated and uploaded avatars."]),
        new(
            SandboxGroupKey.Buttons,
            "Buttons",
            "/buttons",
            "Primary and secondary action treatments for inline and sectional flows.",
            ["Button", "CopyButton"],
            ["Primary action hierarchy must be obvious.", "Disabled actions should still explain intent.", "Copy affordances should confirm success quickly without turning utility actions into noise."]),
        new(
            SandboxGroupKey.Badges,
            "Badges",
            "/badges",
            "Compact status, count, and label tokens for inline metadata and lightweight tagging.",
            ["Badge", "Chip", "Pill"],
            ["Status language must be calm and actionable.", "Dense badge and chip clusters should wrap instead of overflowing.", "Interactive badges and pills need an obvious pressed and disabled state."]),
        new(
            SandboxGroupKey.Forms,
            "Forms",
            "/forms",
            "Field-level components for fast data entry, review, and configuration flows.",
            ["TextBox", "CheckBox", "Password"],
            ["Form state stays explicit.", "Disabled and long-text states must remain legible.", "Mobile stacking should still scan quickly.", "Keep labels, helper copy, and field spacing legible before validation or decoration is added.", "Dense mode should stack more data, not shrink the affordances into noise.", "Disabled mode still needs to explain what the user is looking at."]),
        new(
            SandboxGroupKey.Cards,
            "Cards",
            "/cards",
            "Card, panel, and metric surfaces for structured summaries and dense review content.",
            ["Card", "AuthCard", "PanelCard"],
            ["Long labels cannot destroy hierarchy.", "Dense metadata should still read as grouped information instead of raw text piled into cards.", "Long labels cannot collapse the card rhythm or force awkward truncation everywhere."]),
        new(
            SandboxGroupKey.Lists,
            "Lists",
            "/lists",
            "Selection and detail-shell primitives for scanning and drilling into dense records.",
            ["ListGroup", "FactTable"],
            ["Long labels cannot collapse the list rhythm.", "Selection state must remain obvious when the list gets dense.", "Detail shells should keep context when nothing is selected yet."]),
        new(
            SandboxGroupKey.Navigation,
            "Navigation",
            "/navigation",
            "Shared navigation primitives for segmenting dense workflows into focused views so pages do not turn into long vertical scrolls.",
            ["Tabs", "Steps", "SideMenu"],
            ["Keyboard navigation should remain coherent.", "The active state must be obvious at a glance.", "Prefer tabs or split shells when users switch between modes instead of reading one tall page top-to-bottom.", "Users should understand the route hierarchy by scanning headings and active states only.", "Dense navigation should add information, not extra ambiguity.", "Keyboard and focus behavior matter here more than decoration."]),
        new(
            SandboxGroupKey.Layout,
            "Layout",
            "/layout",
            "Page-level composition patterns for headers, scaffolds, sections, and split regions.",
            ["Layout", "Grid", "Column"],
            ["Desktop layouts should use width intentionally.", "Mobile should orient the user inside the first viewport.", "Shared shells must avoid ad-hoc structure.", "Wide layouts should use horizontal space on purpose instead of drifting into accidental narrow columns.", "Mobile still needs a readable first viewport with a clear entry point into the page.", "Shared shells should remove ad-hoc page structure rather than becoming decorative wrappers.", "Prefer shared layout primitives (Stack, Grid, Row/Column) over one-off structural wrappers."]),
        new(
            SandboxGroupKey.Feedback,
            "Feedback",
            "/feedback",
            "Status, alerts, loading, and empty-state surfaces that explain system state clearly.",
            ["Alert", "Tooltip"],
            ["Status language must be calm and actionable.", "Loading should not feel like a blank failure.", "Empty states need orientation and a next step.", "Severity should be obvious without overwhelming the rest of the page.", "Empty and loading states still need a clear next step."]),
        new(
            SandboxGroupKey.Modals,
            "Modals",
            "/modals",
            "Dialog and modal shells for focused confirmation and inspection flows.",
            ["Dialog", "DialogHost"],
            ["Modal triggers must read clearly.", "The page should keep context when a dialog opens.", "Dialogs should close predictably without losing entered data."]),
        new(
            SandboxGroupKey.DataDisplay,
            "Data Display",
            "/data-display",
            "Timeline, diff, and stepper surfaces for showing how records changed over time.",
            ["DiffViewer", "Timeline"],
            ["Chronological order must stay obvious at a glance.", "Long diffs and histories need calm grouping instead of an unbroken wall of rows.", "Empty views should reuse shared empty surfaces."]),
        new(
            SandboxGroupKey.DataVisualization,
            "Data Visualization",
            "/data-visualization",
            "Chart primitives and data grid surfaces for scanning trends and tabular series.",
            ["Chart", "DataGrid"],
            ["Dense tabular data must stay scannable at narrow widths.", "Axes, gridlines, and labels cannot crowd the plotted series.", "Empty and loading data states need explicit treatment, not a blank grid."]),
        new(
            SandboxGroupKey.Storage,
            "Storage",
            "/storage",
            "Summary and badge surfaces for storage usage and capacity status.",
            ["StorageSummaryCard"],
            ["Capacity status must be scannable at a glance.", "Near-limit and over-limit states need to read as genuinely different severities.", "Empty and unmeasured storage states still need explicit copy."]),
        new(
            SandboxGroupKey.Overlays,
            "Overlays",
            "/overlays",
            "Contextual and modal interaction services shared across the sandbox — dialog, tooltip, and notification hosts.",
            ["dialog service", "tooltip service", "notifications"],
            ["Dialog, tooltip, and notification hosts should remain a shared concern instead of being reimplemented per page.", "The page should keep context when a service-hosted overlay opens.", "Overlay windows must not trap focus or block dismissal."]),
        new(
            SandboxGroupKey.Charts,
            "Charts",
            "/charts",
            "Apex-backed chart wrappers for operational trends, shares, fills, labels, and color tuning.",
            ["pie", "line", "area"],
            ["Charts must render nonblank browser output.", "Legends, labels, and toolbars cannot crowd the plot.", "Consumers should use CanDoItAll chart models instead of Apex component markup.", "The sandbox page should not use raw chart-library component markup directly.", "Desktop proof must show nonblank chart output with readable legends, labels, and toolbars.", "Mobile proof must show charts staying inside the frame without clipped labels or overlapping summary context."]),
        new(
            SandboxGroupKey.Mermaid,
            "Mermaid",
            "/mermaid",
            "First-party Mermaid.js wrapper for diagrams, syntax diagnostics, click events, and pan/zoom inspection.",
            ["flowcharts", "diagrams"],
            ["Diagrams must render from the vendored official Mermaid resource.", "Clickable nodes should raise .NET events with useful node text and id context.", "Syntax failures must show line, column, excerpt, and expected-token hints when Mermaid provides them.", "Every graph card should display a nonblank SVG produced by the wrapper.", "Click a flowchart or architecture node and confirm the .NET event panel updates with node context.", "Toolbar, wheel, drag, touch, and keyboard viewport interaction should come from the same ZoomPanFrame used by non-Mermaid previews.", "The intentional error example should show line and column details rather than a blank diagram."]),
        new(
            SandboxGroupKey.QrCode,
            "QR Code",
            "/qr",
            "QR rendering, long payload, dialog, and manual scanner fallback coverage.",
            ["qr codes", "scanning"],
            ["Rendered codes must remain square and readable at narrow widths.", "Empty and long payloads need explicit states.", "Scanning must remain usable when camera access is unavailable or denied.", "The SVG stays square, labelled, and contained at desktop and mobile widths.", "Long payloads increase QR density without leaking text or overflowing the card.", "The scanner dialog always offers manual paste when camera access is disabled or unavailable."]),
        new(
            SandboxGroupKey.Gantt,
            "Gantt",
            "/gantt",
            "Controlled interactive project scheduling with dense task metadata, dependency editing, insertion, and export.",
            ["gantt charts"],
            ["Every gesture must raise a typed request instead of mutating caller state.", "Rows, task bars, and dependency ports must remain aligned at dense desktop widths.", "Insertion must replace a real bridge edge and visibly propagate the dependent schedule.", "Task rows and canvas bars share the same row-height contract. Use the typed 0.25 h, 1 h, 1 d, or 1 w scale, drag empty timeline space to pan, and double-click an empty row point to request a new eight-hour task from the host.", "The dense proof combines a Canvas runtime hub with exactly eight incoming and eight outgoing edges and a 43-task delivery chain, forcing both scroll axes while exercising endpoint lanes and exact mouse reconnection.", "The implementation task has two predecessors, proving multiple dependencies without encoding them into task records.", "Drop the staged security review onto the Canvas runtime → Host integration bridge to replace that edge and move the dependent path."]),
        new(
            SandboxGroupKey.Canvas,
            "Canvas",
            "/canvas",
            "Typed workbench and calendar surfaces plus runtime primitives and sandbox-only preview assets.",
            ["workbench", "calendars"],
            ["Canvas contracts stay typed.", "Runtime workbench stays distinct from sandbox previews.", "Desktop width should be used aggressively without losing orientation.", "The runtime workbench should stay focused on authoring behavior rather than carrying preview-only diagnostics.", "Floating windows, selection state, and viewport updates must stay typed across the shared contracts.", "Canvas proof assets belong in the sandbox so dense and empty scenarios can be reviewed without touching the runtime pages."]),
        new(
            SandboxGroupKey.Benchmark,
            "Benchmark",
            "/benchmark",
            "A small, repeatable browser-side comparison of equivalent C# WebAssembly and JavaScript aggregation work.",
            ["WebAssembly", "JS interop"],
            ["Open this page in the SandboxWasm host to measure browser-side .NET WebAssembly.", "Both cases use the same deterministic generator, 1,000 strings, and fruit-count aggregation.", "Treat a single short run as an illustrative microbenchmark; browser, runtime, warm-up, and device affect timings."]),
        new(
            SandboxGroupKey.Transitions,
            "Transitions",
            "/transitions",
            "Animated transitions.",
            ["expand transition"],
            ["Transitions smoothly."])
    ];

    public static IReadOnlyList<SandboxGroupDefinition> StandardGroups { get; }
        = Groups.Where(group => group.IsStandardProofGroup).ToArray();

    public static IReadOnlyList<SandboxGroupDefinition> DeferredGroups { get; }
        = Groups.Where(group => !group.IsStandardProofGroup).ToArray();

    public static IReadOnlyList<SandboxExampleDefinition> Examples { get; } =
    [
        CreateExample("typography-default", SandboxGroupKey.Typography, "Type scale", "Baseline typography, surfaces, and icon alignment.", ["typography", "spacing"], "TextBlock", "Icon", "SummaryTiles"),
        CreateExample("typography-dense", SandboxGroupKey.Typography, "Dense editorial stack", "Denser heading, copy, and icon combinations.", ["dense", "copy"], "TextBlock", "Card"),
        CreateExample("typography-empty", SandboxGroupKey.Typography, "Empty orientation", "Foundation-only empty orientation surface.", ["empty"], "EmptyState"),
        CreateExample("identity-default", SandboxGroupKey.Identity, "Avatars and icons", "Icon language paired with generated and uploaded avatar treatments.", ["icons", "avatars"], "Icon", "Avatar", "RoboAvatar"),
        CreateExample("buttons-default", SandboxGroupKey.Buttons, "Primary action hierarchy", "Primary, secondary, ghost, and copy action contrast.", ["buttons", "copy"], "Button", "CopyButton"),
        CreateExample("buttons-dense", SandboxGroupKey.Buttons, "Dense inline actions", "Action rows inside a compact review surface with copy affordances.", ["dense", "inline", "copy"], "Button", "CopyButton", "StatusBadge"),
        CreateExample("buttons-empty", SandboxGroupKey.Buttons, "No actions available", "Action surface when prerequisites are missing.", ["empty"], "EmptyState", "Alert"),
        CreateExample("badges-default", SandboxGroupKey.Badges, "Status and label tokens", "Badge, chip, and pill treatments for compact metadata and tagging.", ["badges", "chips", "tags"], "Badge", "Chip", "Pill"),
        CreateExample("badges-dense", SandboxGroupKey.Badges, "Dense badge clusters", "Higher-density badge and chip groupings inside a compact review surface.", ["dense", "badges"], "BadgesGroup", "ChipRow", "StatusBadge"),
        CreateExample("badges-empty", SandboxGroupKey.Badges, "No status yet", "Badge surface before any status is available.", ["empty"], "EmptyState"),
        CreateExample("forms-default", SandboxGroupKey.Forms, "Form entry", "Standard field layout with validation and helper context.", ["forms"], "FormField", "TextBox", "DropDown", "Numeric"),
        CreateExample("forms-editable", SandboxGroupKey.Forms, "Inline property editing", "Editable property rows switch from displayed values to shared text and checkbox inputs.", ["forms", "inline-edit"], "Editable", "Stack", "TextBox", "CheckBox"),
        CreateExample("forms-dense", SandboxGroupKey.Forms, "Dense intake form", "High-density entry form with stacked fields.", ["dense", "forms"], "FormSection", "TextArea", "Switch"),
        CreateExample("forms-empty", SandboxGroupKey.Forms, "No draft selected", "Entry workflow before a draft is created.", ["empty"], "EmptyState"),
        CreateExample("cards-default", SandboxGroupKey.Cards, "Summary and list views", "Cards, summaries, and list rows for stable data display.", ["cards"], "Card", "SelectionListItem", "SummaryTiles"),
        CreateExample("lists-default", SandboxGroupKey.Lists, "Selection and detail views", "List-detail shell and selection rows for scanning and drilling into records.", ["lists", "selection"], "SelectionListItem", "ListDetailShell"),
        CreateExample("lists-dense", SandboxGroupKey.Lists, "Dense data review", "Metadata-heavy summaries and compact item rows.", ["dense", "metadata"], "SelectionListItem", "StatusBadge"),
        CreateExample("data-display-default", SandboxGroupKey.DataDisplay, "Change history surfaces", "Diff, timeline, and stepper surfaces for showing how records changed over time.", ["diff", "timeline", "stepper"], "DiffViewer", "Timeline", "TimelineStepper"),
        CreateExample("data-display-empty", SandboxGroupKey.DataDisplay, "No results", "Data display surface before any change history is available.", ["empty"], "EmptyState"),
        CreateExample("navigation-default", SandboxGroupKey.Navigation, "Workspace navigation", "Tabs, steps, tree navigation, and list-detail movement in a standard workflow that keeps dense content segmented instead of vertically stacked.", ["keyboard", "tabs", "treeview", "progressive-disclosure", "reduce-scroll"], "Tabs", "Steps", "TreeView", "TooltipTarget", "ListDetailShell"),
        CreateExample("navigation-dense", SandboxGroupKey.Navigation, "Dense workspace routing", "High-information navigation shell with active detail.", ["dense"], "SecondaryTabs", "SelectionListItem"),
        CreateExample("navigation-empty", SandboxGroupKey.Navigation, "Nothing selected", "Navigation shell before an item is selected.", ["empty"], "ListDetailShell", "EmptyState"),
        CreateCustomExample("navigation-tabs-lab", SandboxGroupKey.Navigation, "Tabs lab", "/navigation#tabs", "Dedicated tabs section covering the readable default setup, advanced accent styling, shell customization, overflow strategies, and edge cases that prove tabs can replace long stacked pages.", ["tabs", "responsive", "edge-cases", "customization", "progressive-disclosure", "reduce-scroll", "overflow"], "Tabs"),
        CreateCustomExample("navigation-side-menu-lab", SandboxGroupKey.Navigation, "Side menu lab", "/navigation#side-menu", "Responsive side menu covering declarative and list-fed items, measured overflow, service-driven context switching, utility panels, persistence, and the small-screen top dropdown.", ["side-menu", "responsive", "overflow", "service", "local-storage", "dropdown"], "SideMenu", "SideMenuItem", "SideMenuService"),
        CreateCustomExample("navigation-treeview-adaptive", SandboxGroupKey.Navigation, "Adaptive TreeView rows", "/navigation#treeview", "Three-level TreeView proof for adaptive labels, compact badges, full-width nesting, and right-positioned service tooltips.", ["treeview", "responsive", "long-text", "tooltip", "ellipsis"], "TreeView", "TreeViewNodeRow", "TooltipTarget", "TooltipService"),
        CreateExample("layout-default", SandboxGroupKey.Layout, "Page scaffold", "Page-level composition with header, lead, rail, section blocks, and a standalone zoom/pan preview.", ["page", "zoom-pan"], "PageScaffold", "SectionCard", "ZoomPanFrame"),
        CreateExample("layout-dense", SandboxGroupKey.Layout, "Dense workspace layout", "Split layout that uses wider desktop space deliberately.", ["dense"], "PageScaffold", "FormSection", "StickyActionFooter"),
        CreateExample("layout-empty", SandboxGroupKey.Layout, "No records in view", "Layout shell with an empty workspace region.", ["empty"], "PageScaffold", "EmptyState"),
        CreateCustomExample("layout-composition", SandboxGroupKey.Layout, "Layout composition lab", "/layout#stack", "Compare Stack, Grid, and Row/Column versions of the same analytics control panel.", ["comparison", "responsive", "layout"], "Stack", "Grid", "Row", "Column", "FormRow"),
        CreateExample("feedback-default", SandboxGroupKey.Feedback, "Status surfaces", "Alerts, badges, notifications, and load states.", ["status"], "Alert", "Notification", "StatusBadge"),
        CreateExample("feedback-dense", SandboxGroupKey.Feedback, "Operational feedback", "Mixed severity feedback inside a busy operational surface.", ["dense"], "Alert", "StatusBadge", "SummaryTiles"),
        CreateExample("feedback-empty", SandboxGroupKey.Feedback, "No activity yet", "Shared empty and loading transitions.", ["empty"], "EmptyState", "LoadingState"),
        CreateExample("modals-default", SandboxGroupKey.Modals, "Dialog shell", "Standard dialog composition for focused confirmation and inspection flows.", ["modals", "dialog"], "Dialog"),
        CreateExample("modals-scaffold", SandboxGroupKey.Modals, "Dialog body layouts", "DialogScaffold-based layouts for danger confirmation, inspector, and picker dialog bodies.", ["modals", "dialog", "scaffold"], "DialogScaffold", "DangerActionDialog", "InspectorDialogLayout", "PickerDialogShell"),
        CreateExample("modals-host", SandboxGroupKey.Modals, "Dialog host", "DialogHost renders whatever DialogService currently has open, mounted once at the app layout.", ["modals", "service"], "DialogHost"),
        CreateExample("data-visualization-default", SandboxGroupKey.DataVisualization, "Data grid", "Tabular series review using the shared data grid wrapper.", ["data grid", "tables"], "DataGrid", "DataGridColumn"),
        CreateExample("data-visualization-empty", SandboxGroupKey.DataVisualization, "No rows", "Data grid empty-state behavior before any rows are available.", ["empty"], "DataGrid", "EmptyState"),
        CreateExample("data-visualization-chart", SandboxGroupKey.DataVisualization, "Line chart series", "Chart, LineSeries, and ValueAxis compose an SVG line chart via cascading registration.", ["charts", "svg"], "Chart", "LineSeries", "ValueAxis"),
        CreateExample("data-visualization-progress", SandboxGroupKey.DataVisualization, "Progress meter", "Standalone progress bar for bounded value ranges.", ["progress", "meter"], "ProgressBar"),
        CreateExample("storage-default", SandboxGroupKey.Storage, "Storage summary", "Badge strip and summary card surfaces for storage usage and capacity status.", ["storage", "badges"], "StorageBadgeStrip", "StorageSummaryCard"),
        CreateExample("storage-empty", SandboxGroupKey.Storage, "No storage summary yet", "Storage summary surface before any usage data is available.", ["empty", "storage"], "EmptyState"),
        CreateExample("charts-default", SandboxGroupKey.Charts, "Trend and share charts", "Area, line, multi-line, and pie examples using the CanDoItAll chart wrapper.", ["charts", "apex", "wrapper"], "CdaChart"),
        CreateExample("charts-dense", SandboxGroupKey.Charts, "Dense operational charts", "Higher-density multi-series charts with legends, labels, and color tuning.", ["dense", "multi-series"], "CdaChart"),
        CreateExample("charts-empty", SandboxGroupKey.Charts, "No chart data", "Wrapper empty-state behavior before any chart series is available.", ["empty"], "CdaChart", "EmptyState"),
        CreateExample("mermaid-flowchart", SandboxGroupKey.Mermaid, "Interactive flowchart", "Flowchart proof with .NET node-click events and pan/zoom controls.", ["mermaid", "flowchart", "click", "pan-zoom"], "MermaidDiagram"),
        CreateExample("mermaid-architecture", SandboxGroupKey.Mermaid, "Architecture beta", "architecture-beta sample using groups, services, junctions, icons, labels, and directional ports.", ["mermaid", "architecture-beta", "services"], "MermaidDiagram"),
        CreateExample("mermaid-error", SandboxGroupKey.Mermaid, "Syntax diagnostics", "Intentional Mermaid syntax failure proving line, column, excerpt, and expected-token rendering.", ["mermaid", "syntax-error", "diagnostics"], "MermaidDiagram", "Alert"),
        CreateExample("qr-default", SandboxGroupKey.QrCode, "QR payload rendering", "URL and verification payloads rendered through the shared SVG component.", ["qr", "svg", "payload"], "QrCodeView", "QrCodeButton"),
        CreateExample("qr-long", SandboxGroupKey.QrCode, "Long QR payload", "Long verification content with high error correction and constrained mobile width.", ["qr", "long-text", "error-correction"], "QrCodeView"),
        CreateExample("qr-empty", SandboxGroupKey.QrCode, "Empty and scanner fallback", "Empty renderer state plus a camera-disabled manual scan dialog.", ["qr", "empty", "scanner", "fallback"], "QrCodeView", "QrScanButton"),
        CreateExample("gantt-default", SandboxGroupKey.Gantt, "Controlled project schedule", "Move and resize tasks, reconnect dependencies, independently enable task requests from empty timeline points, reorder rows, insert work into a bridge edge, edit titles, and export the controlled projection.", ["gantt", "controlled", "interactive"], "GanttChart", "GanttTaskDragSource"),
        CreateExample("gantt-dense", SandboxGroupKey.Gantt, "Dense dependency hub", "Eight incoming and eight outgoing hub edges prove compact assignment indicators, endpoint lanes, and exact mouse reconnection under fan pressure.", ["dense", "dependencies", "assignments", "reconnection"], "GanttChart", "GanttAssignment"),
        CreateExample("gantt-empty", SandboxGroupKey.Gantt, "Empty schedule", "Explicit zero-task surface without synthetic schedule data.", ["empty", "gantt"], "GanttChart"),
        CreateExample("overlays-default", SandboxGroupKey.Overlays, "Overlay services", "DialogService, TooltipService, and NotificationService hosting contextual overlays.", ["overlay", "services"], "DialogService", "DialogHost", "TooltipService", "Tooltip", "NotificationService"),
        CreateExample("overlays-dense", SandboxGroupKey.Overlays, "Dense approval flow", "Inline approval surface with secondary actions, modal affordance, and service-hosted overlays.", ["dense", "approval"], "DialogService", "TooltipService", "NotificationService", "Alert"),
        CreateExample("overlays-empty", SandboxGroupKey.Overlays, "No modal context", "Overlay surface before the user selects a record.", ["empty"], "EmptyState", "DialogHost"),
        CreateExample("canvas-default", SandboxGroupKey.Canvas, "Workbench surface", "Shared workbench, floating windows, and calendar surface.", ["canvas", "workbench"], "CanvasWorkbench", "CanvasFloatingWindow", "CanvasCalendar"),
        CreateExample("canvas-dense", SandboxGroupKey.Canvas, "Dense workbench review", "Deeper workbench, preview, and calendar density.", ["dense", "canvas"], "CanvasWorkbench", "CanvasSceneHostPreview", "LayerStackPreview"),
        CreateExample("canvas-empty", SandboxGroupKey.Canvas, "Empty canvas state", "Canvas empty-state coverage across workbench and calendar.", ["empty", "canvas"], "CanvasWorkbench", "CanvasCalendar", "EmptyStateOverlay")
    ];

    public static IReadOnlyList<SandboxExampleDefinition> StandardExamples { get; }
        = Examples.Where(example => GetGroup(example.GroupKey).IsStandardProofGroup).ToArray();

    public static IReadOnlyList<SandboxExampleDefinition> DeferredExamples { get; }
        = Examples.Where(example => !GetGroup(example.GroupKey).IsStandardProofGroup).ToArray();

    public static IReadOnlyList<string> ValidationQuestions { get; } =
    [
        "Can I read all texts properly?",
        "Will I like and understand this UI or layout as a new user?",
        "Is there any too large component, gap, or visual disruption?",
        "Do we use proper shared components instead of ad-hoc markup?",
        "Do we use available space properly?",
        "Can the page be understood by scanning headings only?",
        "Is the hierarchy clear without decorative styling?",
        "Do focus, hover, disabled, loading, and empty states read clearly?",
        "On mobile, does the first viewport orient the user quickly?",
        "On desktop, are we avoiding dead horizontal space and accidental narrow columns?"
    ];

    /// <summary>
    /// Named component sections rendered within a catalog page, keyed by route. Both the page
    /// itself (for anchors/headings/API types) and the sidebar sub-nav read from this list so
    /// they can't drift out of sync.
    /// </summary>
    public static IReadOnlyDictionary<string, IReadOnlyList<SandboxPageSection>> PageSections { get; } =
        new Dictionary<string, IReadOnlyList<SandboxPageSection>>(StringComparer.OrdinalIgnoreCase)
        {
            ["/typography"] =
            [
                new("typeblock", "TextBlock", typeof(TextBlock)),
                new("eyebrow", "Eyebrow", typeof(Eyebrow)),
                new("small-text", "SmallText", typeof(SmallText)),
                new("copyable-mono-value", "CopyableMonoValue", typeof(CopyableMonoValue)),
                new("footer-text", "FooterText", typeof(FooterText)),
                new("hash-display", "HashDisplay", typeof(HashDisplay)),
                new("header", "Header", typeof(Header)),
                new("mono-text", "MonoText", typeof(MonoText)),
                new("muted-inline", "MutedInline", typeof(MutedInline)),
                new("section-head", "SectionHead", typeof(SectionHead)),
                new("section-heading", "SectionHeading", typeof(SectionHeading)),
                new("divider", "Divider", typeof(Divider))
            ],
            ["/identity"] =
            [
                new("icon", "Icon", typeof(Icon)),
                new("avatar", "Avatar", typeof(Avatar)),
                new("creator-line", "CreatorLine", typeof(CreatorLine)),
                new("creator-social-link", "CreatorSocialLink", typeof(CreatorSocialLink)),
                new("homo-avatar", "HomoAvatar", typeof(HomoAvatar)),
                new("robo-avatar", "RoboAvatar", typeof(RoboAvatar))
            ],
            ["/buttons"] =
            [
                new("button", "Button", typeof(Button)),
                new("copy-button", "CopyButton", typeof(CopyButton))
            ],
            ["/badges"] =
            [
                new("badge", "Badge", typeof(Badge)),
                new("badges-group", "BadgesGroup", typeof(BadgesGroup)),
                new("status-badge", "StatusBadge", typeof(StatusBadge)),
                new("chip", "Chip", typeof(Chip)),
                new("chip-row", "ChipRow", typeof(ChipRow)),
                new("compact-stat", "CompactStat", typeof(CompactStat)),
                new("compact-stat-strip", "CompactStatStrip", typeof(CompactStatStrip)),
                new("pill", "Pill", typeof(Pill)),
                new("pill-list", "PillList", typeof(PillList))
            ],
            ["/forms"] =
            [
                new("checkbox", "CheckBox", typeof(CheckBox<>)),
                new("dropdown", "DropDown", typeof(DropDown<>)),
                new("editable", "Editable", typeof(Editable<>)),
                new("entity-picker", "EntityPicker", typeof(EntityPicker)),
                new("fieldset", "Fieldset", typeof(Fieldset)),
                new("file-upload", "FileUpload", typeof(FileUpload)),
                new("form-field", "FormField", typeof(FormField)),
                new("form-row", "FormRow", typeof(FormRow)),
                new("form-section", "FormSection", typeof(FormSection)),
                new("form-stack", "FormStack", typeof(FormStack)),
                new("inline-actions", "InlineActions", typeof(InlineActions)),
                new("numeric", "Numeric", typeof(Numeric<>)),
                new("password", "Password", typeof(Password)),
                new("prefixed-field", "PrefixedField", typeof(PrefixedField)),
                new("secret-field", "SecretField", typeof(SecretField)),
                new("settings-switch-label", "SettingsSwitchLabel", typeof(SettingsSwitchLabel)),
                new("settings-switch-row", "SettingsSwitchRow", typeof(SettingsSwitchRow)),
                new("slider", "Slider", typeof(Slider<>)),
                new("switch", "Switch", typeof(Switch)),
                new("tag-editor", "TagEditor", typeof(TagEditor)),
                new("text-area", "TextArea", typeof(TextArea)),
                new("textbox", "TextBox", typeof(TextBox))
            ],
            ["/cards"] =
            [
                new("card", "Card", typeof(Card)),
                new("card-actions", "CardActions", typeof(CardActions)),
                new("card-button", "CardButton", typeof(CardButton)),
                new("card-grid", "CardGrid", typeof(CardGrid)),
                new("card-stats-with-number", "CardStatsWithNumber", typeof(CardStatsWithNumber)),
                new("action-card", "ActionCard", typeof(ActionCard)),
                new("action-review-panel", "ActionReviewPanel", typeof(ActionReviewPanel)),
                new("auth-card", "AuthCard", typeof(AuthCard)),
                new("hero-card", "HeroCard", typeof(HeroCard)),
                new("metric-card", "MetricCard", typeof(MetricCard)),
                new("panel-card", "PanelCard", typeof(PanelCard)),
                new("parity-section-card", "ParitySectionCard", typeof(ParitySectionCard)),
                new("price-bar", "PriceBar", typeof(PriceBar)),
                new("price-row", "PriceRow", typeof(PriceRow)),
                new("section-card", "SectionCard", typeof(SectionCard)),
                new("stat-box", "StatBox", typeof(StatBox)),
                new("stats-card-row", "StatsCardRow", typeof(StatsCardRow)),
                new("stats-grid", "StatsGrid", typeof(StatsGrid)),
                new("summary-tile", "SummaryTile", typeof(SummaryTile)),
                new("summary-tiles", "SummaryTiles", typeof(SummaryTiles)),
                new("surface-card", "SurfaceCard", typeof(SurfaceCard))
            ],
            ["/lists"] =
            [
                new("fact-table", "FactTable", typeof(FactTable)),
                new("list-detail-shell", "ListDetailShell", typeof(ListDetailShell)),
                new("list-group", "ListGroup", typeof(ListGroup)),
                new("list-item", "ListItem", typeof(ListItem)),
                new("list-panel-header", "ListPanelHeader", typeof(ListPanelHeader)),
                new("meta-list", "MetaList", typeof(MetaList)),
                new("plain-list", "PlainList", typeof(PlainList)),
                new("selection-list-item", "SelectionListItem", typeof(SelectionListItem))
            ],
            ["/navigation"] =
            [
                new("context-menu", "ContextMenu", typeof(ContextMenu)),
                new("filter-bar", "FilterBar", typeof(FilterBar)),
                new("legal-toc", "LegalToc", typeof(LegalToc)),
                new("legal-toc-nav", "LegalTocNav", typeof(LegalTocNav)),
                new("page-header", "PageHeader", typeof(PageHeader)),
                new("page-header-action-button", "PageHeaderActionButton", typeof(PageHeaderActionButton)),
                new("ribbon-tabs", "RibbonTabs", typeof(RibbonTabs<string>)),
                new("secondary-tabs", "SecondaryTabs", typeof(SecondaryTabs)),
                new("side-menu", "SideMenu", typeof(SideMenu)),
                new("side-menu-item", "SideMenuItem", typeof(SideMenuItem)),
                new("steps", "Steps", typeof(Steps)),
                new("steps-item", "StepsItem", typeof(StepsItem)),
                new("tabs", "Tabs", typeof(Tabs)),
                new("tabs-item", "TabsItem", typeof(TabsItem)),
                new("toolbar", "Toolbar", typeof(Toolbar)),
                new("toolbar-actions", "ToolbarActions", typeof(ToolbarActions)),
                new("toolbar-fields", "ToolbarFields", typeof(ToolbarFields)),
                new("toolbar-row", "ToolbarRow", typeof(ToolbarRow)),
                new("tree-view", "TreeView", typeof(TreeView)),
                new("tree-view-node-row", "TreeViewNodeRow", typeof(TreeViewNodeRow))
            ],
            ["/layout"] =
            [
                new("grid", "Grid", typeof(Grid)),
                new("page-scaffold", "PageScaffold", typeof(PageScaffold)),
                new("row-column", "Row", typeof(Row)),
                new("stack", "Stack", typeof(Stack)),
                new("sticky-action-footer", "StickyActionFooter", typeof(StickyActionFooter)),
                new("zoom-pan-frame", "ZoomPanFrame", typeof(ZoomPanFrame)),
                new("body", "Body", typeof(Body)),
                new("sidebar", "Sidebar", typeof(Sidebar)),
                new("page-shell", "PageShell", typeof(PageShell)),
                new("layout", "Layout", typeof(CanDoItAll.Components.BaseLib.Layout)),
                new("cluster", "Cluster", typeof(Cluster)),
                new("split", "Split", typeof(Split)),
                new("theme-host", "ThemeHost", typeof(ThemeHost)),
                new("workspace-panel", "WorkspacePanel", typeof(WorkspacePanel)),
                new("workspace-split", "WorkspaceSplit", typeof(WorkspaceSplit))
            ],
            ["/feedback"] =
            [
                new("alert", "Alert", typeof(Alert)),
                new("callout", "Callout", typeof(Callout)),
                new("empty-state", "EmptyState", typeof(EmptyState)),
                new("help-popover", "HelpPopover", typeof(HelpPopover)),
                new("loading-state", "LoadingState", typeof(LoadingState)),
                new("notification", "Notification", typeof(Notification)),
                new("status-check-list", "StatusCheckList", typeof(StatusCheckList)),
                new("tooltip", "Tooltip", typeof(Tooltip)),
                new("tooltip-target", "TooltipTarget", typeof(TooltipTarget)),
                new("verification-list", "VerificationList", typeof(VerificationList))
            ],
            ["/modals"] =
            [
                new("dialog", "Dialog", typeof(Dialog)),
                new("dialog-scaffold", "DialogScaffold", typeof(DialogScaffold)),
                new("danger-action-dialog", "DangerActionDialog", typeof(DangerActionDialog)),
                new("inspector-dialog-layout", "InspectorDialogLayout", typeof(InspectorDialogLayout)),
                new("picker-dialog-shell", "PickerDialogShell", typeof(PickerDialogShell)),
                new("dialog-host", "DialogHost", typeof(DialogHost))
            ],
            ["/data-display"] =
            [
                new("diff-viewer", "DiffViewer", typeof(DiffViewer)),
                new("timeline", "Timeline", typeof(Timeline)),
                new("timeline-stepper", "TimelineStepper", typeof(TimelineStepper))
            ],
            ["/storage"] =
            [
                new("storage-badge-strip", "StorageBadgeStrip", typeof(StorageBadgeStrip)),
                new("storage-summary-card", "StorageSummaryCard", typeof(StorageSummaryCard))
            ],
            ["/data-visualization"] =
            [
                new("data-grid", "DataGrid", typeof(DataGrid<>)),
                new("data-grid-column", "DataGridColumn", typeof(DataGridColumn<>)),
                new("chart", "Chart", typeof(Chart)),
                new("line-series", "LineSeries", typeof(LineSeries)),
                new("value-axis", "ValueAxis", typeof(ValueAxis)),
                new("category-axis", "CategoryAxis", typeof(CategoryAxis)),
                new("grid-lines", "GridLines", typeof(GridLines)),
                new("progress-bar", "ProgressBar", typeof(ProgressBar))
            ],
            ["/charts"] =
            [
                new("cda-chart", "CdaChart", typeof(CdaChart))
            ],
            ["/mermaid"] =
            [
                new("mermaid-diagram", "MermaidDiagram", typeof(MermaidDiagram))
            ],
            ["/qr"] =
            [
                new("qr-code-view", "QrCodeView", typeof(QrCodeView)),
                new("qr-code-button", "QrCodeButton", typeof(QrCodeButton)),
                new("qr-scan-button", "QrScanButton", typeof(QrScanButton))
            ],
            ["/gantt"] =
            [
                new("gantt-chart", "GanttChart", typeof(GanttChart))
            ],
            ["/overlays"] =
            [
                new("dialog-service", "DialogService", typeof(DialogService)),
                new("tooltip-service", "TooltipService", typeof(TooltipService)),
                new("notification-service", "NotificationService", typeof(NotificationService)),
                new("overlay-window", "OverlayWindow", typeof(OverlayWindow))
            ],
            ["/canvas"] =
            [
                new("canvas-workbench", "CanvasWorkbench", typeof(CanvasWorkbench)),
                new("canvas-calendar", "CanvasCalendar", typeof(CanvasCalendar)),
                new("calendar-selection-panel", "CalendarSelectionPanel", typeof(CalendarSelectionPanel)),
                new("calendar-export-menu", "CalendarExportMenu", typeof(CalendarExportMenu)),
                new("calendar-mini-month-navigator", "CalendarMiniMonthNavigator", typeof(CalendarMiniMonthNavigator)),
                new("calendar-time-grid-renderer", "CalendarTimeGridRenderer", typeof(CalendarTimeGridRenderer)),
                new("preview-assets", "CanvasSceneHostPreview", typeof(CanvasSceneHostPreview))
            ],
            ["/benchmark"] =
            [
                new("fruit-aggregation", "Fruit aggregation", typeof(Benchmark))
            ],
            ["/transitions"] =
            [
                new("expand-transition", "ExpandTransition", typeof(ExpandTransition))
            ]
        };

    public static IReadOnlyList<SandboxPageSection> GetSections(string route)
        => PageSections.TryGetValue(route, out var sections) ? sections : [];

    public static bool IsUnused(string componentName)
        => SandboxUnusedComponents.Names.Contains(componentName);

    public static bool IsGroupUnused(string route)
    {
        var sections = GetSections(route);
        return sections.Count > 0 && sections.All(section => IsUnused(section.ComponentName));
    }

    public static IReadOnlyList<SandboxPageSection> FilterVisible(IReadOnlyList<SandboxPageSection> sections, bool showUnused)
        => showUnused ? sections : sections.Where(section => !IsUnused(section.ComponentName)).ToList();

    public static int UniqueComponentCount { get; } =
        PageSections.Values
            .SelectMany(sections => sections)
            .Select(section => section.ComponentType)
            .Distinct()
            .Count();

    public static int VisibleComponentCount(bool showUnused)
        => PageSections.Values
            .SelectMany(sections => sections)
            .Where(section => showUnused || !IsUnused(section.ComponentName))
            .Select(section => section.ComponentType)
            .Distinct()
            .Count();

    public static SandboxGroupDefinition GetGroup(SandboxGroupKey key)
        => Groups.First(group => group.Key == key);

    public static IReadOnlyList<SandboxExampleDefinition> GetExamples(SandboxGroupKey key)
        => Examples.Where(example => example.GroupKey == key).ToArray();

    private static SandboxExampleDefinition CreateExample(
        string id,
        SandboxGroupKey groupKey,
        string title,
        string summary,
        IReadOnlyList<string> tags,
        params string[] componentNames)
    {
        var group = GetGroup(groupKey);
        var route = group.Route;

        return new SandboxExampleDefinition(
            id,
            groupKey,
            title,
            route,
            summary,
            tags,
            componentNames);
    }

    private static SandboxExampleDefinition CreateCustomExample(
        string id,
        SandboxGroupKey groupKey,
        string title,
        string route,
        string summary,
        IReadOnlyList<string> tags,
        params string[] componentNames)
    {
        return new SandboxExampleDefinition(
            id,
            groupKey,
            title,
            route,
            summary,
            tags,
            componentNames);
    }
}

public static class SandboxFramePresetExtensions
{
    public static SandboxFramePreset Parse(string? value)
    {
        return value?.Trim().ToLowerInvariant() switch
        {
            "tablet" => SandboxFramePreset.Tablet,
            "mobile" => SandboxFramePreset.Mobile,
            _ => SandboxFramePreset.LiveViewport
        };
    }

    public static string ToSlug(this SandboxFramePreset value)
    {
        return value switch
        {
            SandboxFramePreset.Tablet => "tablet",
            SandboxFramePreset.Mobile => "mobile",
            _ => "live"
        };
    }

    public static string ToLabel(this SandboxFramePreset value)
    {
        return value switch
        {
            SandboxFramePreset.Tablet => "Tablet",
            SandboxFramePreset.Mobile => "Mobile",
            _ => "Live"
        };
    }

    public static string ToSurfaceClass(this SandboxFramePreset value)
    {
        return value switch
        {
            SandboxFramePreset.Tablet => "sandbox-demo-frame--tablet",
            SandboxFramePreset.Mobile => "sandbox-demo-frame--mobile",
            _ => "w-full"
        };
    }
}
