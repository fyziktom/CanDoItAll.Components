// End-to-end run: capture.cjs -> diff.cjs -> report.cjs -> store.cjs. The branch name is
// generated up front and passed to both report.cjs (--branch, for the README title) and
// store.cjs (--branch, for the actual commit), so the two always agree.
// Usage: node snapshot.cjs [--config <path>] [--against <branch>]

const path = require("node:path");
const { execFileSync } = require("node:child_process");
const { loadConfig } = require("./lib/config.cjs");
const { ensureRepo } = require("./lib/git-storage.cjs");
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

  const branchName = generateBranchName(config.key);

  run("capture.cjs", configPath);
  run("diff.cjs", configPath, args.against ? ["--against", args.against] : []);
  run("report.cjs", configPath, ["--branch", branchName]);
  run("store.cjs", configPath, ["--branch", branchName]);
}

function run(script, resolvedConfigPath, extraArgs = []) {
  execFileSync(
    process.execPath,
    [path.join(__dirname, script), "--config", resolvedConfigPath, ...extraArgs],
    { stdio: "inherit" }
  );
}
