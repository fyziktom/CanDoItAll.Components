# TreeView adaptive row contract

Status: Accepted (2026-07-19)

## Context

`TreeViewNodeRow` owns the visual and interaction contract for hierarchical BaseLib navigation. Browser list defaults were adding an implicit indentation system beside the component's explicit level padding. At constrained widths, long non-shrinking badges then collapsed wrapping labels to one character per line, while row details were exposed only through a native `title` attribute.

## Responsibility decision

| Current owner | Responsibility | Target owner | Test seam |
|---|---|---|---|
| `TreeViewNodeRow` | Render a node's full label, badge, accessible name, and detail affordance | Remains in `TreeViewNodeRow` | Formatter-focused component tests and browser geometry proof |
| `treeview.css` | Normalize hierarchy spacing and adapt row content to inline space | Remains in BaseLib TreeView styles | Small, medium, and large sandbox viewports |
| Product pages | Supply domain-specific detail text through `TreeViewNode.Tooltip` | Remains with each consumer | Consumer component tests |

No new project, public abstraction, factory, or JavaScript measurement service is justified. The existing `TreeViewNode` contract and `TooltipTarget` service adapter already express the required boundary. Dependency direction remains UI component to BaseLib feedback component, with no new project reference and no partial class.

## Decision

- Reset list margin, padding, and markers so explicit `Level` padding is the only hierarchy indentation.
- Keep labels on one line, scale them from 14px to a 12px minimum using the row's inline-size container, then ellipsize overflow.
- Render a full badge at roomy widths and the first three grapheme clusters plus `...` in constrained rows; keep the full label and badge in the tree item's accessible name.
- Route an authoritative `TreeViewNode.Tooltip` through `TooltipTarget` with `TooltipPosition.Right`; when it is absent, synthesize a full label-and-badge fallback. Expose supplied domain details as the tree item's accessible description for `aria-activedescendant` navigation.
- Preserve the existing public API and keyboard/tree semantics.

## Rejected options

- Product-local CSS: repeats the defect and lets consumers diverge.
- `ResizeObserver`/JavaScript measurement: adds lifecycle and interop complexity for behavior supported by CSS container units and queries.
- A new public adaptive-label component: exposes TreeView-specific badge and tooltip policy as an unrelated abstraction without a second proven consumer contract.

## Acceptance proof

- Unit and rendered component tests cover grapheme-safe compact badges, tooltip fallback/authority, accessible text, right-side TooltipService wiring, and tooltip lifecycle.
- The sandbox long-text scenario contains an expanded three-level row with a long label and badge.
- Browser checks at small, medium, and large component viewports prove one-line text, a 12px minimum, ellipsis under pressure, compact/full badge switching, full nested width, right-side service tooltip, and no horizontal overflow.
- Existing TreeView keyboard tests and simulator component regressions remain green.
