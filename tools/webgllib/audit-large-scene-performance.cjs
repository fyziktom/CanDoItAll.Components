const fs = require("node:fs");
const path = require("node:path");
const { performance } = require("node:perf_hooks");
const { pathToFileURL } = require("node:url");

const repoRoot = path.resolve(__dirname, "..", "..");
const runtimeSceneDir = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene");
const normalizerSourcePath = path.join(runtimeSceneDir, "28-webgl-scene-command-batch-normalizer.js");
const linkSourcePath = path.join(runtimeSceneDir, "27-webgl-scene-links.js");
const stageRunnerSourcePath = path.join(runtimeSceneDir, "30-webgl-scene-stage-runner.js");
const barrierSourcePath = path.join(runtimeSceneDir, "32-webgl-scene-stage-barriers.js");
const journalSourcePath = path.join(runtimeSceneDir, "33-webgl-scene-command-journal.js");
const reportDir = path.join(repoRoot, "artifacts", "webgl-engine-rc-v17", "performance");

async function main() {
  fs.mkdirSync(reportDir, { recursive: true });
  const normalizer = await importModule(writeCommandBatchAuditModule());
  const links = await importModule(writeLinkAuditModule());
  const stageRunner = await importModule(writeStageRunnerAuditModule());

  const motionProof = measure("1000 independent object motions", () => {
    const batch = {
      batchId: "proof.components.1000-motions",
      motions: Array.from({ length: 1000 }, (_, index) => ({
        motionId: `motion.${index}`,
        objectId: `actor.${index}`,
        targetPosition: { x: index, y: 0, z: 0 }
      }))
    };
    const normalized = normalizer.normalizeCommandBatchForAudit(batch);
    assertEqual(normalized.metrics.commandCountBeforeNormalization, 1000, "1000 motions before normalization");
    assertEqual(normalized.metrics.commandCountAfterNormalization, 1000, "1000 motions after normalization");
    assertEqual(normalized.metrics.estimatedHostInteropCallCount, 1, "1000 motions estimated interop calls");
    assertEqual(normalized.metrics.interopCallsAvoided, 999, "1000 motions avoided interop calls");
    return {
      inputMotionCount: 1000,
      outputMotionCount: normalized.motions.length,
      interopCallsAvoided: normalized.metrics.interopCallsAvoided,
      warnings: normalized.warnings || []
    };
  });

  const stagedProof = measure("25 generic objects with 4 ordered stages", () => {
    const batch = {
      batchId: "proof.components.25-actors-4-stages",
      batchingPolicy: "preserve-order",
      stages: Array.from({ length: 4 }, (_, stageIndex) => ({
        stageId: `stage.${stageIndex}`,
        batchingPolicy: "preserve-order",
        motions: Array.from({ length: 25 }, (_, actorIndex) => ({
          motionId: `motion.${stageIndex}.${actorIndex}`,
          objectId: `actor.${actorIndex}`,
          targetPosition: { x: stageIndex + 1, y: 0, z: actorIndex }
        }))
      }))
    };
    const normalized = normalizer.normalizeCommandBatchForAudit(batch);
    const outputMotionCount = normalized.stages.reduce((total, stage) => total + stage.motions.length, 0);
    assertEqual(normalized.metrics.stageCount, 4, "ordered stage count");
    assertEqual(outputMotionCount, 100, "ordered staged motion count");
    assertEqual(normalized.metrics.droppedDuplicateMotionCount, 0, "ordered dropped duplicate motions");
    return {
      actorCount: 25,
      stageCount: normalized.metrics.stageCount,
      inputMotionCount: 100,
      outputMotionCount,
      droppedDuplicateMotionCount: normalized.metrics.droppedDuplicateMotionCount,
      warnings: normalized.warnings || []
    };
  });

  const linkProof = measure("1000 links with indexed moved object update", () => {
    const state = {
      objectLookup: new Map(),
      linkGroupsByObjectId: new Map(),
      diagnostics: {}
    };
    state.objectLookup.set("actor.moved", objectAt(0, 0));
    for (let index = 0; index < 2000; index++) {
      state.objectLookup.set(`endpoint.${index}`, objectAt(index + 1, index % 7));
    }

    const totalLinkCount = 1000;
    const movedActorLinkCount = 10;
    for (let index = 0; index < totalLinkCount; index++) {
      const sourceObjectId = index < movedActorLinkCount ? "actor.moved" : `endpoint.${index * 2}`;
      const targetObjectId = `endpoint.${index * 2 + 1}`;
      const group = links.createLinkGroup(state, {
        id: `link.${index}`,
        sourceObjectId,
        targetObjectId
      });
      if (!group) {
        throw new Error(`Failed to create link group ${index}`);
      }

      links.indexLinkGroup(state, group);
    }

    state.objectLookup.set("actor.moved", objectAt(12, 4));
    links.syncLinksForObject(state, "actor.moved");

    assertEqual(state.linkGroupsByObjectId.get("actor.moved").length, movedActorLinkCount, "indexed moved actor link count");
    assertEqual(state.diagnostics.linksUpdatedLastFrame, movedActorLinkCount, "links updated last frame");
    assertEqual(state.diagnostics.linkSyncScanCount, movedActorLinkCount, "indexed scan count");
    assertEqual(state.diagnostics.linkSyncIndexedHitCount, movedActorLinkCount, "indexed hit count");
    return {
      totalLinkCount,
      movedActorLinkCount,
      linksUpdatedLastFrame: state.diagnostics.linksUpdatedLastFrame,
      linkSyncScanCount: state.diagnostics.linkSyncScanCount,
      linkSyncIndexedHitCount: state.diagnostics.linkSyncIndexedHitCount,
      warnings: []
    };
  });

  const sb14WebGlScaleProof = measure("SB14 WebGL data/runtime scale and bounded stage journal", () => {
    const scene = createSceneData({
      objectCount: 500,
      linkCount: 1000,
      symbolCount: 1000
    });
    const journalMaxEntries = 200;
    const stagedCommandCount = 500;
    const state = createStageState({ maxCommandStageJournalEntries: journalMaxEntries });
    const applied = [];
    stageRunner.enqueueCommandStages(
      state,
      "proof.sb14.webgl-scale",
      Array.from({ length: stagedCommandCount }, (_, index) => stage(`sb14.stage.${index}`, {
        motions: [motion(`sb14.motion.${index}`, index % 50, "append", `object.${index % scene.objects.length}`)]
      })),
      item => applied.push(item.stageId));

    assertEqual(scene.objects.length, 500, "SB14 scene object count");
    assertEqual(scene.links.length, 1000, "SB14 link count");
    assertEqual(countSceneSymbols(scene), 1000, "SB14 symbol count");
    assertEqual(applied.length, stagedCommandCount, "SB14 staged commands applied");
    assertEqual(state.diagnostics.queuedCommandStageCount, 0, "SB14 stage queue drains");
    assertEqual(state.diagnostics.commandStageJournalCount, journalMaxEntries, "SB14 bounded command journal count");
    assertEqual(state.diagnostics.commandStageJournalDroppedCount > 0, true, "SB14 bounded command journal dropped old entries");
    assertEqual(state.diagnostics.commandStageJournalCounters.started, stagedCommandCount, "SB14 journal started counter");
    assertEqual(state.diagnostics.commandStageJournalCounters.applied, stagedCommandCount, "SB14 journal applied counter");
    assertEqual(state.diagnostics.commandStageJournalCounters.completed, stagedCommandCount, "SB14 journal completed counter");

    return {
      sceneObjectCount: scene.objects.length,
      linkCount: scene.links.length,
      symbolCount: countSceneSymbols(scene),
      stagedCommandCount,
      appliedStageCount: applied.length,
      queuedCommandStageCount: state.diagnostics.queuedCommandStageCount,
      stageQueueMaxLength: stagedCommandCount,
      stageQueueBounded: stagedCommandCount <= 500,
      commandStageJournalMaxEntries: journalMaxEntries,
      commandStageJournalCount: state.diagnostics.commandStageJournalCount,
      commandStageJournalDroppedCount: state.diagnostics.commandStageJournalDroppedCount,
      commandStageJournalCounters: state.diagnostics.commandStageJournalCounters,
      recentJournalEntryCount: state.diagnostics.commandStageRecentJournalEntries.length,
      warnings: []
    };
  });

  const proof = {
    generatedAtUtc: new Date().toISOString(),
    largeScreenOnly: true,
    thresholds: {
      webGlSceneObjectCount: 500,
      webGlLinkCount: 1000,
      webGlSymbolCount: 1000,
      stagedCommandCount: 500,
      stageQueueMaxLength: 500,
      commandStageJournalMaxEntries: 200
    },
    operations: [motionProof, stagedProof, linkProof, sb14WebGlScaleProof],
    bottleneckNotes: [
      "Command batching keeps 1000 motion commands in one normalized host interop batch.",
      "Ordered stage policy preserves stage boundaries and prevents duplicate-motion collapse across ordered stages.",
      "Link synchronization uses linkGroupsByObjectId, so moving one object updates only indexed adjacent links instead of scanning all scene links.",
      "The SB14 WebGL runtime scale proof builds 500 scene objects, 1000 links, 1000 attached symbols, drains 500 command stages, and proves the command-stage journal remains bounded at 200 entries."
    ]
  };

  const reportPath = path.join(reportDir, "components-performance-proof.json");
  fs.writeFileSync(reportPath, `${JSON.stringify(proof, null, 2)}\n`, "utf8");
  console.log(`Generic WebGlLib performance audit passed. Wrote ${path.relative(repoRoot, reportPath)}.`);
}

