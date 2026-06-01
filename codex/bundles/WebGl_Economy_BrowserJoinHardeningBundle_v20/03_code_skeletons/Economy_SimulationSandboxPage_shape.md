# Economy desktop simulation sandbox page shape

```razor
@page "/economy/simulation-sandbox"

<EconomySimulationSandboxToolbar
    Session="@Session"
    OnLoad="@Load"
    OnStep="@Step"
    OnPause="@Pause"
    OnSnapshot="@Snapshot"
    OnAnalyze="@Analyze" />

<WebGlSceneView
    @ref="_sceneView"
    SceneDocument="@Session.RunDocument.InitialScene" />

<EconomySimulationDiagnosticsPanel
    Diagnostics="@Session.Diagnostics"
    CurrentAnalysis="@Session.CurrentAnalysis" />
```

Keep this page in the Economy repository. Do not add Economy references to Components.
