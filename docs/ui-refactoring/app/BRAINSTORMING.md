# UI/UX Refactoring — Brainstorming

Working notes on the state of the application UI and where it should go. Still
brainstorming: nothing here is a committed decision.

Every file reference links to the source; every symptom links to the localhost route
(`http://localhost:5032`) where you can see it. Counts were measured on the current
working tree.

---

<a id="target"></a>

## 1. Target

A web/desktop hybrid: dense, quiet, keyboard-friendly, with list–detail as the default
page shape and colour reserved for signals that mean something.

Worth naming what that reference is actually doing, because most of it is subtractive:

- **One border per boundary.** Panes are separated by a single hairline. Nothing is a
  rounded card inside a rounded card.
- **Rows, not cards.** List items are ~32px flat rows with a hover tint and a selection
  tint. No per-item border, no per-item shadow, no per-item radius.
- **Three type sizes visible.** Title, body, meta. Everything else is weight and colour.
- **Colour appears about six times on the whole screen**, and every instance is a status.
  The chrome is greyscale.
- **The detail pane owns its own scroll**, and the properties rail is a third column, not
  a stack of boxes.
- **Density comes from removing padding, not from shrinking type.**

That last point matters: "more dense" and "wide margins" are not in conflict if you
delete the intermediate containers rather than compressing text.

---

<a id="where"></a>

## 2. Where the UI lives

The UI spans two codebases, and both are in scope.

**Application** — this repo. Pages, module components, the shell, [`Tailwind/`](../../Tailwind),
[`app.css`](../../src/App/CanDoItAll.Web/wwwroot/app.css), routing.
258 `.razor` files under [`src/`](../../src) backing **28 routable URLs** — a ~9:1 ratio.

**Component library** — [`CanDoItAll.Components`](../../../CanDoItAll.Components), consumed
as NuGet `0.1.18` and pinned in [`Directory.Build.props`](../../Directory.Build.props). It
owns `PageScaffold`, `PageHeader`, `SecondaryTabs`, `Tabs`, `SurfaceCard`, `Dialog`,
`Button`, `Stack`, `ListDetailShell`, and **all 97 `--cad-*` design tokens** in
[`foundation/theme.css`](../../../CanDoItAll.Components/Tailwind/foundation/theme.css).

A large share of the problems below can only be fixed on the library side, and they are
mutually entangled with the app-side changes — you cannot fix the page without fixing the
primitive it sits in. **A publish-and-bump cycle per iteration is not workable.** Either
fold the library into the monorepo, or wire a live `ProjectReference` to the local clone
behind a build flag. This should be settled before any pixel work starts, because it
determines whether the loop is minutes or hours.