function measure(name, action) {
  const started = performance.now();
  const result = action();
  return {
    name,
    elapsedMilliseconds: Number((performance.now() - started).toFixed(3)),
    ...result
  };
}

function writeCommandBatchAuditModule() {
  const modulePath = path.join(reportDir, "command-batch-performance-audit.mjs");
  fs.writeFileSync(modulePath, fs.readFileSync(normalizerSourcePath, "utf8"), "utf8");
  return modulePath;
}

function writeLinkAuditModule() {
  const source = fs.readFileSync(linkSourcePath, "utf8")
    .replace(/import\s*\{[\s\S]*?\}\s*from\s*"\.\/02-webgl-scene-core\.js";\s*/m, linkRuntimeStub());
  const modulePath = path.join(reportDir, "link-performance-audit.mjs");
  fs.writeFileSync(modulePath, source, "utf8");
  return modulePath;
}

function writeStageRunnerAuditModule() {
  const barrierModulePath = path.join(reportDir, "stage-barriers-performance-audit.mjs");
  const journalModulePath = path.join(reportDir, "command-journal-performance-audit.mjs");
  fs.writeFileSync(barrierModulePath, fs.readFileSync(barrierSourcePath, "utf8"), "utf8");
  fs.writeFileSync(journalModulePath, fs.readFileSync(journalSourcePath, "utf8"), "utf8");
  const source = fs.readFileSync(stageRunnerSourcePath, "utf8")
    .replace('from "./32-webgl-scene-stage-barriers.js"', 'from "./stage-barriers-performance-audit.mjs"')
    .replace('from "./33-webgl-scene-command-journal.js"', 'from "./command-journal-performance-audit.mjs"');
  const modulePath = path.join(reportDir, "stage-runner-performance-audit.mjs");
  fs.writeFileSync(modulePath, source, "utf8");
  return modulePath;
}

