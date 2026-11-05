using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.BaseLib;

public sealed class NativeInputEventArgs : EventArgs {
    public string Value { get; set; } = string.Empty;
    public bool BadInput { get; set; }
}

[EventHandler("oncadnativeinput", typeof(NativeInputEventArgs))]
public static class EventHandlers;
