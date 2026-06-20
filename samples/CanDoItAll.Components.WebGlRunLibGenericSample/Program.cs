using CanDoItAll.Components.WebGlLib;
using CanDoItAll.Components.WebGlRunLib;

WebGlRunDocument[] documents =
[
    CreateGenericRouteDocument(),
    CreateProductionLineCanaryDocument()
];

var validator = new WebGlRunDocumentValidator();
foreach (WebGlRunDocument document in documents)
{
    WebGlRunDocumentValidationResult validation = validator.Validate(document);
    if (!validation.IsValid)
    {
        Console.Error.WriteLine($"Sample validation failed for {document.RunId.Value}:");
        Console.Error.WriteLine(string.Join(Environment.NewLine, validation.Errors));
        return 1;
    }

    Console.WriteLine($"Generic WebGlRunLib sample valid: {document.RunId.Value}");
    Console.WriteLine($"Objects: {document.InitialScene.Scene.Objects.Count}");
    Console.WriteLine($"Links: {document.InitialScene.Scene.Links.Count}");
    Console.WriteLine($"Frames: {document.Timeline.Frames.Count}");
}

return 0;

static WebGlRunDocument CreateGenericRouteDocument()
{
    WebGlSceneDocument sceneDocument = new()
    {
        DocumentId = "scene.generic-route-demo",
        Source = "generic-sample",
        Scene = new()
        {
            SceneId = "scene.generic-route",
            Title = "Generic route",
            Revision = 1,
            Interaction = new()
            {
                AllowHover = true,
                AllowClickSelection = true,
                AllowDragOnGroundPlane = true,
                DragSnapGridSize = 0.25,
                DragBounds = new()
                {
                    MinX = -2,
                    MaxX = 10,
                    MinZ = -2,
                    MaxZ = 4
                }
            },
            Objects =
            {
                Node("node.origin", "Origin", 0, 0, "#60a5fa"),
                Node("node.buffer.a", "Buffer A", 2, 0, "#a78bfa"),
                Node("node.buffer.b", "Buffer B", 4, 0, "#f59e0b"),
                Node("node.target", "Target", 6, 0, "#34d399"),
                Token("token.route.001", 0, 0.8, "#f97316"),
                Token("token.route.002", 2, 0.8, "#22c55e")
            },
            Links =
            {
                Link("link.origin.buffer-a", "node.origin", "node.buffer.a"),
                Link("link.buffer-a.buffer-b", "node.buffer.a", "node.buffer.b"),
                Link("link.buffer-b.target", "node.buffer.b", "node.target")
            },
            Layers =
            {
                new()
                {
                    Id = "layer.route-nodes",
                    Title = "Route nodes",
                    ObjectIds = { "node.origin", "node.buffer.a", "node.buffer.b", "node.target" }
                },
                new()
                {
                    Id = "layer.route-tokens",
                    Title = "Route tokens",
                    ObjectIds = { "token.route.001", "token.route.002" }
                }
            }
        }
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
                    Stages =
                    {
                        MoveStage(
                            "stage.route.move",
                            "token.route.001",
                            new WebGlVector3(6, 0, 0.8),
                            "motion.token-route-001.to-target")
                    }
                },
                new()
                {
                    Index = 2,
                    TimeSeconds = 2,
                    ScenePatches =
                    {
                        SymbolPatch(
                            "patch.route-target-ready",
                            "scene.generic-route",
                            "node.target",
                            "symbol.node.target.ready",
                            "ready",
                            "#22c55e")
                    }
                }
            }
        },
        Metadata =
        {
            ["sample.kind"] = "generic",
            ["interaction.proof"] = "hover-click-drag-enabled"
        }
    };
}

