# SB06 Semantic Invariants

- Standard input components must not invoke change callbacks when disabled; read-only text-like controls must not mutate from change events.
- FormField labels must cascade to nested inputs unless the caller provides an explicit accessible name.
- Composite form controls must accept caller attributes/classes without losing their own required component classes.
- Prefix/suffix field affordances must be visible, not overlay-hidden, and must not force horizontal overflow on mobile.
- EntityPicker options must expose literal `aria-selected="true"` / `"false"` values and preserve listbox naming.
- Inputs sandbox examples must be live controls with callbacks, not decorative static markup.
- Browser proof must include at least one long-text mobile run, one disabled-state run, and one interactive open/action run.
