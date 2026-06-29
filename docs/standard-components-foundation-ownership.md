# Standard Components Foundation Ownership

This note records the foundation boundary used by the publishing-readiness refactor.

## Common

`CanDoItAll.Components.Common` owns framework-neutral helpers and primitives that can be used by every package without referencing Blazor component libraries:

- `CssClassBuilder` joins CSS class and style fragments.
- `ComponentAttributes` merges captured attributes with base class and style values.
- `Orientation`, `AlignItems`, `JustifyContent`, and `FlexWrap` describe generic layout primitives.

These helpers must not depend on BaseLib, AppComponents, sandbox projects, WebGL, Canvas, or app services.

## BaseLib

`CanDoItAll.Components.BaseLib` owns standard Blazor component contracts:

- `StyledComponentBase` exposes `Class`, `Style`, `AdditionalAttributes`, and delegates merge behavior to `ComponentAttributes`.
- `ComponentAttributeExtensions` remains as a compatibility extension wrapper for existing components and consumers. It must not contain duplicate merge logic.
- Component-specific primitives stay near their component group until a wider public API migration proves a better home. Current examples include button, form, badge, feedback, tabs, tree view, and layout-domain enums.

## AppComponents Duplicate Exception

The legacy `C:\repositories\CanDoItAll\src\CanDoItAll.AppComponents\Primitives\ComponentPrimitives.cs` file still duplicates several standard primitives and a full attribute merge helper:

- duplicated Common primitives: `Orientation`, `AlignItems`, `JustifyContent`, `FlexWrap`
- duplicated BaseLib primitives: `ButtonStyle`, `ButtonSize`, `Variant`, `Shade`
- duplicated helper: `ComponentAttributeExtensions`

SB03 does not delete or rewrite this external package source because AppComponents migration needs consumer checks and compatibility proof. SB04 owns the migration audit and removal plan for these duplicates.

## Contract Proof

The helper boundary is guarded by:

- `tests/CanDoItAll.Components.Common.Tests`
- `tests/CanDoItAll.Components.BaseLib.Tests`

The tests pin the class/style merge order, empty attribute cleanup, null result behavior, `CssClassBuilder` trimming behavior, the BaseLib compatibility extension, and `StyledComponentBase` merge output.
