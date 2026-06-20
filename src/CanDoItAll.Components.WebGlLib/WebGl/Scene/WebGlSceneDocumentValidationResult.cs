namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlSceneDocumentValidationResult
{
    public List<string> Errors { get; set; } = [];

    public List<string> Warnings { get; set; } = [];

    public bool IsValid => Errors.Count == 0;
}
