const fs = require("node:fs");
const path = require("node:path");

const repoRoot = path.resolve(__dirname, "..", "..");
const inventoryPath = path.join(repoRoot, "artifacts", "webgl-runtime-hardening-v2", "glb-inventory.json");
const outputDir = path.join(repoRoot, "artifacts", "webgl-engine-prep-v4");
const jsonPath = path.join(outputDir, "model-diagnostics.json");
const markdownPath = path.join(outputDir, "model-diagnostics.md");

if (!fs.existsSync(inventoryPath)) {
  throw new Error(`Missing GLB inventory: ${path.relative(repoRoot, inventoryPath)}`);
}

const inventory = JSON.parse(fs.readFileSync(inventoryPath, "utf8"));
const diagnostics = inventory.map(item => diagnoseInventoryItem(item));
fs.mkdirSync(outputDir, { recursive: true });
fs.writeFileSync(jsonPath, `${JSON.stringify(diagnostics, null, 2)}\n`, "utf8");
fs.writeFileSync(markdownPath, buildMarkdown(diagnostics), "utf8");
console.log(`Wrote ${path.relative(repoRoot, jsonPath)} and ${path.relative(repoRoot, markdownPath)} for ${diagnostics.length} model asset(s).`);

function diagnoseInventoryItem(item) {
  const filePath = path.join(repoRoot, item.path);
  const result = {
    assetId: item.proposedAssetId,
    variantId: item.qualityTier,
    uri: toRuntimeUri(item.path),
    sourcePath: item.path,
    byteSize: item.byteSize,
    loadOk: false,
    meshCount: 0,
    visibleMeshCount: 0,
    materialCount: 0,
    transparentMaterialCount: 0,
    bounds: vector(0, 0, 0),
    center: vector(0, 0, 0),
    warnings: [],
    errors: [],
    suggestedImportRecipe: buildImportRecipe(item)
  };

  if (!fs.existsSync(filePath)) {
    result.errors.push("Model file is missing.");
    return result;
  }

  try {
    const model = item.path.toLowerCase().endsWith(".glb")
      ? readGlbJson(filePath)
      : JSON.parse(fs.readFileSync(filePath, "utf8"));
    result.loadOk = true;
    result.meshCount = Array.isArray(model.meshes) ? model.meshes.length : 0;
    result.visibleMeshCount = result.meshCount;
    result.materialCount = Array.isArray(model.materials) ? model.materials.length : 0;
    result.transparentMaterialCount = countTransparentMaterials(model);
    const bounds = resolveAccessorBounds(model);
    result.bounds = bounds.size;
    result.center = bounds.center;
    result.warnings.push(...buildWarnings(result, model));
    return result;
  } catch (error) {
    result.errors.push(error instanceof Error ? error.message : String(error));
    return result;
  }
}

function readGlbJson(filePath) {
  const buffer = fs.readFileSync(filePath);
  if (buffer.length < 20 || buffer.toString("utf8", 0, 4) !== "glTF") {
    throw new Error("Invalid GLB header.");
  }

  const version = buffer.readUInt32LE(4);
  if (version !== 2) {
    throw new Error(`Unsupported GLB version ${version}.`);
  }

  let offset = 12;
  while (offset + 8 <= buffer.length) {
    const chunkLength = buffer.readUInt32LE(offset);
    const chunkType = buffer.readUInt32LE(offset + 4);
    offset += 8;
    if (chunkType === 0x4e4f534a) {
      return JSON.parse(buffer.toString("utf8", offset, offset + chunkLength));
    }

    offset += chunkLength;
  }

  throw new Error("GLB JSON chunk was not found.");
}

function resolveAccessorBounds(model) {
  let min = [Number.POSITIVE_INFINITY, Number.POSITIVE_INFINITY, Number.POSITIVE_INFINITY];
  let max = [Number.NEGATIVE_INFINITY, Number.NEGATIVE_INFINITY, Number.NEGATIVE_INFINITY];
  let found = false;
  for (const mesh of model.meshes || []) {
    for (const primitive of mesh.primitives || []) {
      const positionAccessorIndex = primitive.attributes?.POSITION;
      const accessor = Number.isInteger(positionAccessorIndex) ? model.accessors?.[positionAccessorIndex] : null;
      if (!accessor?.min || !accessor?.max) {
        continue;
      }

      for (let index = 0; index < 3; index += 1) {
        min[index] = Math.min(min[index], Number(accessor.min[index]));
        max[index] = Math.max(max[index], Number(accessor.max[index]));
      }

      found = true;
    }
  }

  if (!found) {
    return { size: vector(0, 0, 0), center: vector(0, 0, 0) };
  }

  return {
    size: vector(max[0] - min[0], max[1] - min[1], max[2] - min[2]),
    center: vector((max[0] + min[0]) / 2, (max[1] + min[1]) / 2, (max[2] + min[2]) / 2)
  };
}

