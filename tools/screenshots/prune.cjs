// Lists local screenshots_* branches in config.storageRepoPath older than the most recent
// --keep (default 10) and deletes them after an interactive confirm (skip with --yes).
// Only touches origin if --remote is also passed.
// Usage: node prune.cjs [--config <path>] [--keep <n>] [--yes] [--remote]

const path = require("node:path");
const readline = require("node:readline");
const { loadConfig } = require("./lib/config.cjs");
const { listScreenshotBranches, deleteBranch } = require("./lib/git-storage.cjs");
const { parseArgs } = require("./lib/args.cjs");

const args = parseArgs(process.argv.slice(2));
const configPath = args.config || path.join(__dirname, "screenshots.config.json");
const keep = Number(args.keep ?? 10);

main().catch(error => {
  console.error(error.stack || error.message || String(error));
  process.exitCode = 1;
});

async function main() {
  const config = loadConfig(configPath);
  const branches = listScreenshotBranches(config.storageRepoPath);
  const toDelete = branches.slice(keep);

  if (toDelete.length === 0) {
    console.log(`Nothing to prune: ${branches.length} screenshots_* branch(es) in ${config.storageRepoPath}, keeping ${keep}.`);
    return;
  }

  console.log(`Will delete ${toDelete.length} branch(es) from ${config.storageRepoPath}:`);
  for (const branch of toDelete) {
    console.log(`  - ${branch}`);
  }

  if (args.yes !== "true") {
    const confirmed = await confirm("Delete these local branches? [y/N] ");
    if (!confirmed) {
      console.log("Aborted.");
      return;
    }
  }

  for (const branch of toDelete) {
    deleteBranch(config.storageRepoPath, branch, { remote: args.remote === "true" });
    console.log(`Deleted ${branch}`);
  }
}

function confirm(question) {
  const rl = readline.createInterface({ input: process.stdin, output: process.stdout });
  return new Promise(resolve => {
    rl.question(question, answer => {
      rl.close();
      resolve(/^y(es)?$/i.test(answer.trim()));
    });
  });
}