Also note the two repos currently disagree about scope:
[`ui-support-scope.md`](../ui-support-scope.md) here says large-desktop only, while
[`standard-components-tailwind-policy.md`](../../../CanDoItAll.Components/docs/standard-components-tailwind-policy.md)
imposes a Visual Gate requiring narrow-mobile screenshot proof for every styling change.
See [§6](#responsive).

---

<a id="causes"></a>

## 3. Root causes

<a id="nesting"></a>

### 3.1 Over-nesting is structural, not sloppiness

Look at [`/processes`](http://localhost:5032/processes) ([screenshot](./screenshots/processes.png)).
It *is* a list–detail layout — it uses the real primitive — and it still reads as a pile of
randomly placed cards. Counting inward on the detail side: page frame → header card →
`PageScaffold` → `SurfaceCard` → `ListDetailShell` pane → the definition-editor card →
field group. Seven nested rounded containers, five of them with their own visible border.

A large part of that is baked into the library primitive itself.
[`ListDetailShell.razor`](../../../CanDoItAll.Components/src/CanDoItAll.Components.BaseLib/Components/Lists/ListDetailShell.razor)
renders each pane as:

```
rounded-[1.75rem] border border-slate-200 bg-white/90 shadow-sm shadow-slate-200/60 min-h-[26rem]
```

with a default gap of `LayoutGap.XLarge`. So a pane is a fat, shadowed, 28px-radius card
before any content exists. Put two of them side by side inside another card and the
nesting is unavoidable at the call site. The reference in [§1](#target) uses **zero**
radius and **zero** shadow for the same job.

Same file, `ResolveHeaderClass()` and `ResolveContentClass()`, hardcode `px-4 py-3.5` /
`px-4 py-4` — which is why padding never lines up with anything token-driven.

**Where to fix:** library. A flat/hairline variant, radius and shadow off by default,
padding from `--cad-space-*`.

<a id="listdetail"></a>

### 3.2 The list–detail primitive is not actually a list–detail primitive

It's a two-column grid. It has no notion of:

- **Collapsing** — the list pane cannot be hidden to give the detail full width.
- **Resizing** — the divider is not draggable. `ListPaneWidth` is a fixed parameter passed
  from the page; [`/processes`](http://localhost:5032/processes) hardcodes `28rem`.
  There is no docked splitter anywhere in the library:
  [`Split.razor`](../../../CanDoItAll.Components/src/CanDoItAll.Components.BaseLib/Components/Layout/Split.razor)
  is a plain flex row and `WorkspaceSplit.razor` is a bare `<div>`. The only resize
  behaviour that ships is on *floating* windows
  ([`OverlayWindow.razor`](../../../CanDoItAll.Components/src/CanDoItAll.Components.OverlayLib/Components/Core/OverlayWindow.razor),
  `AllowResize`).
- **Responsive collapse** — it does carry one container query at `46rem` in
  [`list-detail.css`](../../../CanDoItAll.Components/Tailwind/data-display/list-detail.css),
  which stacks the panes vertically. Stacking is the wrong behaviour for list–detail; the
  correct narrow behaviour is list-*or*-detail with a back affordance.
- **Selection semantics** — no keyboard navigation, no roving focus, no
  `aria-selected`. Arrow-key movement through a list is table stakes for this shape.

It's used in 14 files / 30 references — [`/processes`](http://localhost:5032/processes),
[`/collaboration`](http://localhost:5032/collaboration),
[`/resources`](http://localhost:5032/resources),
[`/plugins`](http://localhost:5032/plugins),
[`/settings`](http://localhost:5032/settings),
[`/test-lab`](http://localhost:5032/test-lab),
[`/agents`](http://localhost:5032/agents) (capabilities / governance / providers tabs),
[`/projects`](http://localhost:5032/projects) — and absent from the pages that most need
it: [`/crm-hr/directory`](http://localhost:5032/crm-hr/directory),
[`/prompt-gallery`](http://localhost:5032/prompt-gallery),
[`/crm-hr/crm`](http://localhost:5032/crm-hr/crm).

So this is simultaneously the most-used layout primitive and the least-finished one.
Getting it right is probably the single highest-leverage change available.

<a id="color"></a>

### 3.3 Colour: the abstraction exists, the code goes around it

The semantic layer is complete and shipped, in
[`foundation/theme.css`](../../../CanDoItAll.Components/Tailwind/foundation/theme.css):

```
--cad-tone-{primary,secondary,success,warning,danger,info,base,dark,light}-soft-{bg,border,fg,hover}
--cad-color-{page-bg,surface,surface-soft,border,border-soft,text,text-strong,text-muted,text-soft,inverse}
--cad-space-{compact-gap,content-gap,section-gap,surface-padding,dialog-padding}
--cad-radius-{control,surface,panel}   --cad-shadow-{soft,strong}
```

The problem is bypass:

- **86 of 258** `.razor` files hardcode raw palette colours (`slate-200`, `sky-600`,
  `emerald-*`); 79 of those inside a literal `class="…"`.
- Three of the four files under [`Tailwind/`](../../Tailwind) `@apply` hardcoded palette
  utilities rather than tokens —
  [`surfaces/admin.css`](../../Tailwind/surfaces/admin.css),
  [`surfaces/overlays.css`](../../Tailwind/surfaces/overlays.css),
  [`navigation/workbench-shell.css`](../../Tailwind/navigation/workbench-shell.css).
  Only [`main/tunable-boundary.css`](../../Tailwind/main/tunable-boundary.css) consumes
  tokens correctly — it's the model.
- `ListDetailShell` itself hardcodes `slate`/`white`, as above.

Two consequences. First, the "everything is highlighted" feel: with no semantic
discipline, every author reaches for a colour. Second — and this is the practical lever —
**dark mode cannot work until this is fixed**. See [§4.1](#dark).

<a id="type"></a>

### 3.4 Typography: ~30 ways to render a string, no token layer

[`TextPrimitives.cs`](../../../CanDoItAll.Components/src/CanDoItAll.Components.BaseLib/Components/Typography/TextPrimitives.cs)
defines `enum TextStyle` with **17** members: `H1…H6, Subtitle1, Subtitle2, Body1, Body2,
Caption, Small, Note, Eyebrow, Mono, Footer, Muted`. On top of that,
[`Components/Typography/`](../../../CanDoItAll.Components/src/CanDoItAll.Components.BaseLib/Components/Typography)
ships **13 more** standalone components — `Eyebrow`, `SmallText`, `MonoText`,
`MutedInline`, `SectionHead`, `SectionHeading`, `Header`, `FooterText`,
`CopyableMonoValue`, `HashDisplay`, `Divider`, `TextBlock`.

There are **zero** `--cad-font-*` or `--cad-text-*` tokens. Sizes are raw Tailwind baked
into `.cda-text-block--*` classes in
[`typography/text.css`](../../../CanDoItAll.Components/Tailwind/typography/text.css) — and
some are duplicates: `Subtitle1` and `H5` are both `text-lg font-semibold`, `Note` /
`Muted` are identical.

A screen using 6 of those 30 looks busy for reasons no one can point at. Target is
something like 5 sizes × 3 weights, token-backed, with the old enum members kept as
aliases so the 472 `TextBlock` call sites don't all have to change at once.
The cost of that consolidation is discussed in [§10.5](#q-type).

<a id="hierarchy"></a>

### 3.5 Hierarchy inverted: chrome outweighs content

On [`/crm-hr`](http://localhost:5032/crm-hr) ([screenshot](./screenshots/crm-hr.png)) the
first 200px are: a tab strip of ~11 truncated pills, an eyebrow, a title, a two-line
paragraph, five count badges, a tab row, and only then content. On
[`/agents`](http://localhost:5032/agents) ([screenshot](./screenshots/agents.png)) it's
worse — six header badges *plus* an eight-tile metric strip *plus* nine sub-tabs above the
fold.

Contributing pieces, each fixable:

- **Count badges are tone-coloured even at zero.** `0 organizations` renders info-blue,
  `0 workforce` green. `CompactStat` has no muted-when-zero mode (library) and pages pass
  counts unconditionally (app).
- **Per-page hint paragraphs** are always expanded. `PageHeader.Description` is one
  change point (library) for making them collapsible.
- **Eyebrow / title / description / `<PageTitle>` / nav label are five independently
  authored strings per route** with nothing tying them together. That's the whole source
  of the copywriting drift: nav says "Agents", eyebrow says "AGENTFRAMEWORK", title says
  "Integrated technical agent runtime".

<a id="nav"></a>

### 3.6 Navigation: three overlapping systems, one of them invisible

<a id="nav-rail"></a>

**(a) The icon rail.**
[`ShellNavigation.cs`](../../src/App/CanDoItAll.Web/Composition/ShellNavigation.cs)
hardcodes 11 top-level items. A proper contributor mechanism exists in
[`SharedKernel/Navigation`](../../src/Foundation/CanDoItAll.SharedKernel/Navigation)
(`IShellNavigationContributor`) but **only 3 modules use it**, and one carries a
`DesignNote` admitting the workaround: *"Processes are shown beside Agents until nested
module navigation is introduced."* That is the direct cause of the
[`/processes`](http://localhost:5032/processes) vs
[`/processes/live`](http://localhost:5032/processes/live) confusion.

<a id="nav-tabstrip"></a>

**(b) The pinned/recent tab strip.**
[`AppTabStrip.razor`](../../src/UI/CanDoItAll.AppComponents/Components/AppTabStrip.razor)
(262 lines), visible on every route. It duplicates the rail, truncates labels to a few
characters, and reveals four buttons on hover so everything shifts. Behind it,
[`WorkbenchTabState.cs`](../../src/Modules/CanDoItAll.Modules.Workbench/Workbench/WorkbenchTabState.cs)
is **856 lines** — pin, sleep, close-others, close-right, reopen-recent, capacity
trimming, persisted per database profile with fingerprint fencing via
[`BrowserWorkspaceStateStore.cs`](../../src/App/CanDoItAll.Web/Infrastructure/BrowserWorkspaceStateStore.cs).
Replacing it with starring-into-nav is right, but it's a *replacement* — keep a slimmed
favourites store, drop the rest. Note Playwright covers `app-shell-workbar`.

<a id="nav-workspace"></a>

**(c) `ShellWorkspaceItem` — a third nav axis that never renders on desktop.**
It's a record declared in
[`ShellNavigationItem.cs:24`](../../src/Foundation/CanDoItAll.SharedKernel/Navigation/ShellNavigationItem.cs)
and populated with three hardcoded entries in
[`MainLayout.State.cs:12`](../../src/App/CanDoItAll.Web/Components/Layout/MainLayout.State.cs):

```csharp
new("delivery",   "Delivery Workspace", "Project authoring, structure, calendars, and prompt drafts.", "/projects"),
new("quality",    "Quality Desk",       "Test plans and evidence review.",                             "/test-lab"),
new("operations", "Operations Desk",    "Scheduler, runtime settings, and environment status.",        "/scheduler")
```

It is passed into `AppShell.Workspaces` and rendered in exactly one place —
[`AppShell.razor:251-271`](../../src/UI/CanDoItAll.AppComponents/Components/AppShell.razor),
inside `<details class="cda-shell-mobile-panel">`, the mobile navigation block. Since the
app is developed and validated at desktop width, **this UI is never seen**. There is no
desktop route where it appears; to view it you have to narrow the viewport below the
mobile breakpoint on any page, e.g. [`/`](http://localhost:5032/).

So: a "workspace" concept (Delivery / Quality / Operations) exists in the information
architecture, competing with both the rail and the tab strip, and is effectively dead
code. Either promote it to a real organising principle or delete it — but decide, because
it's currently a third answer to "where am I".

<a id="dialogs"></a>

### 3.7 Dialogs instead of routes

39 files host dialogs, and **all of them** use the declarative `<Dialog IsOpen="@someBool">`
form, so every dialog's state is a private bool on its parent. That's why aggregator files
exist at all: `ProjectStructureCanvasDialogs.razor` (1313 lines),
[`ProjectModalHost.razor`](../../src/Modules/CanDoItAll.Modules.Projects/Pages/Components/ProjectModalHost.razor)
(633 lines).

An imperative `DialogService` + `DialogHost` + `DialogScaffold` + `PickerDialogShell` +
`DangerActionDialog` + `InspectorDialogLayout` all ship from the library, and
`<DialogHost />` **is already mounted** at
[`MainLayout.razor:237`](../../src/App/CanDoItAll.Web/Components/Layout/MainLayout.razor).
No application code calls the service.

Separately, much of what is currently a dialog should be a route rendering into the detail
pane — record editors on [`/crm-hr/directory`](http://localhost:5032/crm-hr/directory),
[`/crm-hr/crm`](http://localhost:5032/crm-hr/crm),
[`/prompt-gallery`](http://localhost:5032/prompt-gallery).

Note the tension to resolve up front: "inline create form" and "bookmarkable `/new`" are
only compatible if `/{module}/new` is a **real route** that renders the create form into
the detail pane. Same component, two entry points.

<a id="subtabs"></a>

### 3.8 Sub-tabs: three state models, two primitives, none consistent

| Route | Mechanism | Bookmarkable |
|---|---|---|
| [`/agents`](http://localhost:5032/agents) | `[SupplyParameterFromQuery(Name="tab")]` + `AllowedTabs` whitelist + `NavigateTo(replace: true)` — [`AgentsHomePage.razor.cs:52`](../../src/Modules/CanDoItAll.Modules.AgentFramework/Pages/AgentsHomePage.razor.cs) | yes |
| [`/crm-hr`](http://localhost:5032/crm-hr) | each tab is its own route — [`CrmHrSecondaryTabs.razor`](../../src/Modules/CanDoItAll.Modules.CrmHr/Components/CrmHrSecondaryTabs.razor) | yes |
| [`/settings`](http://localhost:5032/settings) | local field — [`SettingsPage.razor`](../../src/Modules/CanDoItAll.Modules.Workspace/Pages/SettingsPage.razor) | **no** |
| [`/test-lab`](http://localhost:5032/test-lab) | local field — [`TestLabPage.razor`](../../src/Modules/CanDoItAll.Modules.TestLab/Pages/TestLabPage.razor) | **no** |
| [`/processes`](http://localhost:5032/processes) | index-based `<Tabs SelectedIndex>` in [`ProcessWorkspaceShell.razor`](../../src/Modules/CanDoItAll.Modules.Processes/Components/ProcessWorkspaceShell.razor) (twice — 8 detail tabs, 7 run-view tabs) | **no** |

Two different primitives (`SecondaryTabs`, key-based; `Tabs`, index-based) with different
visual output, which is why tab styling varies page to page. Which URL shape to
standardise on is discussed in [§8.5](#rollout).

<a id="discovery"></a>

### 3.9 Hard-to-discover routes

These are legitimate project-scoped pages — they don't belong in the global rail. The
problem is that the only way in is an unlabelled icon button with no tooltip, from
[`/projects`](http://localhost:5032/projects):

- [`/projects/{id}/structure`](http://localhost:5032/projects/3491f36f-cbf7-4efe-9e14-35db19eb5fb7/structure)
- [`/projects/{id}/structure?tab=gantt`](http://localhost:5032/projects/3491f36f-cbf7-4efe-9e14-35db19eb5fb7/structure?tab=gantt)
- [`/projects/{id}/processes`](http://localhost:5032/projects/3491f36f-cbf7-4efe-9e14-35db19eb5fb7/processes)
- [`/projects/{id}/calendar`](http://localhost:5032/projects/3491f36f-cbf7-4efe-9e14-35db19eb5fb7/calendar)

What they need is a **project context** that makes them obvious once you're inside a
project: a named sub-navigation on the project detail (Structure / Gantt / Processes /
Calendar), a breadcrumb that says which project you're in, and the same set reachable from
the project row's context menu. Labels and tooltips on the icons are the five-minute fix;
the real fix is that a project should feel like a place you enter, not a row with four
mystery buttons.

<a id="appcss"></a>

### 3.10 `app.css` is a list of missing library APIs

[`app.css`](../../src/App/CanDoItAll.Web/wwwroot/app.css) (363 lines) loads after both
generated stylesheets and is mostly `!important` fights with packaged components —
`z-index: 1900 !important`, `display: flex !important`, one selector repeated twice purely
for specificity. Each entry marks a place where a library component didn't expose the knob
the app needed. Read as a backlog, it should shrink to near zero.

Related: [`wwwroot/css/output.css`](../../src/App/CanDoItAll.Web/wwwroot/css/output.css)
(3600 lines) is generated from [`Tailwind/input.css`](../../Tailwind/input.css), committed,
and has **zero** build enforcement — no `tailwind` reference in any `.csproj`, `.targets`,
or workflow. Regeneration is manual. That's a silent-drift risk to close before a styling
push.

---

<a id="unused"></a>

## 4. Built but switched off

<a id="dark"></a>

### 4.1 Dark mode

The full dark palette is defined at `[data-cad-theme="dark"]` in
[`foundation/theme.css`](../../../CanDoItAll.Components/Tailwind/foundation/theme.css), and
a [`ThemeHost.razor`](../../../CanDoItAll.Components/src/CanDoItAll.Components.BaseLib/Components/Layout/ThemeHost.razor)
component exists whose entire job is emitting that attribute. The component sandbox has a
working light/dark toggle
([`SandboxThemeState.cs`](../../../CanDoItAll.Components/samples/CanDoItAll.Components.Sandbox/SandboxThemeState.cs)).

Usage in this application: `grep -r "data-cad-theme" src` → **0**. `ThemeHost` → **0**.

The wiring is trivial; the blocker is [§3.3](#color). The 86 files with hardcoded
`slate-*`/`sky-*` — and `ListDetailShell` itself — will simply stay light.
**De-hardcoding is the prerequisite for dark mode**, which is a far better justification
for that work than tidiness.

### 4.2 `DialogService`

Mounted, never called. See [§3.7](#dialogs).

### 4.3 The navigation contributor mechanism

Built, used by 3 modules out of ~15. See [§3.6a](#nav-rail).

---

<a id="tailwind"></a>

## 5. The Tailwind question

Tailwind **v4, CSS-first** already builds in both repos —
[`Tailwind/input.css`](../../Tailwind/input.css) here and
[`../../../CanDoItAll.Components/Tailwind/input.css`](../../../CanDoItAll.Components/Tailwind/input.css)
there, no JS config in either. There is no adopt-or-not decision left.

The owner's position — build the components *on* Tailwind, and use that as the argument
against a third-party library — is coherent and, given the code, already executed. The
`@apply`-into-semantic-classes approach in
[`typography/text.css`](../../../CanDoItAll.Components/Tailwind/typography/text.css) and
[`controls/buttons.css`](../../../CanDoItAll.Components/Tailwind/controls/buttons.css) is
the right way to use Tailwind in a component system, and
[`standard-components-tailwind-policy.md`](../../../CanDoItAll.Components/docs/standard-components-tailwind-policy.md)
writes the rule down explicitly.

So the concern isn't Tailwind. It's that the policy isn't enforced — [§3.3](#color).
Utility classes leak into 86 app files and into the primitives themselves, which is the
failure mode Tailwind-in-a-component-system is specifically prone to. The fix is a
lint/CI rule that rejects raw palette utilities outside the token layer, not a change of
technology.

---

<a id="responsive"></a>

## 6. Responsiveness

Current state is close to none. **21 of 258** `.razor` files use any responsive prefix
(`sm:`/`md:`/`lg:`/`xl:`), there are **zero** container queries in this repo's
[`Tailwind/`](../../Tailwind), and 33 hardcoded `min-w-[…]` values pin layouts open.
[`ui-support-scope.md`](../ui-support-scope.md) formalises this as large-desktop-only.

Proposed tiers:

- **Desktop** — primary. Everything works here.
- **Tablet (~768–1180px)** — must be genuinely usable. Concretely: rail collapses to
  icons, list–detail becomes list-*or*-detail with a back affordance (not stacked panes),
  the properties/third column folds into the detail flow, tab strips scroll rather than
  wrap.
- **Mobile** — needs an owner decision, independent of the below.

### 6.1 Desktop first, tablet later

**Pros**
- Fastest to a visible result; every decision has one right answer, so iteration is quick.
- The pilot pages settle layout and density without a second variable in play.
- Matches the existing [`ui-support-scope.md`](../ui-support-scope.md), so no doc conflict
  to resolve now.
- Lower risk of over-abstracting a component before its desktop shape is even proven.

**Cons**
- **Tablet is not a skin, it's a behaviour.** Collapse, resize, list-or-detail and
  back-navigation are *state and interaction*, not media queries. Adding them later means
  reopening `ListDetailShell`, the shell, and every page that hardcodes a pane width — a
  second pass over the same files.
- Fixed pixel assumptions accumulate fast. There are already 33 hardcoded `min-w-[…]`
  values and a `ListPaneWidth="28rem"` literal at the call site; each one added now is a
  later edit.
- The "back affordance" implies routes that can express *which pane is showing*. Retrofit
  it and you touch routing again after it has just been standardised.
- Risk of the deferral becoming permanent — which is what happened to the existing mobile
  code ([§3.6c](#nav-workspace)).

### 6.2 Desktop + tablet together

**Pros**
- The three capabilities tablet needs — collapsible pane, resizable divider,
  list-or-detail switch — are the **same** capabilities the desktop design wants anyway
  ([§3.2](#listdetail)). Building them once serves both.
- The work concentrates in one component rather than spreading across pages. If
  `ListDetailShell` handles it, most pages inherit tablet behaviour with no per-page code.
- Routes get designed once with pane state in mind.
- Rules out the whole class of "works at 1920, unusable at 1280" bugs that are currently
  invisible because nobody tests there.

**Cons**
- Slower to the first finished-looking page; two viewports to review on every iteration.
- Doubles the screenshot surface in the review loop (though [`tools/`](./tools) automates
  the capture).
- Some genuinely dense surfaces — Gantt, the workflow canvas, project structure — may not
  have a sensible tablet form at all, so the tier needs explicit per-page exemptions
  rather than a blanket promise.

**Reading:** the cons of doing them together are mostly *schedule*; the cons of deferring
are mostly *rework*, concentrated in the one component that everything else depends on.
Since `ListDetailShell` has to be reopened regardless ([§3.2](#listdetail)), building the
collapse/resize/switch behaviour in that same pass is close to free — and the alternative
is doing that pass twice. A reasonable middle: build the *capability* into the component
now, adopt it on desktop, and treat tablet as a validation pass on the pilot pages rather
than on all 28 routes at once.

### 6.3 Mobile

Two costs pull against each other: the library's Visual Gate already demands
narrow-mobile screenshot proof for every styling change, while this repo's scope doc
forbids adding mobile branches. Right now we pay for the first without benefiting from
it. Whichever way it goes, the two documents must agree.

The existing mobile code is not a starting point — it's the `<details>` panel in
[`AppShell.razor:251`](../../src/UI/CanDoItAll.AppComponents/Components/AppShell.razor)
that nobody has looked at ([§3.6c](#nav-workspace)).

---

<a id="library"></a>

## 7. The component library

An honest assessment, since it's the crux of the build-vs-buy argument.

**What's actually there.** More than expected: 10 packages, 236 components, a
[Sandbox](../../../CanDoItAll.Components/samples/CanDoItAll.Components.Sandbox) described
as a "living visual catalog" with 20 catalogue pages
([`Foundations`](../../../CanDoItAll.Components/samples/CanDoItAll.Components.Sandbox/Components/Pages/Foundations.razor),
`Layout`, `Navigation`, `DataDisplay`, `Feedback`, `Inputs`, `Overlays`, …), a
[coverage index](../../../CanDoItAll.Components/samples/CanDoItAll.Components.Sandbox/Components/Pages/Coverage.razor),
a light/dark toggle, unit tests for 6 of the packages, and a written Tailwind policy. So
"no preview, no docs" isn't quite right — the preview exists and runs with
`dotnet run --project samples/CanDoItAll.Components.Sandbox`.

**What's genuinely missing.** Per-component API documentation — there is no prop table,
no usage guidance, no do/don't. `docs/architecture/` contains exactly one file. The
Sandbox shows you *that* a component exists and roughly what it looks like; it doesn't
tell you which of `SectionHead` / `SectionHeading` / `Header` to use, or that
`ListDetailShell` won't collapse. That gap is why the app has 30 ways to render text and
86 files that gave up and wrote Tailwind directly.

### 7.1 Suggestion: publish the catalogue as a GitHub Page

The Sandbox is the library's best asset and it's currently invisible unless you clone the
repo and run it. Publishing it would change how it gets used: a URL you can link from a
review comment, a place to point at when arguing "use this, not raw Tailwind", and a
public artefact that raises the library's perceived quality without writing a single doc
page.

One real constraint: the Sandbox is **Blazor Server** —
[`Program.cs`](../../../CanDoItAll.Components/samples/CanDoItAll.Components.Sandbox/Program.cs)
calls `AddInteractiveServerComponents()` and the csproj is `Microsoft.NET.Sdk.Web`. GitHub
Pages is static hosting, so it can't run as-is. Options, cheapest first:

1. **Add a WASM host project** alongside the existing one, sharing the same catalogue
   pages and registry. Static-publish that to Pages. Most components are presentational,
   so this is mostly a hosting-model change, not a rewrite — but anything relying on
   server-side services would need stubbing.
2. **Prerender to static HTML** — publish the Server app, crawl the ~20 catalogue routes
   with the same puppeteer approach already used in [`tools/`](./tools), and ship the
   HTML. Loses interactivity (tabs, menus, the theme toggle), which is a real loss for a
   component catalogue.
3. **Screenshot gallery** — a generated page of captures per component group. Cheapest,
   least useful, but still better than nothing and it doubles as visual-regression
   evidence for the Visual Gate.

Option 1 is the one worth doing. There's already a
[`ci.yml`](../../../CanDoItAll.Components/.github/workflows/ci.yml) to hang a Pages job
off.

**Is it overkill to make the library good enough?** The alternative is worse. Call sites
in this app: `Button` 720, `Stack` 648, `TextBlock` 472, `SurfaceCard` 270, `TabsItem`
110, `PanelCard` 87, `Dialog` 52 — plus bUnit renders the real primitives across 164 test
files in [`tests/Components`](../../tests/Components). Replacing the library is a rewrite
of the entire UI, not a refactor, and it would also discard the Tailwind-native
architecture the owner deliberately chose.

The realistic read: the library's *foundations* are sound (tokens, theme, Tailwind policy,
sandbox) and its *component maturity* is uneven. The gap is concentrated in a handful of
high-traffic primitives — `ListDetailShell`, the typography set, `PageHeader`,
`CompactStat`, tabs. Fixing those five is a bounded piece of work with outsized effect.
Rewriting 236 components to a high polish is not, and isn't necessary.

---

<a id="direction"></a>

## 8. Direction

Rough ordering, with the dependency that matters called out first.

**0. Unblock the loop.** Monorepo or live project-linking to the library clone. Nothing
below is pleasant to iterate on without it.

**1. Foundations.**
- Token discipline: semantic aliases in this repo's Tailwind layer, de-hardcode the three
  offending files under [`Tailwind/`](../../Tailwind), then the app files. Add the CI rule.
- Typography: token-backed scale, ~5×3, old enum members aliased.
- Turn on `ThemeHost` + a theme toggle once the above lands.
- Enforce `output.css` regeneration in the build.

**2. Fix `ListDetailShell` properly** — flat by default, token padding, collapsible,
resizable divider, list-or-detail at tablet width, keyboard selection. This is the
component the whole target design rests on, and per [§6.2](#responsive) it's also where
tablet support is cheapest.

**3. Prove the pattern on two pages.** One simple —
[`/prompt-gallery`](http://localhost:5032/prompt-gallery), where the editor dialog becomes
a detail pane and `/prompt-gallery/{id}` and `/prompt-gallery/new` become real routes. One
complex — [`/crm-hr/directory`](http://localhost:5032/crm-hr/directory) (2106 lines), where
tabs already route correctly so the work is layout and dialog removal.

**4. Shell.** Split
[`AppShell.razor`](../../src/UI/CanDoItAll.AppComponents/Components/AppShell.razor) (869
lines: rail + overflow flyout + mobile panel + workbar in one file). Replace the tab strip
with starring-into-nav. Extract the 140-line database flyout from
[`MainLayout.razor`](../../src/App/CanDoItAll.Web/Components/Layout/MainLayout.razor).
Route remaining modules through `IShellNavigationContributor`. Give projects a real
context sub-navigation ([§3.9](#discovery)). Resolve `ShellWorkspaceItem`.

<a id="rollout"></a>

**5. Roll out.** Remaining pages, tab standardisation, `DialogService` migration,
copywriting pass.

On tab URLs specifically, there are two shapes in play and the current split is accidental
rather than principled:

- `/page?tab={tab}` — what [`/agents`](http://localhost:5032/agents) does.
- `/page/{tab}` — what [`/crm-hr`](http://localhost:5032/crm-hr) does, as separate routes.

`/page/{tab}` is the cleaner default: it reads as a place rather than a setting, it keeps
the query string free for things that genuinely are parameters (filters, search, selected
record), it gives each tab its own `<PageTitle>` and nav-matching behaviour for free, and
it composes with the detail route — `/crm-hr/directory/{id}` is obvious in a way that
`/crm-hr?tab=directory&id=…` is not. Blazor supports it directly with multiple `@page`
directives or a `{tab}` route parameter, so it costs no more than the query-string version.

Reserve `?tab=` for genuine sub-views of an already-selected record, where the tab is a
lens on the thing in the URL rather than a destination — e.g.
`/projects/{id}/structure?tab=gantt` is arguably correct as-is.

The monoliths go last, once the pattern is settled: `ProcessWorkspaceShell` 4053,
`LiveProcessesDashboard` 3152, `ProjectStructurePage` 3052, `SchedulerPlannerPage` 2492,
`CrmHrDirectoryPage` 2106, `CrmHrCrmPage` 1951.

---

<a id="constraints"></a>

## 9. Constraints any proposal must respect

- **`data-testid` is load-bearing.** 43 Playwright files in
  [`tests/Playwright`](../../tests/Playwright) (notably `AppShellLayoutTests.cs`) and 164
  bUnit files in [`tests/Components`](../../tests/Components) key off `shell-nav-*`,
  `app-shell-workbar`, `crmhr-home-*`, `agents-shell-*`. Removing the workbar means
  updating both suites in the same change.
- **Blazor Server / InteractiveServer, globally**, with a guard test. No WASM, no Auto —
  which is also why interaction latency deserves attention in any keyboard-navigation work.
- **A screenshot harness already exists** — [`tools/`](./tools) runs puppeteer-core and
  regenerates the page/dialog/screenshot tables in [`README.md`](./README.md). Baseline
  before, diff after. This is what makes pixel iteration viable despite Blazor.
- **The library's Visual Gate** requires desktop + narrow-mobile screenshot proof for
  styling changes there ([§6.3](#responsive)).

---

<a id="questions"></a>

## 10. Open questions

1. **Monorepo, or live project-link to the clone?** Blocking — see [§2](#where).
2. **Tablet now or later?** [§6.1](#responsive) vs [§6.2](#responsive).
3. **Mobile: in or out?** And whichever way, align
   [`ui-support-scope.md`](../ui-support-scope.md) with the library's Visual Gate.
4. **`ShellWorkspaceItem` — promote or delete?** A third navigation axis that currently
   renders only on mobile ([§3.6c](#nav-workspace)).

<a id="q-type"></a>

5. **How aggressive on typography, and who absorbs the cost?**

   Collapsing 17 `TextStyle` members to ~5 is the same shape of problem as collapsing a
   17-value status enum down to 5 in a database migration. Some mappings are mechanical —
   `H1` → `Display`, `Body1` → `Body`, and the outright duplicates (`Note` and `Muted`
   render identically today, so they merge with zero visual change). Those you can script.

   The rest need a human to look at the rendered page. `Subtitle2` could legitimately
   become `BodyStrong` or `Label` depending on what the author meant, and **the compiler
   cannot help you** — both compile, both render, one looks wrong. It's the same as
   migrating `OrderStatus.PendingReview` when the new enum has both `Pending` and
   `InReview`: valid either way, only the business context decides.

   Scale: 472 `TextBlock` call sites. The clean-mapping majority is scriptable; the
   judgement cases are the ones where the current design is genuinely inconsistent — call
   it 15–20% needing a look, concentrated in the busiest pages.

   Keeping the old members as aliases makes the migration incremental and non-breaking:
   nothing has to change on day one, and pages get converted as they're touched. The cost
   of that safety is that the inconsistency survives in un-touched pages, possibly
   indefinitely. Reasonable compromise: alias everything, convert the pilot pages
   immediately, and mark the aliases `[Obsolete]` with a warning so the remaining call
   sites are visible in the build output rather than forgotten.

6. **Density reference.** Picking concrete numbers up front settles a lot of arguments:
   the reference in [§1](#target) is roughly a 4px base grid, 32px list rows, 13–14px body.
7. **Is per-component API documentation worth writing**, or is the Sandbox — published per
   [§7.1](#library) — plus a short "which component when" decision guide enough?
