import fs from "node:fs";
import path from "node:path";

const bundleRoot = process.cwd();
const repoRoot = path.resolve(bundleRoot, "..", "..", "..");
const data = JSON.parse(fs.readFileSync(path.join(bundleRoot, "inventories", "current-state-data.json"), "utf8"));

const requiredTailwindRefs = [
  "Tailwind/input.css",
  "Tailwind/forms/fields.css",
  "Tailwind/controls/buttons.css",
  "Tailwind/navigation/tabs.css",
  "Tailwind/foundation/theme.css",
];

for (const relativePath of requiredTailwindRefs) {
  if (!fs.existsSync(path.join(repoRoot, relativePath))) {
    console.error(`Missing SB02 source reference: ${relativePath}`);
    process.exit(2);
  }
}

const tailwindFiles = data.tailwindFiles ?? [];
const highOrMedium = tailwindFiles.filter((file) => file.severity === "High" || file.severity === "Medium");
if (highOrMedium.length < 5) {
  console.error(`Too few Tailwind review rows for SB02: ${highOrMedium.length}`);
  process.exit(3);
}

console.log(`SB01-INV-001 downstreamTailwindReviewRows=${highOrMedium.length}`);
console.log(`SB01-INV-001 downstreamRequiredTailwindRefs=${requiredTailwindRefs.length}`);
