"use strict";

const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const { renderGeneratedText } = require("./generated-text.cjs");

const repoRoot = path.resolve(__dirname, "..", "..");
const generatedComponentPaths = [
  "src/CanDoItAll.Components.CanvasLib/Components/Shared/Assets/CanvasLibHeadAssets.razor",
  "src/CanDoItAll.Components.CanvasLib/Components/Shared/Assets/CanvasLibBodyAssets.razor",
  "src/CanDoItAll.Components.CanvasLib/Components/Shared/Assets/CanvasRuntimeBodyAssets.razor",
  "src/CanDoItAll.Components.WebGlLib/Components/Shared/Assets/WebGlLibHeadAssets.razor",
  "src/CanDoItAll.Components.WebGlLib/Components/Shared/Assets/WebGlLibBodyAssets.razor"
];

test("generated text uses canonical LF line endings with one final newline", () => {
  const actual = renderGeneratedText(["first", "", "  ", "last"]);

  assert.equal(actual, "first\n\n  \nlast\n");
});

test("generated Razor asset components use canonical LF line endings", () => {
  for (const relativePath of generatedComponentPaths) {
    const content = fs.readFileSync(path.join(repoRoot, relativePath), "utf8");

    assert.equal(content.includes("\r"), false, `${relativePath} contains a CR character.`);
    assert.equal(content.endsWith("\n"), true, `${relativePath} must end with a newline.`);
  }
});
