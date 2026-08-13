// Renders data/manifest.json + data/diff-results.json (if present) into config.outputDir:
//   README.md                       — per-alternative screenshot grid + page index
//   current/{combo}/README.md       — gallery of every page's current screenshot for that combo
//   diff/{combo}/README.md          — gallery of every changed page's diff image for that combo
//   pages/{alternative}/{page}.md   — baseline/current/diff per page, paginated across pages
// Uses the Mustache templates in templates/ for the README and per-combo galleries; page
// reports are built directly as markdown strings for full control over conditional sections.
// Requires "npm run visual:capture" (and usually "visual:diff") to have run.
// Usage: node report.cjs [--config <path>] [--branch <name>]

const fs = require("node:fs");
const path = require("node:path");
const Mustache = require("mustache");
const { loadConfig, slugify } = require("./lib/config.cjs");
const { parseBranchTimestamp, humanizeAgo, generateBranchName } = require("./lib/branch-name.cjs");
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
  const diffResultsPath = path.join(dataDir, "diff-results.json");

  if (!fs.existsSync(manifestPath)) {
    throw new Error(`No capture manifest found at ${manifestPath}. Run "npm run visual:capture" first.`);
  }

  const manifest = JSON.parse(fs.readFileSync(manifestPath, "utf8"));
  const diffResults = fs.existsSync(diffResultsPath)
    ? JSON.parse(fs.readFileSync(diffResultsPath, "utf8"))
    : { against: null, results: [] };

  const resultsByKey = new Map(diffResults.results.map(result => [result.key, result]));
  const themeNames = Object.keys(config.themes);
  const viewportNames = config.viewports.map(viewport => viewport.name);

  const { combos, pages, pageOrderByAlternative } = bucketJobs(manifest.jobs, resultsByKey, diffResults.against);
  const showAlternativeHeadings = pageOrderByAlternative.size > 1;

  // The report is regenerated fresh each run; current/baseline/diff PNGs are left alone.
  fs.rmSync(path.join(config.outputDir, "pages"), { recursive: true, force: true });

  const readmePath = path.join(config.outputDir, "README.md");
  const branchTitle = args.branch || `(uncommitted run — would be ${generateBranchName()})`;

  writeGalleries(config.outputDir, readmePath, combos);
  writePageReports(config.outputDir, readmePath, pages, pageOrderByAlternative, showAlternativeHeadings);
  writeReadme(config.outputDir, readmePath, branchTitle, manifest, diffResults, {
    themeNames,
    viewportNames,
    combos,
    pages,
    pageOrderByAlternative,
    showAlternativeHeadings,
  });

  console.log(`Report written to ${readmePath}`);
}

function bucketJobs(jobs, resultsByKey, against) {
  const combos = new Map(); // combo -> { combo, viewportName, themeName, alternativeName, items: [] }
  const pages = new Map(); // "alternative::page" -> { alternativeName, page, title, entries: [] }
  const pageOrderByAlternative = new Map(); // alternativeName -> [page, ...] in first-seen order

  for (const job of jobs) {
    const diffResult = resultsByKey.get(job.key) ?? {
      status: job.error ? "capture-failed" : against ? "not-diffed" : "first-run",
    };

    if (!combos.has(job.combo)) {
      combos.set(job.combo, {
        combo: job.combo,
        viewportName: job.viewport,
        themeName: job.theme,
        alternativeName: job.alternative,
        items: [],
      });
    }
    if (job.file) {
      combos.get(job.combo).items.push({
        page: job.page,
        title: job.title,
        currentFile: job.file,
        diffFile: diffResult.diffFile ?? null,
      });
    }

    const pageKey = `${job.alternative}::${job.page}`;
    if (!pages.has(pageKey)) {
      pages.set(pageKey, { alternativeName: job.alternative, page: job.page, title: job.title, entries: [] });
    }
    pages.get(pageKey).entries.push({
      viewportName: job.viewport,
      themeName: job.theme,
      currentFile: job.file ?? null,
      status: diffResult.status,
      reason: diffResult.reason ?? null,
      diffPercentage: diffResult.diffPercentage ?? null,
      diffCount: diffResult.diffCount ?? null,
      totalPixels: diffResult.totalPixels ?? null,
      baselineFile: diffResult.baselineFile ?? null,
      diffFile: diffResult.diffFile ?? null,
      error: job.error ?? null,
    });

    if (!pageOrderByAlternative.has(job.alternative)) {
      pageOrderByAlternative.set(job.alternative, []);
    }
    const order = pageOrderByAlternative.get(job.alternative);
    if (!order.includes(job.page)) {
      order.push(job.page);
    }
  }

  return { combos, pages, pageOrderByAlternative };
}

