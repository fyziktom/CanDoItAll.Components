const fs = require("node:fs");
const path = require("node:path");

const repoRoot = path.resolve(__dirname, "..", "..");
const commandBatchPath = path.join(
    repoRoot,
    "src",
    "CanDoItAll.Components.WebGlLib",
    "wwwroot",
    "js",
    "runtime",
    "scene",
    "26-webgl-scene-command-batch.js");

const source = fs.readFileSync(commandBatchPath, "utf8");

assertContains(
    "const policyMode = String(options?.policyMode || \"visualStrict\").trim() || \"visualStrict\";",
    "applyCommandBatchAndWait must normalize policyMode from caller options and default to visualStrict.");

assertContains(
    "waitForRuntimeIdle(state, { timeoutMs, pollIntervalMs, reason, policyMode })",
    "applyCommandBatchAndWait must pass policyMode to waitForRuntimeIdle.");

assertContains(
    "result.metadata.runtimeIdlePolicyMode = String(idleResult?.policyMode || \"\")",
    "command batch idle proof must expose the resolved policy mode in metadata.");

console.log("Command batch runtime idle policy audit passed.");

function assertContains(expected, message) {
    if (!source.includes(expected)) {
        throw new Error(`${message}\nMissing source fragment: ${expected}`);
    }
}
