const fs = require("node:fs");
const path = require("node:path");

const repoRoot = path.resolve(__dirname, "..", "..");
const outputDir = path.join(repoRoot, "artifacts", "webgl-scene-hardening");
const outputPath = path.join(outputDir, "glb-inventory.json");
const inventoryMarkdownPath = path.join(outputDir, "01_INVENTORY.md");
const variantMarkdownPath = path.join(outputDir, "ASSET_VARIANT_INVENTORY.md");
const modelExtensions = new Set([".glb", ".gltf"]);

function walk(directory) {
  if (!fs.existsSync(directory)) {
    return [];
  }

  const entries = [];
  for (const entry of fs.readdirSync(directory, { withFileTypes: true })) {
    const fullPath = path.join(directory, entry.name);
    if (entry.isDirectory()) {
      entries.push(...walk(fullPath));
      continue;
    }

    if (modelExtensions.has(path.extname(entry.name).toLowerCase())) {
      entries.push(fullPath);
    }
  }

  return entries;
}

function slug(value) {
  return value
    .replace(/\.[^.]+$/, "")
    .replace(/[_\s]+/g, "-")
    .replace(/[^a-zA-Z0-9-]+/g, "")
    .replace(/--+/g, "-")
    .toLowerCase();
}

function categoryFor(relativePath) {
  const normalized = relativePath.replaceAll("\\", "/").toLowerCase();
  if (normalized.includes("/buildings/")) {
    return "building";
  }

  if (normalized.includes("/people/")) {
    return "person";
  }

  if (normalized.includes("/props/")) {
    return "prop";
  }

  return "library";
}

function qualityTierFor(relativePath, byteSize) {
  if (relativePath.startsWith("src/CanDoItAll.Components.WebGlLib/")) {
    return "model-low";
  }

  return byteSize >= 350000 ? "model-high" : "model-medium";
}

function proposedAssetId(relativePath) {
  const category = categoryFor(relativePath);
  const name = slug(path.basename(relativePath));
  return category === "library"
    ? `asset.library.${name}`
    : `asset.external.${category}.${name}`;
}

function fallbackFor(category) {
  switch (category) {
    case "building":
      return "asset.building.house.default";
    case "person":
      return "asset.agent.person.default";
    case "prop":
      return "asset.primitive.fallback";
    default:
      return "asset.primitive.fallback";
  }
}

const files = [
  ...walk(path.join(repoRoot, "3DModels")),
  ...walk(path.join(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "assets", "model"))
].sort((left, right) => left.localeCompare(right));

const inventory = files.map(filePath => {
  const relativePath = path.relative(repoRoot, filePath).replaceAll("\\", "/");
  const category = categoryFor(relativePath);
  const byteSize = fs.statSync(filePath).size;
  return {
    path: relativePath,
    byteSize,
    proposedAssetId: proposedAssetId(relativePath),
    category,
    qualityTier: qualityTierFor(relativePath, byteSize),
    fallbackAssetId: fallbackFor(category),
    usedBySandbox: relativePath.includes("src/CanDoItAll.Components.WebGlLib") ||
      relativePath.includes("House_") ||
      relativePath.includes("Blacksmith") ||
      relativePath.includes("Inn") ||
      relativePath.includes("Male_Running") ||
      relativePath.includes("Female_Running")
  };
});

fs.mkdirSync(outputDir, { recursive: true });
fs.writeFileSync(outputPath, `${JSON.stringify(inventory, null, 2)}\n`, "utf8");
fs.writeFileSync(inventoryMarkdownPath, buildInventoryMarkdown(inventory), "utf8");
fs.writeFileSync(variantMarkdownPath, buildVariantMarkdown(inventory), "utf8");
console.log(`Wrote ${path.relative(repoRoot, outputPath)} with ${inventory.length} model asset(s).`);

function buildInventoryMarkdown(items) {
  const rows = items.map(item => [
    item.path,
    item.byteSize,
    item.proposedAssetId,
    recommendedUse(item),
    item.fallbackAssetId,
    item.usedBySandbox ? "wired in sandbox/catalog" : "catalog alternative"
  ]);
  return [
    "# WebGL Scene Hardening Inventory",
    "",
    "Large-screen-only hard rule: this follow-up bundle intentionally does not tune small-screen layouts.",
    "",
    "## GLB/GLTF Asset Table",
    "",
    "| asset file | size | logical proposed id | recommended use | fallback id | status |",
    "| --- | ---: | --- | --- | --- | --- |",
    ...rows.map(row => `| ${row.map(escapeCell).join(" | ")} |`),
    ""
  ].join("\n");
}

function buildVariantMarkdown(items) {
  const externalCount = items.filter(item => item.path.startsWith("3DModels/")).length;
  return [
    "# Asset Variant Inventory",
    "",
    `Detected ${items.length} GLB/GLTF assets, including ${externalCount} external/user-provided models under \`3DModels/glb\`.`,
    "",
    "The sandbox catalog exposes the external models as optional high-detail catalog entries and uses selected building/person models as `model-high` variants. Primitive variants remain the default fallback profile.",
    "",
    "Profiles:",
    "",
    "- `primitive`: generated runtime primitives only.",
    "- `model-low`: WebGlLib GLBs plus safe low-detail external alternatives.",
    "- `model-high`: optional external building/person models from `3DModels/glb` with primitive fallback.",
    "",
    "Generated source of truth: `glb-inventory.json`.",
    ""
  ].join("\n");
}

function recommendedUse(item) {
  if (item.category === "building") {
    return item.qualityTier === "model-high" ? "optional high-detail building variant" : "mixed-profile building model";
  }

  if (item.category === "person") {
    return "optional agent/person variant";
  }

  if (item.category === "prop") {
    return "optional prop catalog asset";
  }

  return "shared WebGlLib model asset";
}

function escapeCell(value) {
  return String(value).replaceAll("|", "\\|");
}
