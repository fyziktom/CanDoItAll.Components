import fs from "node:fs";
import path from "node:path";

const root = process.cwd();
const bundleRoot = path.join(root, "codex", "bundles", "StandardComponents_PublishingReadiness_v1");

function read(relativePath) {
  return fs.readFileSync(path.join(root, relativePath), "utf8");
}

function assert(condition, message) {
  if (!condition) {
    throw new Error(message);
  }
}

function exists(relativePath) {
  return fs.existsSync(path.join(root, relativePath));
}

const files = {
  fields: read("Tailwind/forms/fields.css"),
  buttons: read("Tailwind/controls/buttons.css"),
  theme: read("Tailwind/foundation/theme.css"),
  tabs: read("Tailwind/navigation/tabs.css"),
  selectionListItem: read("src/CanDoItAll.Components.BaseLib/Components/Lists/SelectionListItem.razor"),
  policy: read("docs/standard-components-tailwind-policy.md"),
};

assert(files.fields.includes("@apply flex flex-col gap-[0.35rem];"), "prefixed-field mobile stack must use Tailwind @apply");
assert(files.fields.includes("@apply block w-full resize-y leading-6;"), "textarea layout must use Tailwind @apply");
assert(files.buttons.includes("@apply min-h-7 min-w-7 p-0;"), "copy button xs icon size must use Tailwind @apply");
assert(files.buttons.includes("@apply inline-flex cursor-pointer items-center justify-center border-0 bg-transparent;"), "copy icon base layout must use Tailwind @apply");
assert(files.theme.includes("@apply block min-h-full;"), "theme host layout must use Tailwind @apply");
assert(files.tabs.includes("@apply flex min-w-0 w-full max-w-full gap-0;"), "tabs root must be width constrained");
assert(files.tabs.includes(".cad-tabs--position-left.cad-tabs--vertical,\n    .cad-tabs--position-right.cad-tabs--vertical"), "mobile vertical tabs must override left/right position specificity");
assert(files.tabs.includes("@apply w-full flex-none border-l-0 border-r-0 p-0;"), "mobile vertical tab list must become full-width");
assert(files.selectionListItem.includes('Wrap="FlexWrap.Wrap"'), "SelectionListItem must allow actions to wrap");
assert(files.selectionListItem.includes('class="cda-selection-list-item__actions min-w-0 w-full sm:w-auto"'), "SelectionListItem actions must stack full-width on mobile");
assert(files.policy.includes("Use Tailwind Composition"), "Tailwind policy must define composition guidance");
assert(files.policy.includes("Keep Raw CSS With Rationale"), "Tailwind policy must define raw CSS rationale");

const baseline = JSON.parse(read("codex/bundles/StandardComponents_PublishingReadiness_v1/proof/SB02/data/sb02-visual-baseline.json"));
const after = JSON.parse(read("codex/bundles/StandardComponents_PublishingReadiness_v1/proof/SB02/data/sb02-visual-after.json"));

const baselineActionsMobile = baseline.summary.find((item) => item.route === "actions" && item.viewport === "390" && item.kind === "default");
const baselineTabsMobile = baseline.summary.find((item) => item.route === "tabs" && item.viewport === "390" && item.kind === "default");
assert(baselineActionsMobile?.viewportOverflows > 0, "baseline must prove mobile actions had viewport overflow pressure");
assert(baselineTabsMobile?.viewportOverflows > 0, "baseline must prove mobile tabs had viewport overflow pressure");

for (const item of after.summary) {
  assert(item.pageHorizontalOverflow === false, `after visual must not have page horizontal overflow: ${item.route}/${item.viewport}/${item.kind}`);
  assert(item.clippedText === 0, `after visual must not have clipped visible text: ${item.route}/${item.viewport}/${item.kind}`);
  assert(exists(path.join("codex/bundles/StandardComponents_PublishingReadiness_v1", item.screenshot)), `missing screenshot ${item.screenshot}`);
}

const mcpScreenshots = [
  "codex/bundles/StandardComponents_PublishingReadiness_v1/proof/SB02/screenshots/mcp/sb02-baseline-inputs-1366.png",
  "codex/bundles/StandardComponents_PublishingReadiness_v1/proof/SB02/screenshots/mcp/sb02-after-tabs-390.png",
];

for (const screenshot of mcpScreenshots) {
  assert(exists(screenshot), `missing MCP screenshot ${screenshot}`);
}

assert(exists("src/CanDoItAll.Components.BaseLib/wwwroot/css/output.css"), "Tailwind output.css must exist");

console.log("SB02-INV-001 Tailwind utility-composition replacements present");
console.log("SB02-INV-002 Mobile tabs and list-item action hardening present");
console.log("SB02-INV-003 Before/after visual evidence proves repaired viewport/clipping defects");
