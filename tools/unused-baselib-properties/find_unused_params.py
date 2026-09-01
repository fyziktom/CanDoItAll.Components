# Finds candidate unused [Parameter] properties in BaseLib components.
# Run from repo root: python3 tools/unused-baselib-properties/find_unused_params.py
#
# A property is a CANDIDATE if its name (ignoring XML-doc/`//` comments, and
# ignoring its own declaration statement) never appears elsewhere across a
# component's .razor + .razor.cs files. Declaration statements are stripped
# before counting so that `public Shade Shade { get; set; } = Shade.Default;`
# -- where the property name collides with its own type name -- doesn't hide
# behind its own declaration.
#
# This is a lower bound, not a verdict: item/child components (e.g. TabsItem,
# StepsItem, LineSeries, ValueAxis) are legitimately read by a *parent*
# component via CascadingParameter + reflection-style member access
# (item.Icon, tab.Disabled), so every candidate must be manually confirmed by
# grepping the whole repo (BaseLib + samples + tests) for the parent's usage
# and any consumer call sites before removing it. See
# removed-component-properties.txt for the verified list from the run that
# produced this script.
import re, os, sys, glob, json

ROOT = "src/CanDoItAll.Components.BaseLib/Components"

# Captures the full auto-property declaration (attribute through optional
# default-value initializer) so it can be stripped from the text before
# counting real usages.
decl_re = re.compile(
    r'\[Parameter(?:\([^)]*\))?\]\s*(?:\r?\n\s*\[[^\]]*\]\s*)*\r?\n?\s*'
    r'public\s+[\w<>\[\],\?\. ]+?\s+(\w+)\s*\{\s*get;\s*set;\s*\}(?:\s*=\s*[^;]+;)?',
    re.MULTILINE
)

xmldoc_re = re.compile(r'^\s*///.*$', re.MULTILINE)
linecomment_re = re.compile(r'//[^\n]*')


def strip_comments(text):
    text = xmldoc_re.sub('', text)
    text = linecomment_re.sub('', text)
    return text


results = []
files_by_base = {}
for path in glob.glob(os.path.join(ROOT, "**", "*"), recursive=True):
    if not os.path.isfile(path):
        continue
    if path.endswith(".razor") or path.endswith(".razor.cs"):
        base = path
        if base.endswith(".razor.cs"):
            base = base[:-len(".razor.cs")]
        elif base.endswith(".razor"):
            base = base[:-len(".razor")]
        files_by_base.setdefault(base, []).append(path)

for base, paths in sorted(files_by_base.items()):
    combined_raw = ""
    for p in paths:
        with open(p, encoding="utf-8") as f:
            combined_raw += "\n" + f.read()
    combined = strip_comments(combined_raw)

    # Text with every property's own declaration statement removed, so a
    # property whose name matches its type (or another common word) doesn't
    # get "credit" for its own declaration line.
    combined_sans_decls = decl_re.sub('', combined)

    params = []
    for p in paths:
        with open(p, encoding="utf-8") as f:
            content = f.read()
        content_nocomment = strip_comments(content)
        for m in decl_re.finditer(content_nocomment):
            params.append((m.group(1), p))

    for name, declfile in params:
        occ = len(re.findall(r'\b' + re.escape(name) + r'\b', combined_sans_decls))
        if occ == 0:
            results.append({"component": base, "property": name, "file": declfile, "occurrences": occ})

print(json.dumps(results, indent=2))
print(f"\nTotal candidates: {len(results)}", file=sys.stderr)