function writeGalleries(outputDir, readmePath, combos) {
  const galleryTemplate = fs.readFileSync(path.join(__dirname, "templates", "gallery.mustache"), "utf8");

  for (const comboData of combos.values()) {
    const sortedItems = [...comboData.items].sort((a, b) => a.title.localeCompare(b.title));

    const currentReadmePath = path.join(outputDir, "current", comboData.combo, "README.md");
    fs.mkdirSync(path.dirname(currentReadmePath), { recursive: true });
    fs.writeFileSync(
      currentReadmePath,
      Mustache.render(galleryTemplate, {
        galleryTitle: `${comboData.viewportName} / ${comboData.themeName} / ${comboData.alternativeName} — Current`,
        backLink: relHref(currentReadmePath, readmePath),
        items: sortedItems.map(item => ({
          title: item.title,
          image: relHref(currentReadmePath, path.join(outputDir, "current", item.currentFile)),
        })),
      })
    );

    const changedItems = sortedItems.filter(item => item.diffFile);
    if (changedItems.length > 0) {
      const diffReadmePath = path.join(outputDir, "diff", comboData.combo, "README.md");
      fs.mkdirSync(path.dirname(diffReadmePath), { recursive: true });
      fs.writeFileSync(
        diffReadmePath,
        Mustache.render(galleryTemplate, {
          galleryTitle: `${comboData.viewportName} / ${comboData.themeName} / ${comboData.alternativeName} — Diff`,
          backLink: relHref(diffReadmePath, readmePath),
          items: changedItems.map(item => ({
            title: item.title,
            image: relHref(diffReadmePath, path.join(outputDir, item.diffFile)),
          })),
        })
      );
    }
  }
}

function writePageReports(outputDir, readmePath, pages, pageOrderByAlternative, showAlternativeHeadings) {
  for (const [alternativeName, pageOrder] of pageOrderByAlternative) {
    pageOrder.forEach((page, index) => {
      const pageData = pages.get(`${alternativeName}::${page}`);
      const pagePath = path.join(outputDir, "pages", alternativeName, `${page}.md`);
      const hrefForIndex = targetIndex =>
        relHref(pagePath, path.join(outputDir, "pages", alternativeName, `${pageOrder[targetIndex]}.md`));
      const indexHref = relHref(pagePath, readmePath);
      const pagination = buildPaginationLine(indexHref, pageOrder, index, hrefForIndex);

      const combosForPage = [...pageData.entries]
        .sort((a, b) => a.viewportName.localeCompare(b.viewportName) || a.themeName.localeCompare(b.themeName))
        .map(entry => ({
          viewport: entry.viewportName,
          theme: entry.themeName,
          status: entry.status,
          reason: entry.reason,
          diffPercentage: entry.diffPercentage,
          diffCount: entry.diffCount,
          totalPixels: entry.totalPixels,
          error: entry.error,
          currentImage: entry.currentFile
            ? relHref(pagePath, path.join(outputDir, "current", entry.currentFile))
            : null,
          baselineImage: entry.baselineFile ? relHref(pagePath, path.join(outputDir, entry.baselineFile)) : null,
          diffImage: entry.diffFile ? relHref(pagePath, path.join(outputDir, entry.diffFile)) : null,
        }));

      fs.mkdirSync(path.dirname(pagePath), { recursive: true });
      fs.writeFileSync(
        pagePath,
        renderPageMarkdown({
          pageTitle: pageData.title,
          alternativeName,
          showAlternativeHeadings,
          pagination,
          combos: combosForPage,
        })
      );
    });
  }
}

function renderPageMarkdown({ pageTitle, alternativeName, showAlternativeHeadings, pagination, combos }) {
  const sections = combos.map(combo => {
    const lines = [`## ${combo.viewport} / ${combo.theme}`, ""];

    const statusLine = formatStatusLine(combo);
    if (statusLine) {
      lines.push(statusLine, "");
    }

    lines.push(combo.currentImage ? `![current](${combo.currentImage})` : "_none_", "");

    if (combo.baselineImage) {
      lines.push("**Baseline**", "", `![baseline](${combo.baselineImage})`, "");
    }

    if (combo.diffImage) {
      lines.push("**Diff**", "", `![diff](${combo.diffImage})`, "");
    }

    while (lines[lines.length - 1] === "") {
      lines.pop();
    }
    return lines.join("\n");
  });

  const heading = showAlternativeHeadings
    ? `# ${pageTitle}<small> &mdash; ${alternativeName}</small>`
    : `# ${pageTitle}`;

  return [heading, "", pagination, "", sections.join("\n\n---\n\n"), "", pagination, ""].join("\n");
}

function formatStatusLine(combo) {
  switch (combo.status) {
    case "unchanged":
      return null;
    case "changed":
      if (combo.reason === "pixel-diff") {
        const percentage =
          combo.diffPercentage != null ? Number(combo.diffPercentage).toFixed(2) : "?";
        const total = combo.totalPixels ?? "?";
        return `⚠️ ${percentage}% pixels (${combo.diffCount} of ${total}) changed`;
      }
      if (combo.reason === "layout-diff") {
        return "⚠️ layout size changed";
      }
      return "⚠️ changed";
    case "added":
      return "🆕 added (no baseline)";
    case "removed":
      return "🗑️ removed (no longer captured)";
    case "capture-failed":
      return `❌ capture failed${combo.error ? `: ${combo.error}` : ""}`;
    case "error":
      return `❌ compare error${combo.reason ? `: ${combo.reason}` : ""}`;
    case "first-run":
      return "first run (no baseline yet)";
    case "not-diffed":
      return "not diffed";
    default:
      return null;
  }
}

