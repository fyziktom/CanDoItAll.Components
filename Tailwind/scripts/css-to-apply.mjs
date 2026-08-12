// Rewrites plain CSS declarations into Tailwind `@apply` utilities in place,
// using css-to-tailwindcss4 (https://www.npmjs.com/package/css-to-tailwindcss4).
// Usage: node scripts/css-to-apply.mjs [dir-or-file ...]   (defaults to ./sandbox)
// Paths are resolved relative to the Tailwind/ package root, not the caller's cwd.
// Directories are walked recursively; node_modules is always skipped.

import { convertCSS } from 'css-to-tailwindcss4';
import { readFileSync, writeFileSync, readdirSync, statSync } from 'node:fs';
import { join, resolve, relative } from 'node:path';

const packageRoot = new URL('..', import.meta.url);
const themeCSS = readFileSync(new URL('foundation/theme.css', packageRoot), 'utf8');

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
    if (result.css !== original) {
        writeFileSync(file, result.css);
        console.log(`updated: ${label}`);
    } else {
        console.log(`no change: ${label}`);
    }
}
