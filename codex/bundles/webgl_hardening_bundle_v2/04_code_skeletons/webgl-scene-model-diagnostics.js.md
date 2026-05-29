# webgl-scene-model-diagnostics.js skeleton

```js
import { THREE, resolveFiniteNumber } from "./02-webgl-scene-core.js";

export function inspectLoadedModel(asset, template, uri) {
    const diagnostics = {
        assetId: asset?.id || "",
        variantId: asset?.variantId || "",
        uri: uri || asset?.uri || "",
        loaded: !!template,
        meshCount: 0,
        visibleMeshCount: 0,
        transparentMaterialCount: 0,
        boundsMin: { x: 0, y: 0, z: 0 },
        boundsMax: { x: 0, y: 0, z: 0 },
        boundsSize: { x: 0, y: 0, z: 0 },
        warnings: [],
        errors: []
    };

    if (!template) {
        diagnostics.errors.push("GLB did not contain a scene template.");
        return diagnostics;
    }

    template.traverse(child => {
        if (!child.isMesh) {
            return;
        }

        diagnostics.meshCount += 1;
        if (child.visible !== false) {
            diagnostics.visibleMeshCount += 1;
        }

        const materials = Array.isArray(child.material) ? child.material : [child.material];
        for (const material of materials) {
            if (material && material.transparent && resolveFiniteNumber(material.opacity, 1) <= 0.03) {
                diagnostics.transparentMaterialCount += 1;
            }
        }
    });

    if (diagnostics.meshCount === 0) {
        diagnostics.errors.push("Model contains no mesh nodes.");
    }

    if (diagnostics.meshCount > 0 && diagnostics.visibleMeshCount === 0) {
        diagnostics.errors.push("Model contains meshes, but all mesh nodes are invisible.");
    }

    const bounds = new THREE.Box3().setFromObject(template);
    if (bounds.isEmpty()) {
        diagnostics.errors.push("Model bounds are empty.");
        return diagnostics;
    }

    const size = bounds.getSize(new THREE.Vector3());
    diagnostics.boundsMin = toDto(bounds.min);
    diagnostics.boundsMax = toDto(bounds.max);
    diagnostics.boundsSize = toDto(size);

    const largest = Math.max(size.x, size.y, size.z);
    if (!Number.isFinite(largest) || largest <= 0.0001) {
        diagnostics.errors.push("Model bounds are near zero or invalid.");
    } else if (largest > 500 || largest < 0.001) {
        diagnostics.warnings.push(`Model has extreme bounds size: ${largest}. Check unit scale.`);
    }

    return diagnostics;
}

export function addDebugBounds(group, diagnostics, color = "#facc15") {
    const size = diagnostics?.boundsSize || { x: 1, y: 1, z: 1 };
    const geometry = new THREE.BoxGeometry(Math.max(size.x, 0.01), Math.max(size.y, 0.01), Math.max(size.z, 0.01));
    const edges = new THREE.EdgesGeometry(geometry);
    const line = new THREE.LineSegments(edges, new THREE.LineBasicMaterial({ color }));
    line.userData = { debugBounds: true, ownsGeometry: true, ownsMaterial: true };
    group.add(line);
    return line;
}

function toDto(vector) {
    return {
        x: Number(vector.x) || 0,
        y: Number(vector.y) || 0,
        z: Number(vector.z) || 0
    };
}
```
