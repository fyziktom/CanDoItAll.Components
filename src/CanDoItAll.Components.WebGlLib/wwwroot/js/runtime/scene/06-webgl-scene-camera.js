import { OrbitControls } from "../../../vendor/OrbitControls.js";
import {
    THREE,
    clamp,
    focusHost,
    projectionModes,
    resolveFiniteNumber,
    resolveObjectPosition,
    resolveObjectSize,
    round,
    toThreeVector
} from "./02-webgl-scene-core.js";

function resolveViewport(host) {
    return {
        width: Math.max(Math.round(host.clientWidth || 1), 1),
        height: Math.max(Math.round(host.clientHeight || 1), 1)
    };
}

export function createCamera(sceneModel, viewport) {
    const projectionMode = sceneModel.camera?.projectionMode || projectionModes.perspective;
    const aspect = viewport.width / Math.max(viewport.height, 1);
    if (projectionMode === projectionModes.orthographic) {
        const camera = new THREE.OrthographicCamera(
            -viewport.width / 2,
            viewport.width / 2,
            viewport.height / 2,
            -viewport.height / 2,
            -1000,
            4000);
        camera.position.set(18, 22, 18);
        return camera;
    }

    const camera = new THREE.PerspectiveCamera(
        clamp(resolveFiniteNumber(sceneModel.camera?.fieldOfView, 48), 24, 72),
        aspect,
        0.1,
        4000);
    camera.position.set(18, 22, 18);
    return camera;
}

export function createControls(camera, domElement, keyTarget, sceneModel) {
    const controls = new OrbitControls(camera, domElement);
    const interaction = sceneModel.interaction || {};
    controls.enableDamping = true;
    controls.dampingFactor = 0.08;
    controls.enablePan = interaction.allowCameraPan !== false;
    controls.enableZoom = interaction.allowCameraZoom !== false;
    controls.enableRotate = interaction.allowCameraOrbit !== false;
    controls.rotateSpeed = 0.72;
    controls.zoomSpeed = 0.9;
    controls.panSpeed = 0.74;
    controls.minDistance = 4;
    controls.maxDistance = 160;
    controls.minPolarAngle = 0.22;
    controls.maxPolarAngle = Math.PI / 2.12;
    controls.listenToKeyEvents(keyTarget);
    return controls;
}

export function syncViewport(state, force = false) {
    const viewport = resolveViewport(state.host);
    const changed = force ||
        viewport.width !== state.viewport.width ||
        viewport.height !== state.viewport.height;
    if (!changed) {
        return false;
    }

    state.viewport = viewport;
    if (state.camera.isPerspectiveCamera) {
        state.camera.aspect = viewport.width / Math.max(viewport.height, 1);
    } else {
        state.camera.left = -viewport.width / 2;
        state.camera.right = viewport.width / 2;
        state.camera.top = viewport.height / 2;
        state.camera.bottom = -viewport.height / 2;
    }

    state.camera.updateProjectionMatrix();
    state.renderer.setSize(viewport.width, viewport.height, false);
    return true;
}

export function applyCameraState(state, cameraState = state.sceneModel.camera || {}) {
    const target = toThreeVector(cameraState.target, { x: 0, y: 0, z: 0 });
    const distance = clamp(resolveFiniteNumber(cameraState.distance, 32), 4, 160);
    const azimuth = resolveFiniteNumber(cameraState.azimuth, -0.78);
    const polar = clamp(resolveFiniteNumber(cameraState.polar, 1.02), 0.22, Math.PI / 2.08);
    const offset = new THREE.Vector3().setFromSpherical(new THREE.Spherical(distance, polar, azimuth));

    state.controls.target.copy(target);
    state.camera.position.copy(target.clone().add(offset));
    state.camera.lookAt(target);
    state.camera.updateProjectionMatrix();
    state.controls.update();
    updateCameraModel(state);
}

export function updateCameraModel(state) {
    const target = state.controls.target;
    const offset = state.camera.position.clone().sub(target);
    const spherical = new THREE.Spherical().setFromVector3(offset);
    const camera = state.sceneModel.camera = state.sceneModel.camera || {};
    camera.target = {
        x: round(target.x),
        y: round(target.y),
        z: round(target.z)
    };
    camera.distance = round(spherical.radius);
    camera.azimuth = round(spherical.theta, 4);
    camera.polar = round(spherical.phi, 4);
    camera.projectionMode = state.camera.isOrthographicCamera
        ? projectionModes.orthographic
        : projectionModes.perspective;
}

export function fitView(state) {
    const bounds = buildObjectBounds(state);
    if (!bounds) {
        return;
    }

    const center = bounds.getCenter(new THREE.Vector3());
    const size = bounds.getSize(new THREE.Vector3());
    const radius = Math.max(size.x, size.y, size.z, 4);
    state.sceneModel.camera = {
        ...(state.sceneModel.camera || {}),
        target: {
            x: round(center.x),
            y: round(center.y),
            z: round(center.z)
        },
        distance: clamp(radius * 1.58, 12, 120),
        azimuth: resolveFiniteNumber(state.sceneModel.camera?.azimuth, -0.78),
        polar: resolveFiniteNumber(state.sceneModel.camera?.polar, 1.02)
    };
    applyCameraState(state, state.sceneModel.camera);
    focusHost(state);
}

export function focusObject(state, objectId) {
    const sceneObject = state.objectLookup.get(objectId);
    if (!sceneObject) {
        return;
    }

    const position = resolveObjectPosition(sceneObject);
    const size = resolveObjectSize(sceneObject);
    state.sceneModel.camera = {
        ...(state.sceneModel.camera || {}),
        target: {
            x: round(position.x),
            y: round(position.y + (size.y / 2)),
            z: round(position.z)
        },
        distance: clamp(Math.max(size.x, size.y, size.z) * 4.6, 8, 46)
    };
    applyCameraState(state, state.sceneModel.camera);
}

export function resetCamera(state) {
    const original = state.initialCamera || {};
    state.sceneModel.camera = {
        ...original
    };
    applyCameraState(state, state.sceneModel.camera);
    if (state.sceneModel.interaction?.fitViewOnCreate !== false) {
        fitView(state);
    }
}

function buildObjectBounds(state) {
    if (!state.sceneModel.objects?.length) {
        return null;
    }

    const bounds = new THREE.Box3();
    for (const sceneObject of state.sceneModel.objects) {
        const position = resolveObjectPosition(sceneObject);
        const size = resolveObjectSize(sceneObject);
        bounds.expandByPoint(new THREE.Vector3(position.x - size.x / 2, position.y, position.z - size.z / 2));
        bounds.expandByPoint(new THREE.Vector3(position.x + size.x / 2, position.y + size.y, position.z + size.z / 2));
    }

    return bounds;
}

