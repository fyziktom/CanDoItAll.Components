// Rewrites plain CSS declarations into Tailwind `@apply` utilities in place,
// using css-to-tailwindcss4 (https://www.npmjs.com/package/css-to-tailwindcss4).
// Usage: node scripts/css-to-apply.mjs [dir-or-file ...]   (defaults to ./sandbox)
// Paths are resolved relative to the Tailwind/ package root, not the caller's cwd.
// Directories are walked recursively; node_modules is always skipped.
// `@apply` lines longer than .prettierrc.json's printWidth are split into one
// `@apply` per variant group (sm:, md:, hover:, ...) so long conversions like
// several stacked responsive variants don't collapse into one unreadable line.

import { convertCSS } from 'css-to-tailwindcss4';
import { readFileSync, writeFileSync, readdirSync, statSync } from 'node:fs';
import { join, resolve, relative } from 'node:path';

const packageRoot = new URL('..', import.meta.url);
const themeCSS = readFileSync(new URL('theme.css', packageRoot), 'utf8');
const { printWidth = 80 } = JSON.parse(readFileSync(new URL('.prettierrc.json', packageRoot), 'utf8'));

// Group key = the variant chain before the utility itself, e.g. "sm:" or
// "dark:hover:". Brackets/parens are excluded from the scan since arbitrary
// values (col-[...], bg-(--brand)) can contain colons that aren't variants.
function variantGroupKey(className) {
    const bracketIndex = className.search(/[[(]/);
    const prefix = bracketIndex === -1 ? className : className.slice(0, bracketIndex);
    const lastColon = prefix.lastIndexOf(':');
    return lastColon === -1 ? '' : prefix.slice(0, lastColon + 1);
}

function splitLongApplyLines(css) {
    return css.replace(/^([ \t]*)@apply\s+([^;]+);/gm, (match, indent, classList) => {
        if (match.length <= printWidth) return match;

        const groups = new Map();
        for (const className of classList.trim().split(/\s+/)) {
            const key = variantGroupKey(className);
            if (!groups.has(key)) groups.set(key, []);
            groups.get(key).push(className);
        }
        if (groups.size <= 1) return match;

        return [...groups.values()].map((classes) => `${indent}@apply ${classes.join(' ')};`).join('\n');
    });
}

function walkDir(dir) {
    return readdirSync(dir, { withFileTypes: true }).flatMap((entry) => {
        if (entry.name === 'node_modules' || entry.name.startsWith('.')) return [];
        const full = join(dir, entry.name);
        if (entry.isDirectory()) return walkDir(full);
        return entry.name.endsWith('.css') ? [full] : [];
    });
}

function collectCssFiles(target) {
    const resolved = resolve(packageRoot.pathname, target);
    let stat;
    try {
        stat = statSync(resolved);
    } catch {
        console.error(`error: no such file or directory: ${target}`);
        process.exit(1);
    }
    return stat.isDirectory() ? walkDir(resolved) : [resolved];
}

const targets = process.argv.slice(2);
const files = (targets.length > 0 ? targets : ['sandbox']).flatMap(collectCssFiles);

for (const file of files) {
    const label = relative(packageRoot.pathname, file);
    const original = readFileSync(file, 'utf8');
    const result = await convertCSS(original, { themeCSS });
    const output = splitLongApplyLines(result.css);
    if (output !== original) {
        writeFileSync(file, output);
        console.log(`updated: ${label}`);
    } else {
        console.log(`no change: ${label}`);
    }
}
