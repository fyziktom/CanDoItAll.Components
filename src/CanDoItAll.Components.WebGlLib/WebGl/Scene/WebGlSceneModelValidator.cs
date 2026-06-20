namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlSceneModelValidator
{
    public WebGlSceneDocumentValidationResult Validate(WebGlSceneModel scene)
    {
        ArgumentNullException.ThrowIfNull(scene);
        var result = new WebGlSceneDocumentValidationResult();
        if (string.IsNullOrWhiteSpace(scene.SceneId))
        {
            result.Errors.Add("Scene id is required.");
        }

        WebGlSceneDocumentValidator.ValidateScene(scene, result);
        return result;
    }
}
