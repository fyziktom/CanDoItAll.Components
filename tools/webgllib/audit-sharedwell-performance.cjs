const fs = require("node:fs");
const path = require("node:path");
const { performance } = require("node:perf_hooks");
const { pathToFileURL } = require("node:url");

const repoRoot = path.resolve(__dirname, "..", "..");
const runtimeSceneDir = path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene");
const normalizerSourcePath = path.join(runtimeSceneDir, "28-webgl-scene-command-batch-normalizer.js");
const linkSourcePath = path.join(runtimeSceneDir, "27-webgl-scene-links.js");
const reportDir = path.join(repoRoot, "artifacts", "webgl-economy-sharedwell-hardening-v9", "performance");

async function main() {
  fs.mkdirSync(reportDir, { recursive: true });
  const normalizer = await importModule(writeCommandBatchAuditModule());
  const links = await importModule(writeLinkAuditModule());

  const motionProof = measure("100 independent motions", () => {
    const batch = {
      batchId: "proof.components.100-motions",
      motions: Array.from({ length: 100 }, (_, index) => ({
        motionId: `motion.${index}`,
        objectId: `actor.${index}`,
        targetPosition: { x: index, y: 0, z: 0 }
      }))
    };
    const normalized = normalizer.normalizeCommandBatchForAudit(batch);
    assertEqual(normalized.metrics.commandCountBeforeNormalization, 100, "100 motions before normalization");
    assertEqual(normalized.metrics.commandCountAfterNormalization, 100, "100 motions after normalization");
    assertEqual(normalized.metrics.estimatedHostInteropCallCount, 1, "100 motions estimated interop calls");
    assertEqual(normalized.metrics.interopCallsAvoided, 99, "100 motions avoided interop calls");
    return {
      inputMotionCount: 100,
      outputMotionCount: normalized.motions.length,
      interopCallsAvoided: normalized.metrics.interopCallsAvoided,
      warnings: normalized.warnings || []
    };
  });

  const stagedProof = measure("25 actors with 4 ordered stages", () => {
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

  const linkProof = measure("300 links with indexed moved actor update", () => {
    const state = {
      objectLookup: new Map(),
      linkGroupsByObjectId: new Map(),
      diagnostics: {}
    };
    state.objectLookup.set("actor.moved", objectAt(0, 0));
    for (let index = 0; index < 600; index++) {
      state.objectLookup.set(`endpoint.${index}`, objectAt(index + 1, index % 7));
    }

    const totalLinkCount = 300;
    const movedActorLinkCount = 4;
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

  const proof = {
    generatedAtUtc: new Date().toISOString(),
    largeScreenOnly: true,
    operations: [motionProof, stagedProof, linkProof],
    bottleneckNotes: [
      "Command batching keeps 100 motion commands in one normalized host interop batch.",
      "Ordered stage policy preserves stage boundaries and prevents duplicate-motion collapse across ordered stages.",
      "Link synchronization uses linkGroupsByObjectId, so moving one actor updates only indexed adjacent links instead of scanning all scene links."
    ]
  };

  const reportPath = path.join(reportDir, "components-performance-proof.json");
  fs.writeFileSync(reportPath, `${JSON.stringify(proof, null, 2)}\n`, "utf8");
  console.log(`Shared Well WebGlLib performance audit passed. Wrote ${path.relative(repoRoot, reportPath)}.`);
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
