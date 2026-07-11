using CanDoItAll.Components.BaseLib;

namespace CanDoItAll.Components.FileBrowser.Sandbox;

internal sealed class SandboxThemeState
{
    public event Action? Changed;

    public string ThemeKey { get; private set; } = CadThemes.Light;

    public bool IsDark => ThemeKey == CadThemes.Dark;

    public void Toggle()
    {
        ThemeKey = IsDark ? CadThemes.Light : CadThemes.Dark;
        Changed?.Invoke();
    }
}

