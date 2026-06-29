#!/usr/bin/env node

import fs from 'node:fs';
import path from 'node:path';

const bundleRoot = path.resolve('codex/bundles/StandardComponents_PublishingReadiness_v1');

const requiredSubbundles = Array.from({ length: 12 }, (_, index) => String(index + 1).padStart(2, '0'));
const criticalSubbundles = ['01', '02', '03', '04', '05', '10', '11', '12'];
const rawNotes = Array.from({ length: 10 }, (_, index) => `RAW${String(index + 1).padStart(2, '0')}`);

const checks = [];

function read(relativePath) {
  return fs.readFileSync(path.join(bundleRoot, relativePath), 'utf8');
}

function exists(relativePath) {
  return fs.existsSync(path.join(bundleRoot, relativePath));
}

function pass(name) {
  checks.push({ name, passed: true });
  console.log(`PASS: ${name}`);
}

function fail(name, details) {
  checks.push({ name, passed: false, details });
  console.error(`FAIL: ${name}${details ? ` - ${details}` : ''}`);
}

function assert(condition, name, details = '') {
  if (condition) {
    pass(name);
    return;
  }

  fail(name, details);
}

function assertIncludes(content, expected, name) {
  assert(content.includes(expected), name, `missing ${expected}`);
}

const rootReadme = read('README.md');
const executionReport = read('reviews/01-execution-report.md');
const rawClosure = read('proof/SB12/raw-note-closure.md');
const transferChecklist = read('proof/SB12/transfer-checklist.md');
const residualRisk = read('proof/SB12/residual-risk-and-followup.md');
const packageVerification = JSON.parse(read('proof/SB12/data/sb12-package-verification.json'));
const matrix = JSON.parse(read('proof/SB11/data/sb11-visual-matrix.json'));

for (const subbundleNumber of requiredSubbundles) {
  const directory = fs
    .readdirSync(path.join(bundleRoot, 'subbundles'))
    .find(name => name.startsWith(`${subbundleNumber}-`));
  const subbundleReadme = read(`subbundles/${directory}/README.md`);
  assertIncludes(subbundleReadme, 'Status: `Completed`', `SB12-INV-001 subbundle ${subbundleNumber} is completed`);
}

for (const subbundleNumber of criticalSubbundles) {
  assert(exists(`proof/SB${subbundleNumber}/manifest.md`), `SB12-INV-001 SB${subbundleNumber} manifest exists`);
  assert(exists(`proof/SB${subbundleNumber}/semantic-invariants.md`), `SB12-INV-001 SB${subbundleNumber} semantic invariants exist`);
  assertIncludes(executionReport, `proof/SB${subbundleNumber}/semantic-invariants.md`, `SB12-INV-001 execution report cites SB${subbundleNumber} semantic proof`);
  assertIncludes(executionReport, `## SB${subbundleNumber} Semantic Adequacy Evidence`, `SB12-INV-001 execution report has SB${subbundleNumber} semantic adequacy section`);
}

assertIncludes(rootReadme, 'Final closure gate: `Passed completed-stage validator on 2026-06-29`', 'SB12-INV-001 root README final closure is passed');
assertIncludes(executionReport, 'Execution status: `Completed`', 'SB12-INV-001 execution report is completed');

assert(matrix.routeCount === 51, 'SB12-INV-002 visual matrix covers 51 routes');
assert(matrix.viewportCount === 4, 'SB12-INV-002 visual matrix covers 4 viewports');
assert(matrix.screenshotCount === 102, 'SB12-INV-002 visual matrix captured 102 screenshots');
assert(matrix.checks.every(check => check.passed), 'SB12-INV-002 visual matrix has zero failed checks');
assert((matrix.consoleErrors ?? []).length === 0, 'SB12-INV-002 visual matrix has zero console errors');
assertIncludes(executionReport, 'SB11 | every standard component route/scenario in the final matrix', 'SB12-INV-002 execution report includes SB11 matrix analytics');

assert(packageVerification.passed === true, 'SB12-INV-003 SB12 package verification passed');
assert(packageVerification.packages?.length === 5, 'SB12-INV-003 SB12 package verification has five packages');
for (const packageName of [
  'CanDoItAll.Components.Common',
  'CanDoItAll.Components.BaseLib',
  'CanDoItAll.Components.Charts',
  'CanDoItAll.Components.OverlayLib',
  'CanDoItAll.Components.Mermaid',
]) {
  assert(packageVerification.packages.some(entry => entry.packageId === packageName), `SB12-INV-003 package verified: ${packageName}`);
}

for (const rawNote of rawNotes) {
  assertIncludes(rawClosure, `| ${rawNote} | Solved`, `SB12-INV-004 raw note ${rawNote} is solved in closure table`);
  assertIncludes(executionReport, `| ${rawNote} | Solved`, `SB12-INV-004 execution report closes ${rawNote}`);
}

assertIncludes(transferChecklist, 'WebGL/Canvas implementation is excluded from this transfer checklist', 'SB12-INV-004 transfer checklist excludes WebGL/Canvas implementation');
assertIncludes(residualRisk, 'WebGL/Canvas follow-up bundle', 'SB12-INV-004 residual risk names WebGL/Canvas follow-up bundle');
assertIncludes(rawClosure, 'bundle://inventories/standard-components-publishing-map.xlsx', 'SB12-INV-004 raw closure cites mandatory xlsx map');

const failed = checks.filter(check => !check.passed);
if (failed.length > 0) {
  console.error(`SB12 closure verifier failed: ${failed.length} failed checks`);
  process.exit(1);
}

console.log('All SB12 closure assertions passed.');
