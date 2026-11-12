// Diffs the most recent capture (config.outputDir/data/manifest.json) against a baseline
// screenshots_* branch (most recent by default, or --against <branch>) using odiff-bin.
// Compares by current/{combo}/{page}.png filename against the baseline branch's own
// current/ tree (also produced by this tool, so baseline branches created before the
// current/data/pages restructure will show everything as added/removed once). Copies
// changed/removed baseline images into outputDir/baseline/{combo}/{page}.png and writes diff
// images into outputDir/diff/{combo}/{page}.png, plus data/diff-results.json. Requires
// "npm run visual:capture" to have already run. Read-only against storageRepoPath.
// Usage: node diff.cjs [--config <path>] [--against <branch>]

const fs = require("node:fs");
const path = require("node:path");
const { compare } = require("odiff-bin");
const { loadConfig } = require("./lib/config.cjs");
const { listScreenshotBranches, checkoutBranchToTemp } = require("./lib/git-storage.cjs");
const { parseArgs } = require("./lib/args.cjs");

const args = parseArgs(process.argv.slice(2));
const configPath = args.config || path.join(__dirname, "screenshots.config.json");

main().catch(error => {
  console.error(error.stack || error.message || String(error));
  process.exitCode = 1;
});

async function main() {
  const config = loadConfig(configPath);
  const dataDir = path.join(config.outputDir, "data");
  const manifestPath = path.join(dataDir, "manifest.json");

  if (!fs.existsSync(manifestPath)) {
    throw new Error(`No capture manifest found at ${manifestPath}. Run "npm run visual:capture" first.`);
  }

  const manifest = JSON.parse(fs.readFileSync(manifestPath, "utf8"));
  const branches = listScreenshotBranches(config.storageRepoPath, config.key);
  const against = args.against || branches[0];

  if (!against) {
    console.log(`No prior ${config.key}_* branch found — nothing to diff against (first run).`);
    writeResults(dataDir, { against: null, results: [] });
    return;
  }

  console.log(`Diffing capture against baseline branch "${against}"...`);
  const baseline = checkoutBranchToTemp(config.storageRepoPath, against);
  const baselineCurrentDir = path.join(baseline.path, "current");
  const currentDir = path.join(config.outputDir, "current");
  const diffDir = path.join(config.outputDir, "diff");
  const baselineDir = path.join(config.outputDir, "baseline");

  const results = [];

  try {
    for (const job of manifest.jobs) {
      if (job.error) {
        results.push({ key: job.key, status: "capture-failed", error: job.error });
        continue;
      }

      const currentPath = path.join(currentDir, job.file);
      const baselinePath = path.join(baselineCurrentDir, job.file);

      if (!fs.existsSync(baselinePath)) {
        results.push({ key: job.key, status: "added" });
        continue;
      }

      const diffPath = path.join(diffDir, job.file);
      fs.mkdirSync(path.dirname(diffPath), { recursive: true });
      const result = await compare(baselinePath, currentPath, diffPath, { threshold: 0.1 });

      if (result.match) {
        results.push({ key: job.key, status: "unchanged" });
        continue;
      }

      const baselineCopyPath = path.join(baselineDir, job.file);
      fs.mkdirSync(path.dirname(baselineCopyPath), { recursive: true });
      fs.copyFileSync(baselinePath, baselineCopyPath);
      const baselineFile = `baseline/${job.file}`;

      if (result.reason === "layout-diff") {
        results.push({ key: job.key, status: "changed", reason: "layout-diff", baselineFile });
      } else if (result.reason === "pixel-diff") {
        const { width, height } = readPngDimensions(currentPath);
        results.push({
          key: job.key,
          status: "changed",
          reason: "pixel-diff",
          diffPercentage: result.diffPercentage,
          diffCount: result.diffCount,
          totalPixels: width * height,
          baselineFile,
          diffFile: `diff/${job.file}`,
        });
      } else {
        results.push({ key: job.key, status: "error", reason: result.reason, baselineFile });
      }
    }

    const baselineFiles = listPngFilesRecursive(baselineCurrentDir);
    const currentFiles = new Set(manifest.jobs.map(job => job.file).filter(Boolean));
    for (const file of baselineFiles) {
      if (!currentFiles.has(file)) {
        const baselineCopyPath = path.join(baselineDir, file);
        fs.mkdirSync(path.dirname(baselineCopyPath), { recursive: true });
        fs.copyFileSync(path.join(baselineCurrentDir, file), baselineCopyPath);
        results.push({
          key: file.replace(/\.png$/, ""),
          status: "removed",
          baselineFile: `baseline/${file}`,
        });
      }
    }
  } finally {
    baseline.cleanup();
  }

  writeResults(dataDir, { against, results });

  const changed = results.filter(entry => entry.status === "changed");
  console.log(`Diff complete against "${against}": ${results.length} compared, ${changed.length} changed.`);
}

// Reads width/height straight from the PNG's IHDR chunk (bytes 16-23), so the status line
// can report an exact "N of M pixels changed" instead of just odiff's rounded percentage.
function readPngDimensions(filePath) {
  const fd = fs.openSync(filePath, "r");
  try {
    const buffer = Buffer.alloc(24);
    fs.readSync(fd, buffer, 0, 24, 0);
    return { width: buffer.readUInt32BE(16), height: buffer.readUInt32BE(20) };
  } finally {
    fs.closeSync(fd);
  }
}

// Lists *.png files under rootDir, returned as POSIX-style paths relative to rootDir, so
// they line up with manifest job.file values regardless of nesting depth.
function listPngFilesRecursive(rootDir) {
  const results = [];
  const walk = dir => {
    for (const entry of fs.readdirSync(dir, { withFileTypes: true })) {
      const full = path.join(dir, entry.name);
      if (entry.isDirectory()) {
        walk(full);
      } else if (entry.name.endsWith(".png")) {
        results.push(path.relative(rootDir, full).split(path.sep).join("/"));
      }
    }
  };

  if (fs.existsSync(rootDir)) {
    walk(rootDir);
  }

  return results;
}

function writeResults(dataDir, payload) {
  fs.mkdirSync(dataDir, { recursive: true });
  const resultsPath = path.join(dataDir, "diff-results.json");
  fs.writeFileSync(
    resultsPath,
    `${JSON.stringify({ generatedAtUtc: new Date().toISOString(), ...payload }, null, 2)}\n`
  );
  console.log(`Wrote ${resultsPath}`);
}
