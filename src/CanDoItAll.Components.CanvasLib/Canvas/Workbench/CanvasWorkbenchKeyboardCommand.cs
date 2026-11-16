using System.Text.Json.Serialization;

namespace CanDoItAll.Components.CanvasLib;

[JsonConverter(typeof(JsonStringEnumConverter<CanvasWorkbenchKeyboardCommand>))]
public enum CanvasWorkbenchKeyboardCommand {
    None,
    CopyIdentifier
}
