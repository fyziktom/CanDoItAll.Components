# SB04 - Components: anchors and target resolution

Renderer-level movement should not require domain code to calculate coordinates.

Add generic anchor support:

- `WebGlSceneObjectAnchor` on scene objects or metadata-backed anchor definitions;
- built-in anchors: `center`, `base`, `top`, `front`, `back`, `left`, `right`, `home`, `work`, `use`, `admin`;
- anchor offset support;
- target resolver in `WebGlRunLib` that can resolve `{ objectId, anchorKey, offset }` to a `WebGlVector3` using a scene snapshot.

Keep this generic:

- no wells;
- no citizens;
- no resource-specific logic.

Important for later examples:

- citizen goes from house to well;
- entrepreneur goes to market;
- administrator sits at desk;
- all of these are just object ids + anchor keys.