function writeReadme(outputDir, readmePath, branchTitle, manifest, diffResults, context) {
  const { themeNames, viewportNames, combos, pages, pageOrderByAlternative, showAlternativeHeadings } = context;

  const summary = {
    total: manifest.jobs.length,
    unchanged: diffResults.results.filter(result => result.status === "unchanged").length,
    changed: diffResults.results.filter(result => result.status === "changed").length,
    added: diffResults.results.filter(result => result.status === "added").length,
    removed: diffResults.results.filter(result => result.status === "removed").length,
    errors: manifest.jobs.filter(job => job.error).length,
  };

  const alternativeNames = [...pageOrderByAlternative.keys()];

  const alternatives = alternativeNames.map(alternativeName => {
    const screenshotsTable = renderScreenshotsTable(viewportNames, themeNames, (viewportName, themeName) => {
      const combo = [viewportName, themeName, alternativeName].map(slugify).join("_");
      const comboData = combos.get(combo);
      if (!comboData) {
        return null;
      }
      const hasDiff = comboData.items.some(item => item.diffFile);
      return {
        current: relHref(readmePath, path.join(outputDir, "current", combo, "README.md")),
        diff: hasDiff ? relHref(readmePath, path.join(outputDir, "diff", combo, "README.md")) : null,
      };
    });

    const pageOrder = pageOrderByAlternative.get(alternativeName) ?? [];
    const pageRows = pageOrder.map(page => {
      const pageData = pages.get(`${alternativeName}::${page}`);
      const hasDiff = pageData.entries.some(entry => entry.status === "changed");
      return {
        title: pageData.title,
        link: relHref(readmePath, path.join(outputDir, "pages", alternativeName, `${page}.md`)),
        linkText: hasDiff ? "diff" : "open",
      };
    });
    const pagesTable = renderPagesTable(themeNames, pageRows);

    return { name: alternativeName, screenshotsTable, pagesTable };
  });

  const parsedAgainst = diffResults.against ? parseBranchTimestamp(diffResults.against) : null;

  const readmeTemplate = fs.readFileSync(path.join(__dirname, "templates", "readme.mustache"), "utf8");
  fs.writeFileSync(
    readmePath,
    Mustache.render(readmeTemplate, {
      title: branchTitle,
      against: diffResults.against,
      agoText: parsedAgainst ? humanizeAgo(parsedAgainst) : null,
      summary,
      showAlternativeHeadings,
      alternatives,
    })
  );
}

function renderScreenshotsTable(viewportNames, themeNames, lookupCell) {
  const header = `| | ${themeNames.join(" | ")} |`;
  const divider = `| --- | ${themeNames.map(() => "---").join(" | ")} |`;
  const rows = viewportNames.map(viewportName => {
    const cells = themeNames.map(themeName => {
      const cell = lookupCell(viewportName, themeName);
      if (!cell) {
        return "—";
      }
      const current = `[current](${cell.current})`;
      const diff = cell.diff ? `[diff](${cell.diff})` : "—";
      return `${current} · ${diff}`;
    });
    return `| ${viewportName} | ${cells.join(" | ")} |`;
  });
  return [header, divider, ...rows].join("\n");
}

function renderPagesTable(themeNames, pageRows) {
  const header = `| | ${themeNames.join(" | ")} |`;
  const divider = `| --- | ${themeNames.map(() => "---").join(" | ")} |`;
  const rows = pageRows.map(row => {
    const cells = themeNames.map(() => `[${row.linkText}](${row.link})`);
    return `| ${row.title} | ${cells.join(" | ")} |`;
  });
  return [header, divider, ...rows].join("\n");
}

// Index [Previous] 1 2 [3] 4 [Next] — current page index not a link.
function buildPaginationLine(indexHref, pageOrder, currentIndex, hrefForIndex) {
  const parts = [`[Index](${indexHref})`];
  parts.push(currentIndex > 0 ? `[Previous](${hrefForIndex(currentIndex - 1)})` : "Previous");
  pageOrder.forEach((_, index) => {
    parts.push(index === currentIndex ? `**${index + 1}**` : `[${index + 1}](${hrefForIndex(index)})`);
  });
  parts.push(currentIndex < pageOrder.length - 1 ? `[Next](${hrefForIndex(currentIndex + 1)})` : "Next");
  return parts.join(" ");
}

function relHref(fromAbsFile, toAbsPath) {
  const rel = path.relative(path.dirname(fromAbsFile), toAbsPath);
  return rel.split(path.sep).join("/");
}
