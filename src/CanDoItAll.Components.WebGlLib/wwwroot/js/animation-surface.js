import * as THREE from '../vendor/three.module.min.js';

export { THREE };

/** Additive low-level surface for continuously animated consumer-owned scenes.
 * The consumer owns game state; the surface owns graphics resources and browser lifecycle.
 */
export function createAnimationSurface(canvas, { frame, visibilityChanged, contextChanged, maximumPixelRatio = 1.5 } = {}) {
  const renderer = new THREE.WebGLRenderer({ canvas, antialias: true, alpha: false, powerPreference: 'high-performance' });
  renderer.setPixelRatio(Math.min(devicePixelRatio || 1, maximumPixelRatio));
  renderer.outputColorSpace = THREE.SRGBColorSpace;
  const scene = new THREE.Scene();
  const camera = new THREE.PerspectiveCamera(78, 1, .08, 380);
  const events = new AbortController();
  let disposed = false, running = false, request = 0, previous = 0, lost = false;
  const resize = new ResizeObserver(() => {
    const width = Math.max(1, canvas.clientWidth), height = Math.max(1, canvas.clientHeight);
    renderer.setSize(width, height, false);
    camera.aspect = width / height;
    camera.updateProjectionMatrix();
  });
  resize.observe(canvas);
  function tick(now) {
    if (disposed || !running || lost) return;
    const delta = previous ? Math.min((now - previous) / 1000, .1) : 0;
    previous = now;
    frame?.(delta, now / 1000);
    renderer.render(scene, camera);
    request = requestAnimationFrame(tick);
  }
  function start() {
    if (disposed || running || document.hidden || lost) return;
    running = true;
    previous = 0;
    request = requestAnimationFrame(tick);
  }
  function stop() { running = false; cancelAnimationFrame(request); previous = 0; }
  document.addEventListener('visibilitychange', () => {
    if (document.hidden) stop();
    else start();
    visibilityChanged?.(!document.hidden);
  }, { signal: events.signal });
  canvas.addEventListener('webglcontextlost', event => {
    event.preventDefault(); lost = true; stop(); contextChanged?.(false);
  }, { signal: events.signal });
  canvas.addEventListener('webglcontextrestored', () => {
    lost = false; contextChanged?.(true); start();
  }, { signal: events.signal });
  return {
    renderer, scene, camera, start, stop,
    dispose() {
      if (disposed) return;
      disposed = true;
      stop(); resize.disconnect(); events.abort();
      const geometries = new Set(), materials = new Set(), textures = new Set();
      scene.traverse(object => {
        if (object.geometry) geometries.add(object.geometry);
        for (const material of (Array.isArray(object.material) ? object.material : [object.material])) {
          if (!material) continue;
          materials.add(material);
          for (const value of Object.values(material)) if (value?.isTexture) textures.add(value);
        }
      });
      textures.forEach(value => value.dispose());
      materials.forEach(value => value.dispose());
      geometries.forEach(value => value.dispose());
      scene.clear(); renderer.dispose(); renderer.forceContextLoss();
    }
  };
}
