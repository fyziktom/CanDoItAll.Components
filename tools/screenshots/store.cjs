// Commits config.outputDir (the result of capture.cjs + diff.cjs + report.cjs) to a new
// orphan branch in config.storageRepoPath (a separate git repo from this one). Writes
// config.gitignore into the run first. Never pushes; prints the push command to run
// yourself. Usable standalone after a manual capture/diff/report, or invoked by
// snapshot.cjs with --branch so the branch name matches what report.cjs already titled
// the README with.
// Usage: node store.cjs [--config <path>] [--branch <name>]

const fs = require("node:fs");
const path = require("node:path");
const { loadConfig } = require("./lib/config.cjs");
const { ensureRepo, createSnapshotBranch } = require("./lib/git-storage.cjs");
const { generateBranchName } = require("./lib/branch-name.cjs");
const { parseArgs } = require("./lib/args.cjs");

const args = parseArgs(process.argv.slice(2));
const configPath = args.config || path.join(__dirname, "screenshots.config.json");

main().catch(error => {
  console.error(error.stack || error.message || String(error));
  process.exitCode = 1;
});

async function main() {
  const config = loadConfig(configPath);
  ensureRepo(config.storageRepoPath);

  const manifestPath = path.join(config.outputDir, "data", "manifest.json");
  if (!fs.existsSync(manifestPath)) {
    throw new Error(`No capture found at ${config.outputDir}. Run "npm run visual:capture" first.`);
  }

  writeGitignore(config);

  const branchName = args.branch || generateBranchName();
  createSnapshotBranch(config.storageRepoPath, branchName, config.outputDir);

  console.log(`\nSnapshot committed to branch "${branchName}" in ${config.storageRepoPath}`);
  console.log("This branch was not pushed. Share it yourself when ready:");
  console.log(`  git -C "${config.storageRepoPath}" push origin ${branchName}`);
}

function writeGitignore(config) {
  const patterns = config.gitignore ?? [];
  if (patterns.length === 0) {
    return;
  }

  const contents = `${patterns.join("\n")}\n`;
  fs.writeFileSync(path.join(config.outputDir, ".gitignore"), contents);
}
