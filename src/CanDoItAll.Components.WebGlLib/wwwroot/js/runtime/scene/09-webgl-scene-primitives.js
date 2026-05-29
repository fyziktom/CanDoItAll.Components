import {
    THREE,
    createMaterial,
    primitiveKinds,
    resolveObjectSize,
    resolveString
} from "./02-webgl-scene-core.js";

export function createPrimitiveVisual(asset, sceneObject, options = {}) {
    const primitiveKind = resolveString(asset?.primitiveKind, primitiveKinds.box);
    const color = sceneObject?.color || asset?.color || "#94a3b8";
    const size = resolveObjectSize(sceneObject);
    const group = new THREE.Group();

    switch (primitiveKind) {
        case primitiveKinds.house:
            addHousePrimitive(group, size, color);
            break;
        case primitiveKinds.tree:
            addTreePrimitive(group, size, color);
            break;
        case primitiveKinds.person:
            addPersonPrimitive(group, size, color);
            break;
        case primitiveKinds.marker:
            addMarkerPrimitive(group, size, color, options.symbol);
            break;
        case primitiveKinds.gear:
            addGearPrimitive(group, size, color);
            break;
        case primitiveKinds.sphere:
            addSpherePrimitive(group, size, color);
            break;
        case primitiveKinds.cylinder:
            addCylinderPrimitive(group, size, color);
            break;
        case primitiveKinds.cone:
            addConePrimitive(group, size, color);
            break;
        default:
            addBoxPrimitive(group, size, color);
            break;
    }

    return group;
}

function addBoxPrimitive(group, size, color) {
    const mesh = new THREE.Mesh(
        new THREE.BoxGeometry(size.x, size.y, size.z),
        createMaterial(color, { roughness: 0.52 }));
    mesh.position.y = size.y / 2;
    group.add(mesh);
}

function addHousePrimitive(group, size, color) {
    const bodyHeight = size.y * 0.68;
    const roofHeight = size.y * 0.34;
    const body = new THREE.Mesh(
        new THREE.BoxGeometry(size.x, bodyHeight, size.z),
        createMaterial(color, { roughness: 0.62 }));
    body.position.y = bodyHeight / 2;

    const roof = new THREE.Mesh(
        new THREE.ConeGeometry(Math.max(size.x, size.z) * 0.72, roofHeight, 4),
        createMaterial("#b45309", { roughness: 0.7 }));
    roof.position.y = bodyHeight + roofHeight / 2;
    roof.rotation.y = Math.PI / 4;

    const door = new THREE.Mesh(
        new THREE.BoxGeometry(size.x * 0.22, bodyHeight * 0.46, 0.035),
        createMaterial("#334155", { roughness: 0.82 }));
    door.position.set(0, bodyHeight * 0.23, size.z / 2 + 0.021);
    group.add(body, roof, door);
}

function addTreePrimitive(group, size, color) {
    const trunk = new THREE.Mesh(
        new THREE.CylinderGeometry(size.x * 0.12, size.x * 0.16, size.y * 0.45, 12),
        createMaterial("#7c4a21", { roughness: 0.8 }));
    trunk.position.y = size.y * 0.225;

    const crown = new THREE.Mesh(
        new THREE.ConeGeometry(size.x * 0.54, size.y * 0.74, 18),
        createMaterial(color || "#16a34a", { roughness: 0.7 }));
    crown.position.y = size.y * 0.78;
    group.add(trunk, crown);
}

function addPersonPrimitive(group, size, color) {
    const body = new THREE.Mesh(
        new THREE.CapsuleGeometry(size.x * 0.22, size.y * 0.42, 8, 16),
        createMaterial(color || "#e2e8f0", { roughness: 0.46 }));
    body.position.y = size.y * 0.44;

    const head = new THREE.Mesh(
        new THREE.SphereGeometry(size.x * 0.22, 18, 18),
        createMaterial("#f8d8bd", { roughness: 0.64 }));
    head.position.y = size.y * 0.82;

    const base = new THREE.Mesh(
        new THREE.CylinderGeometry(size.x * 0.34, size.x * 0.34, size.y * 0.04, 22),
        createMaterial("#334155", { transparent: true, opacity: 0.76 }));
    base.position.y = size.y * 0.02;
    group.add(base, body, head);
}

function addMarkerPrimitive(group, size, color, isSymbol) {
    const radius = Math.max(size.x, size.y, size.z) * (isSymbol ? 0.34 : 0.28);
    const halo = new THREE.Mesh(
        new THREE.SphereGeometry(radius * 1.75, 22, 22),
        createMaterial(color, { transparent: true, opacity: 0.16, depthWrite: false }));
    const core = new THREE.Mesh(
        new THREE.SphereGeometry(radius, 28, 28),
        createMaterial(color, { emissive: color, emissiveIntensity: 0.28, roughness: 0.35 }));
    const ring = new THREE.Mesh(
        new THREE.TorusGeometry(radius * 1.45, radius * 0.08, 10, 36),
        createMaterial(color, { transparent: true, opacity: 0.72 }));
    ring.rotation.x = Math.PI / 2;
    group.add(halo, core, ring);
}

function addGearPrimitive(group, size, color) {
    const radius = Math.max(size.x, size.z) * 0.34;
    const torus = new THREE.Mesh(
        new THREE.TorusGeometry(radius, Math.max(0.04, radius * 0.18), 16, 32),
        createMaterial(color, { metalness: 0.2, roughness: 0.42 }));
    torus.position.y = size.y * 0.5;
    torus.rotation.x = Math.PI / 2;
    const hub = new THREE.Mesh(
        new THREE.CylinderGeometry(radius * 0.42, radius * 0.42, size.y * 0.16, 24),
        createMaterial("#f8fafc", { metalness: 0.18, roughness: 0.5 }));
    hub.position.y = size.y * 0.5;
    group.add(torus, hub);
}

function addSpherePrimitive(group, size, color) {
    const mesh = new THREE.Mesh(
        new THREE.SphereGeometry(Math.max(size.x, size.y, size.z) * 0.5, 28, 28),
        createMaterial(color, { roughness: 0.42 }));
    mesh.position.y = size.y / 2;
    group.add(mesh);
}

function addCylinderPrimitive(group, size, color) {
    const mesh = new THREE.Mesh(
        new THREE.CylinderGeometry(size.x / 2, size.x / 2, size.y, 28),
        createMaterial(color, { roughness: 0.52 }));
    mesh.position.y = size.y / 2;
    group.add(mesh);
}

function addConePrimitive(group, size, color) {
    const mesh = new THREE.Mesh(
        new THREE.ConeGeometry(size.x / 2, size.y, 28),
        createMaterial(color, { roughness: 0.58 }));
    mesh.position.y = size.y / 2;
    group.add(mesh);
}
