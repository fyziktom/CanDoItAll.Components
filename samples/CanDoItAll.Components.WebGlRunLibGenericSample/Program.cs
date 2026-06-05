using CanDoItAll.Components.WebGlLib;
using CanDoItAll.Components.WebGlRunLib;

WebGlRunDocument document = CreateDocument();
WebGlRunDocumentValidationResult validation = new WebGlRunDocumentValidator().Validate(document);
if (!validation.IsValid)
{
    Console.Error.WriteLine(string.Join(Environment.NewLine, validation.Errors));
    return 1;
}

Console.WriteLine($"Generic WebGlRunLib sample valid: {document.RunId.Value}");
Console.WriteLine($"Frames: {document.Timeline.Frames.Count}");
return 0;

static WebGlRunDocument CreateDocument()
{
    WebGlSceneDocument sceneDocument = new()
    {
        DocumentId = "scene.generic-route-demo",
        Scene = new()
        {
            SceneId = "scene.generic-route",
            Title = "Generic route",
            Objects =
            {
                new()
                {
                    Id = "node.origin",
                    Kind = "node",
                    Family = "generic",
                    Title = "Origin",
                    Position = new WebGlVector3(0, 0, 0),
                    Color = "#60a5fa"
                },
                new()
                {
                    Id = "node.target",
                    Kind = "node",
                    Family = "generic",
                    Title = "Target",
                    Position = new WebGlVector3(4, 0, 0),
                    Color = "#34d399"
                }
            }
        },
        Source = "generic-sample"
    };

    sceneDocument = WebGlSceneDocumentSerializer.Deserialize(WebGlSceneDocumentSerializer.Serialize(sceneDocument));

    return new()
    {
        RunId = new("run.generic-route"),
        InitialScene = sceneDocument,
        Timeline = new()
        {
            FrameRate = 30,
            Frames =
            {
                new()
                {
                    Index = 0,
                    TimeSeconds = 0
                },
                new()
                {
                    Index = 1,
                    TimeSeconds = 1,
                    Motions =
                    {
                        new()
                        {
                            MotionId = "motion.origin-to-target",
                            ObjectId = "node.origin",
                            TargetPosition = new WebGlVector3(4, 0, 0),
                            DurationSeconds = 0.25
                        }
                    }
                }
            }
        },
        Metadata =
        {
            ["sample.kind"] = "generic"
        }
    };
}
