import fs from "node:fs";
import path from "node:path";

const repoRoot = path.resolve(process.cwd(), "..", "..", "..");
const bundleRoot = process.cwd();
const dataPath = path.join(bundleRoot, "inventories", "current-state-data.json");
const workbookPath = path.join(bundleRoot, "inventories", "standard-components-publishing-map.xlsx");
const previewRoot = path.join(bundleRoot, "reviews", "workbook-previews");
const appComponentsPath = "C:\\repositories\\CanDoItAll\\src\\CanDoItAll.AppComponents\\Components";

function assert(condition, message, code) {
  if (!condition) {
    console.error(message);
    process.exit(code);
  }
}

const data = JSON.parse(fs.readFileSync(dataPath, "utf8"));
const components = data.components ?? [];
const appComponents = data.appComponents ?? [];
const tailwindFiles = data.tailwindFiles ?? [];
const standardMatches = appComponents.filter((item) => item.hasStandardMatch).length;
const workbookStats = fs.statSync(workbookPath);
const previews = fs.readdirSync(previewRoot).filter((name) => name.endsWith(".png"));
const formulaScan = fs.readFileSync(path.join(previewRoot, "formula-error-scan.ndjson"), "utf8");

assert(fs.existsSync(path.join(repoRoot, "README.md")), "repo README missing", 2);
assert(fs.existsSync(path.join(repoRoot, "CanDoItAll.Components.slnx")), "solution missing", 3);
assert(fs.existsSync(path.join(repoRoot, "src", "CanDoItAll.Components.BaseLib")), "BaseLib missing", 4);
assert(fs.existsSync(path.join(repoRoot, "Tailwind")), "Tailwind missing", 5);
assert(fs.existsSync(appComponentsPath), "AppComponents source path missing", 6);
assert(components.length >= 200, `component inventory too small: ${components.length}`, 7);
assert(appComponents.length >= 40, `app component inventory too small: ${appComponents.length}`, 8);
assert(tailwindFiles.length >= 10, `tailwind inventory too small: ${tailwindFiles.length}`, 9);
assert(standardMatches >= 30, `standard duplicate matches too small: ${standardMatches}`, 10);
assert(workbookStats.size > 10000, `workbook is too small: ${workbookStats.size}`, 11);
assert(previews.length >= 6, `not enough workbook previews: ${previews.length}`, 12);
assert(formulaScan.includes("matched 0 entries"), "formula error scan did not prove zero matches", 13);

console.log(`SB01-INV-001 components=${components.length}`);
console.log(`SB01-INV-001 appComponents=${appComponents.length}`);
console.log(`SB01-INV-001 standardMatches=${standardMatches}`);
console.log(`SB01-INV-001 tailwindFiles=${tailwindFiles.length}`);
console.log(`SB01-INV-002 workbookBytes=${workbookStats.size}`);
console.log(`SB01-INV-002 previewCount=${previews.length}`);
console.log("SB01-INV-002 formulaErrorScan=matched 0 entries");