static WebGlRunDocument CreateProductionLineCanaryDocument()
{
    WebGlSceneDocument sceneDocument = new()
    {
        DocumentId = "scene.production-line-canary",
        Source = "sample-only-canary",
        Metadata =
        {
            ["domainVocabularyLocation"] = "sample-only"
        },
        Scene = new()
        {
            SceneId = "scene.production-line-canary",
            Title = "Production line canary",
            Revision = 1,
            Interaction = new()
            {
                AllowHover = true,
                AllowClickSelection = true,
                AllowDragOnGroundPlane = true,
                FocusOnDoubleClick = true
            },
            Objects =
            {
                Station("station.intake", "Intake", 0, "#38bdf8"),
                Buffer("buffer.queue", "Queue", 2, 3),
                Station("station.inspect", "Inspection", 4, "#a78bfa"),
                Station("station.pack", "Pack", 6, "#22c55e"),
                Token("wip.token.001", 2, 0.8, "#f59e0b"),
                Token("wip.token.002", 2.4, 0.8, "#f97316")
            },
            Links =
            {
                Link("conveyor.intake.queue", "station.intake", "buffer.queue", "conveyor"),
                Link("conveyor.queue.inspect", "buffer.queue", "station.inspect", "conveyor"),
                Link("conveyor.inspect.pack", "station.inspect", "station.pack", "conveyor")
            },
            Layers =
            {
                new()
                {
                    Id = "layer.production-line-stations",
                    Title = "Stations",
                    ObjectIds = { "station.intake", "station.inspect", "station.pack" }
                },
                new()
                {
                    Id = "layer.production-line-flow",
                    Title = "Buffers and WIP",
                    ObjectIds = { "buffer.queue", "wip.token.001", "wip.token.002" }
                }
            }
        }
    };

    return new()
    {
        RunId = new("run.production-line-canary"),
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
                    Stages =
                    {
                        MoveStage(
                            "stage.wip-to-inspection",
                            "wip.token.001",
                            new WebGlVector3(4, 0, 0.8),
                            "motion.wip-token-001.to-inspection")
                    }
                },
                new()
                {
                    Index = 2,
                    TimeSeconds = 2,
                    Stages =
                    {
                        MoveStage(
                            "stage.wip-to-pack",
                            "wip.token.001",
                            new WebGlVector3(6, 0, 0.8),
                            "motion.wip-token-001.to-pack",
                            SymbolPatch(
                                "patch.inspect-alarm",
                                "scene.production-line-canary",
                                "station.inspect",
                                "symbol.station.inspect.quality-alert",
                                "quality-alert",
                                "#ef4444"))
                    }
                }
            }
        },
        Metadata =
        {
            ["sample.kind"] = "production-line-canary",
            ["domainVocabularyLocation"] = "sample-only"
        }
    };
}

static WebGlSceneObject Node(string id, string title, double x, double z, string color)
    => new()
    {
        Id = id,
        Kind = "node",
        Family = "generic-node",
        Title = title,
        Position = new WebGlVector3(x, 0, z),
        Color = color,
        IsSelectable = true,
        IsDraggable = true
    };

static WebGlSceneObject Station(string id, string title, double x, string color)
    => new()
    {
        Id = id,
        Kind = "station",
        Family = "canary-node",
        Title = title,
        Position = new WebGlVector3(x, 0, 0),
        Color = color,
        IsSelectable = true,
        IsDraggable = true,
        Symbols =
        {
            new()
            {
                Id = $"symbol.{id}.status",
                SemanticKind = "status",
                Color = "#22c55e",
                Intensity = 0.7
            }
        }
    };

static WebGlSceneObject Buffer(string id, string title, double x, int queueLength)
    => new()
    {
        Id = id,
        Kind = "buffer",
        Family = "canary-group",
        Title = title,
        Position = new WebGlVector3(x, 0, 0),
        Color = "#facc15",
        IsSelectable = true,
        Metadata =
        {
            ["queueLength"] = queueLength.ToString(System.Globalization.CultureInfo.InvariantCulture)
        }
    };

static WebGlSceneObject Token(string id, double x, double z, string color)
    => new()
    {
        Id = id,
        Kind = "token",
        Family = "generic-token",
        Title = id,
        Position = new WebGlVector3(x, 0, z),
        Size = new WebGlVector3(0.4, 0.4, 0.4),
        Color = color,
        IsSelectable = true,
        IsDraggable = true
    };

static WebGlSceneLink Link(string id, string sourceObjectId, string targetObjectId, string kind = "directed-flow")
    => new()
    {
        Id = id,
        SourceObjectId = sourceObjectId,
        TargetObjectId = targetObjectId,
        Kind = kind,
        IsDirectional = true,
        Color = "#94a3b8",
        Width = 1.5
    };

static WebGlRunActionStage MoveStage(
    string stageId,
    string objectId,
    WebGlVector3 targetPosition,
    string motionId,
    WebGlRunFramePatch? scenePatch = null)
{
    WebGlRunActionStage stage = new()
    {
        StageId = stageId,
        StageIndex = 0,
        OrderIndex = 0,
        ExecutionPolicy = WebGlRunStageExecutionPolicies.PreserveOrder,
        BarrierPolicy = WebGlSceneStageBarrierPolicies.WaitForObjectMotions,
        BarrierObjectIds = { objectId },
        Motions =
        {
            new()
            {
                MotionId = motionId,
                ObjectId = objectId,
                TargetPosition = targetPosition,
                DurationSeconds = 0.25,
                QueueMode = WebGlMotionQueueModes.Append,
                Metadata =
                {
                    ["interaction"] = "selectable-hoverable-token"
                }
            }
        }
    };

    if (scenePatch is not null)
    {
        stage.ScenePatches.Add(scenePatch);
    }

    return stage;
}

static WebGlRunFramePatch SymbolPatch(
    string patchId,
    string sceneId,
    string objectId,
    string symbolId,
    string semanticKind,
    string color)
    => new()
    {
        Id = patchId,
        Patch = new()
        {
            SceneId = sceneId,
            ObjectPatches =
            {
                new()
                {
                    ObjectId = objectId,
                    Symbols =
                    [
                        new()
                        {
                            Id = symbolId,
                            SemanticKind = semanticKind,
                            Color = color,
                            Intensity = 1,
                            EffectKey = WebGlSymbolEffects.Pulse
                        }
                    ]
                }
            }
        }
    };