function countTransparentMaterials(model) {
  return (model.materials || []).filter(material => {
    const alphaMode = String(material.alphaMode || "OPAQUE").toUpperCase();
    const baseColor = material.pbrMetallicRoughness?.baseColorFactor || [];
    return alphaMode === "BLEND" || alphaMode === "MASK" || Number(baseColor[3]) < 1;
  }).length;
}

function buildWarnings(result, model) {
  const warnings = [];
  if (result.meshCount === 0) {
    warnings.push("No meshes found in model JSON.");
  }

  if (result.bounds.x === 0 && result.bounds.y === 0 && result.bounds.z === 0) {
    warnings.push("No POSITION accessor bounds were found; runtime should use debug bounds.");
  }

  if (Math.max(result.bounds.x, result.bounds.y, result.bounds.z) > 100) {
    warnings.push("Model bounds are large; use fit-bounds import mode or fixed scale.");
  }

  if (Math.hypot(result.center.x, result.center.y, result.center.z) > 50) {
    warnings.push("Model center is far from origin; use center-bottom or center-bounds import mode.");
  }

  if ((model.nodes || []).some(node => node.mesh !== undefined && node.scale?.some(value => Number(value) === 0))) {
    warnings.push("One or more mesh nodes have a zero scale component.");
  }

  if (result.transparentMaterialCount > 0) {
    warnings.push("Transparent materials detected; enable material visibility normalization when invisible.");
  }

  return warnings;
}

function buildImportRecipe(item) {
  return {
    unitScale: 1,
    centerMode: "center-bottom",
    fitMode: item.byteSize > 350000 ? "fit-bounds" : "original-scale",
    fixedScale: 1,
    rotationOffset: vector(0, 0, 0),
    positionOffset: vector(0, 0, 0),
    doubleSidedMaterial: true,
    materialVisibilityNormalization: true,
    debugBounds: !item.usedBySandbox,
    notes: [
      item.usedBySandbox ? "Catalog-wired model." : "Catalog alternative; inspect in Model Lab before high-detail use.",
      `Fallback asset: ${item.fallbackAssetId}.`
    ]
  };
}

function buildMarkdown(items) {
  const rows = items.map(item => [
    item.assetId,
    item.variantId,
    item.loadOk ? "yes" : "no",
    item.meshCount,
    item.visibleMeshCount,
    item.materialCount,
    item.transparentMaterialCount,
    `${item.bounds.x} x ${item.bounds.y} x ${item.bounds.z}`,
    item.warnings.concat(item.errors).join("; ") || "ok"
  ]);

  return [
    "# WebGL Model Diagnostics",
    "",
    "Deterministic batch diagnostics generated from repository GLB/GLTF metadata. Browser proof still validates actual rendering.",
    "",
    "| asset id | variant | load ok | meshes | visible meshes | materials | transparent | bounds | warnings/errors |",
    "| --- | --- | --- | ---: | ---: | ---: | ---: | --- | --- |",
    ...rows.map(row => `| ${row.map(escapeCell).join(" | ")} |`),
    ""
  ].join("\n");
}

function toRuntimeUri(sourcePath) {
  if (sourcePath.startsWith("src/CanDoItAll.Components.WebGlLib/wwwroot/")) {
    return `/_content/CanDoItAll.Components.WebGlLib/${sourcePath.substring("src/CanDoItAll.Components.WebGlLib/wwwroot/".length)}`;
  }

  if (sourcePath.startsWith("3DModels/")) {
    return `/assets/external-models/${sourcePath.substring("3DModels/".length)}`;
  }

  return sourcePath;
}

function vector(x, y, z) {
  return { x: round(x), y: round(y), z: round(z) };
}

function round(value) {
  return Number.isFinite(value) ? Number(value.toFixed(4)) : 0;
}

function escapeCell(value) {
  return String(value).replaceAll("|", "\\|");
}