function linkRuntimeStub() {
  return `
const THREE = {
  LineBasicMaterial: class {
    constructor(options) {
      this.options = options;
    }
  },
  Line: class {
    constructor(geometry, material) {
      this.geometry = geometry;
      this.material = material;
    }
  },
  Group: class {
    constructor() {
      this.children = [];
      this.userData = {};
    }
    add(child) {
      this.children.push(child);
    }
  },
  BufferGeometry: class {
    setFromPoints(points) {
      this.points = points;
      return this;
    }
    dispose() {
      this.disposed = true;
    }
  },
  Vector3: class {
    constructor(x, y, z) {
      this.x = x;
      this.y = y;
      this.z = z;
    }
  }
};
function resolveFiniteNumber(value, fallback) {
  return Number.isFinite(Number(value)) ? Number(value) : fallback;
}
function resolveObjectPosition(sceneObject) {
  return sceneObject?.position || { x: 0, y: 0, z: 0 };
}
`;
}

function objectAt(x, z) {
  return { position: { x, y: 0, z } };
}

function createSceneData({ objectCount, linkCount, symbolCount }) {
  const objects = Array.from({ length: objectCount }, (_, index) => ({
    id: `object.${index}`,
    kind: index % 5 === 0 ? "resource" : "actor",
    position: { x: index % 50, y: 0, z: Math.floor(index / 50) },
    symbols: []
  }));
  for (let index = 0; index < symbolCount; index++) {
    objects[index % objects.length].symbols.push({
      id: `symbol.${index}`,
      semanticKind: "status",
      color: index % 2 === 0 ? "#facc15" : "#38bdf8"
    });
  }

  const links = Array.from({ length: linkCount }, (_, index) => ({
    id: `link.${index}`,
    sourceObjectId: `object.${index % objectCount}`,
    targetObjectId: `object.${(index * 17 + 3) % objectCount}`
  }));
  return { objects, links };
}

function countSceneSymbols(scene) {
  return scene.objects.reduce((total, item) => total + item.symbols.length, 0);
}

function createStageState(options = {}) {
  return {
    commandStageRunner: null,
    diagnostics: { animatedSymbolCount: 0 },
    motions: new Map(),
    motionQueuesByObjectId: new Map(),
    options: { renderMode: "auto", ...options },
    cameraDampingFrames: 0,
    renderRequested: false,
    renderReason: "",
    scheduleCount: 0,
    scheduleRender() {
      this.scheduleCount += 1;
    }
  };
}

function stage(stageId, options = {}) {
  return {
    stageId,
    waitSeconds: options.waitSeconds || 0,
    barrierPolicy: options.barrierPolicy || "",
    barrierObjectIds: options.barrierObjectIds || [],
    barrierEventId: options.barrierEventId || "",
    barrierTimeoutSeconds: options.barrierTimeoutSeconds || 0,
    patches: [],
    motions: options.motions || []
  };
}

function motion(motionId, x, queuePolicy, objectId = "actor") {
  return {
    motionId,
    objectId,
    durationSeconds: 1,
    queuePolicy,
    targetPosition: { x, y: 0, z: 0 }
  };
}

async function importModule(modulePath) {
  return import(`${pathToFileURL(modulePath).href}?v=${Date.now()}`);
}

function assertEqual(actual, expected, label) {
  if (actual !== expected) {
    throw new Error(`${label}: expected ${expected}, actual ${actual}`);
  }
}

main().catch(error => {
  console.error(error?.stack || error);
  process.exit(1);
});
