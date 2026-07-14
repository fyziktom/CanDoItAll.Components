# Interactive Gantt component boundary

Status: Accepted for implementation, 2026-07-14

## Decision

Create `CanDoItAll.Components.Gantt` as a focused reusable Razor component package. It owns generic Gantt contracts, dependency scheduling, canvas rendering, pointer interactions, accessibility projection, and PNG export. It references `CanDoItAll.Components.CanvasLib` and no CanDoItAll product/domain project.

The package exposes a controlled component. Inputs are immutable task/dependency/assignment models. Date, duration, title, placement, insertion, and dependency edits are emitted as strongly typed mutation requests. The component never persists or silently accepts a mutation on behalf of its host.

CanvasLib gains only a low-level public canvas-surface seam for device-pixel-ratio sizing, invalidation, hit regions, pointer capture, and image export. Existing `CanvasWorkbench` behavior and contracts remain unchanged.

## Responsibility inventory

| Responsibility | Owner before | Owner after |
|---|---|---|
| Generic Gantt contracts | None | `CanDoItAll.Components.Gantt` |
| Dependency validation and schedule propagation | Product-specific Mermaid projection only | `CanDoItAll.Components.Gantt` pure scheduling types |
| Gantt drawing and hit testing | None | Gantt runtime using CanvasLib surface primitives |
| Gantt mutation persistence | Not applicable to shared components | Consuming application |
| Sandbox behavior proof | None | `CanDoItAll.Components.Sandbox` `/groups/gantt` |
| Product-structure mapping | Existing Workbench projection layer | Workbench Gantt adapter, outside this repository |

## Dependency direction

```text
Sandbox -> Gantt -> CanvasLib -> BaseLib/Common/OverlayLib
Gantt.Tests -> Gantt

Product Workbench -> packaged Gantt
Gantt -/-> Product Workbench, EF, HTTP, or persistence
```

Contracts and scheduling code remain free of product models, databases, browser globals, and external SDKs. The sandbox and product host own model adaptation and mutation acceptance.

## Pattern selection record

- Observed force: user gestures must cross a reusable UI boundary without changing the host's source of truth.
- Rejected simpler option: two-way bound mutable task lists would make rejected or failed persistence indistinguishable from accepted state.
- Selected pattern: command-as-data through immutable, strongly typed mutation requests. The host accepts, rejects, or transforms each request and supplies a new model.
- Rejected reuse: `CanvasWorkbench` is card-graph specific; its fixed node geometry and X/Y move event cannot express task-width resize or link endpoint reconnection without breaking existing consumers.
- No renderer interface is introduced. There is one canvas renderer and no current alternate implementation requiring an abstraction.
- Unit-test seam: scheduling and mutation planning are pure .NET code. Browser behavior is tested through the sandbox.

## Visual and interaction thesis

- Visual thesis: a dense engineering timeline with a quiet neutral grid, saturated task bars, cyan dependency handles, and one restrained critical-path accent.
- Content plan: compact toolbar, aligned optional task columns, one shared scroll surface, timeline, and transient assignment detail.
- Interaction thesis: direct manipulation of bar bodies and endpoints; empty-space dragging pans horizontal overflow; typed time-scale presets change timeline density; dependency endpoints reconnect in place; assignment indicators expand only while hovered or focused.

## Acceptance criteria

- Multiple dependencies are rendered and validated without magic string identifiers.
- Every dependency segment avoids unrelated task rectangles and terminates directly at its successor bar edge.
- The timeline exposes typed `0.25 h`, `1 h`, `1 d`, and `1 w` scales plus horizontal scrollbar and empty-space panning.
- Dragging either task edge emits normalized start/end/duration changes.
- Dragging a dependency endpoint can add or reconnect a dependency and cyclic results are rejected predictably.
- Insertion produces an explicit change set that rewires the selected chain and propagates dependent dates.
- The task table and canvas rows share one row-height contract and stay aligned while scrolling.
- Assignment badges remain compact and reveal typed process, workflow, agent, or person details on hover/focus.
- PNG export includes the visible task columns, timeline, bars, and dependencies.
- Unit tests cover DAG ordering, multiple prerequisites, cycle rejection, resize propagation, and insertion rewiring.
- Sandbox browser proof covers both requested desktop viewports and records no console errors.

## Risks and mitigations

- Large schedules can make per-pointer Blazor interop too slow. Pointer preview and hit testing stay in JavaScript; only committed gestures cross into .NET.
- Product nodes can be unscheduled. Projection-only dates are visually distinguished and are persisted only after an explicit accepted mutation.
- An additive CanvasLib seam could accidentally alter asset order. Asset verification and existing CanvasLib tests must pass unchanged.
- Reusing package version `0.1.2` can resolve stale NuGet cache entries. Consumer validation uses a unique prerelease package version.
