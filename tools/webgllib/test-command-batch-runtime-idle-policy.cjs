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
const commandResultsPath = path.join(
    repoRoot,
    "src",
    "CanDoItAll.Components.WebGlLib",
    "wwwroot",
    "js",
    "runtime",
    "scene",
    "20-webgl-scene-command-results.js");
const commandCompactionPath = path.join(
    repoRoot,
    "src",
    "CanDoItAll.Components.WebGlLib",
    "wwwroot",
    "js",
    "runtime",
    "scene",
    "42-webgl-scene-command-result-compaction.js");
const motionPath = path.join(
    repoRoot,
    "src",
    "CanDoItAll.Components.WebGlLib",
    "wwwroot",
    "js",
    "runtime",
    "scene",
    "14-webgl-scene-motion.js");
const notificationsPath = path.join(
    repoRoot,
    "src",
    "CanDoItAll.Components.WebGlLib",
    "wwwroot",
    "js",
    "runtime",
    "scene",
    "24-webgl-scene-notifications.js");
const corePath = path.join(
    repoRoot,
    "src",
    "CanDoItAll.Components.WebGlLib",
    "wwwroot",
    "js",
    "runtime",
    "scene",
    "02-webgl-scene-core.js");

const source = fs.readFileSync(commandBatchPath, "utf8");
const commandResultsSource = fs.readFileSync(commandResultsPath, "utf8");
const commandCompactionSource = fs.readFileSync(commandCompactionPath, "utf8");
const motionSource = fs.readFileSync(motionPath, "utf8");
const notificationsSource = fs.readFileSync(notificationsPath, "utf8");
const coreSource = fs.readFileSync(corePath, "utf8");

assertContains(
    "const policyMode = String(options?.policyMode || \"visualStrict\").trim() || \"visualStrict\";",
    "applyCommandBatchAndWait must normalize policyMode from caller options and default to visualStrict.");

assertContains(
    "waitForRuntimeIdle(state, { timeoutMs, pollIntervalMs, reason, policyMode })",
    "applyCommandBatchAndWait must pass policyMode to waitForRuntimeIdle.");

assertContains(
    "result.metadata.runtimeIdlePolicyMode = String(idleResult?.policyMode || \"\")",
    "command batch idle proof must expose the resolved policy mode in metadata.");

assertContains(
    "withSuppressedChildCommandCallbacks(state, () => {",
    "command batch execution must suppress child command callbacks while retaining the top-level batch result.");

assertContains(
    "state.suppressCommandResultCallbacks = previous;",
    "command batch child callback suppression must restore the previous callback state.");

assertCommandResultsContains(
    "normalizeLimit } from \"./42-webgl-scene-command-result-compaction.js\"",
    "command result history must import normalizeLimit after moving batch compaction helpers.");

assertCommandResultsContains(
    "const defaultMaxCallbackMessages = 5;",
    "command result callbacks must own a compact message limit after moving batch compaction helpers.");

assertCommandResultsDoesNotContain(
    "defaultMaxBatchMessages",
    "command result callbacks must not reference batch compaction module-private constants.");

assertCommandResultsContains(
    "state.suppressCommandResultCallbacks === true",
    "command result notification must honor batch child callback suppression.");

assertCommandResultsContains(
    "state.options?.notifyCommandCompleted === false",
    "command completed callbacks must be skipped when the component has no handler.");

assertCommandResultsContains(
    "state.options?.notifyCommandFailed === false",
    "command failed callbacks must be skipped when the component has no handler.");

assertMotionContains(
    "state?.options?.notifyMotionCompleted !== false",
    "motion completed callbacks must be gated by interop callback options.");

assertMotionContains(
    "state?.options?.notifyCommandCompleted !== false",
    "motion completion command-result callbacks must be gated by interop callback options.");

assertNotificationsContains(
    "state?.options?.notifyStateChanged === false",
    "state changed callbacks must be skipped when the component has no handler.");

assertCoreContains(
    "\"notifyCommandCompleted\": options?.notifyCommandCompleted !== false",
    "runtime option normalization must preserve command callback gating.");

assertCoreContains(
    "\"notifyMotionCompleted\": options?.notifyMotionCompleted !== false",
    "runtime option normalization must preserve motion callback gating.");

assertCoreContains(
    "maxCommandBatchProofSnapshotPositions: Math.max(0, Math.min(1000, resolveFiniteNumber(options?.maxCommandBatchProofSnapshotPositions, 10)))",
    "runtime option normalization must preserve command batch proof snapshot compaction limits.");

assertCoreContains(
    "maxCommandBatchChildResults: Math.max(1, Math.min(1000, resolveFiniteNumber(options?.maxCommandBatchChildResults, 5)))",
    "runtime option normalization must keep command batch child result defaults compact.");

assertCoreContains(
    "maxCommandBatchMessages: Math.max(1, Math.min(1000, resolveFiniteNumber(options?.maxCommandBatchMessages, 5)))",
    "runtime option normalization must keep command batch message defaults compact.");

assertCommandCompactionContains(
    "const defaultMaxBatchChildResults = 5;",
    "command batch interop results must keep child result payloads compact by default.");

assertCommandCompactionContains(
    "const defaultMaxBatchMessages = 5;",
    "command batch interop results must keep warning and error payloads compact by default.");

assertCommandCompactionContains(
    "const defaultMaxBatchProofSnapshotPositions = 10;",
    "command batch interop results must cap inline proof snapshot positions.");

assertCommandCompactionContains(
    "compactBatchProofSnapshotForInterop(state, result);",
    "command batch interop compaction must include proof snapshot payload trimming.");

assertCommandCompactionContains(
    "proofSnapshotCompactedForInterop: \"true\"",
    "command batch interop compaction must annotate omitted proof snapshot positions.");

console.log("Command batch runtime idle policy audit passed.");

function assertContains(expected, message) {
    if (!source.includes(expected)) {
        throw new Error(`${message}\nMissing source fragment: ${expected}`);
    }
}

function assertCommandResultsContains(expected, message) {
    if (!commandResultsSource.includes(expected)) {
        throw new Error(`${message}\nMissing source fragment: ${expected}`);
    }
}

function assertCommandResultsDoesNotContain(unexpected, message) {
    if (commandResultsSource.includes(unexpected)) {
        throw new Error(`${message}\nUnexpected source fragment: ${unexpected}`);
    }
}

function assertCommandCompactionContains(expected, message) {
    if (!commandCompactionSource.includes(expected)) {
        throw new Error(`${message}\nMissing source fragment: ${expected}`);
    }
}

function assertMotionContains(expected, message) {
    if (!motionSource.includes(expected)) {
        throw new Error(`${message}\nMissing source fragment: ${expected}`);
    }
}

function assertNotificationsContains(expected, message) {
    if (!notificationsSource.includes(expected)) {
        throw new Error(`${message}\nMissing source fragment: ${expected}`);
    }
}

function assertCoreContains(expected, message) {
    if (!coreSource.includes(expected)) {
        throw new Error(`${message}\nMissing source fragment: ${expected}`);
    }
}
