const states = new WeakMap();

const wheelModes = Object.freeze({
  disabled: 0,
  zoom: 1,
  controlKey: 2,
});

const interactiveSelector = [
  'a',
  'button',
  'input',
  'select',
  'textarea',
  '[contenteditable="true"]',
  '[data-cda-zoom-pan-interactive]',
].join(',');

export function initialize(viewport, content, options) {
  if (!viewport || !content) {
    throw new Error('ZoomPanFrame requires both viewport and content elements.');
  }

  destroy(viewport);
  const state = {
    viewport,
    content,
    options: normalizeOptions(options),
    scale: 1,
    x: 0,
    y: 0,
    pointers: new Map(),
    lastPointer: null,
    pinchDistance: null,
    pinchMidpoint: null,
    cleanup: [],
    resizeObserver: null,
  };

  states.set(viewport, state);
  attachListeners(state);
  state.resizeObserver = new ResizeObserver(() => applyTransform(state));
  state.resizeObserver.observe(viewport);
  state.resizeObserver.observe(content);
  applyTransform(state);
}

export function configure(viewport, options) {
  const state = states.get(viewport);
  if (!state) {
    return;
  }

  state.options = normalizeOptions(options);
  state.scale = clamp(state.scale, state.options.minimumZoom, state.options.maximumZoom);
  if (!state.options.enabled) {
    resetState(state);
  }

  applyTransform(state);
}

export function zoomBy(viewport, factor) {
  const state = states.get(viewport);
  if (!state?.options.enabled) {
    return false;
  }

  return zoomAt(state, Number(factor), viewportCenter(state));
}

export function reset(viewport) {
  const state = states.get(viewport);
  if (!state) {
    return;
  }

  resetState(state);
  applyTransform(state);
}

export function destroy(viewport) {
  const state = states.get(viewport);
  if (!state) {
    return;
  }

  for (const cleanup of state.cleanup) {
    cleanup();
  }

  state.resizeObserver?.disconnect();
  state.content.style.removeProperty('transform');
  viewport.removeAttribute('data-cda-zoom');
  viewport.removeAttribute('data-cda-pan-x');
  viewport.removeAttribute('data-cda-pan-y');
  viewport.removeAttribute('data-cda-zoomed');
  viewport.removeAttribute('data-cda-panning');
  states.delete(viewport);
}

function attachListeners(state) {
  const { viewport } = state;
  const onWheel = (event) => handleWheel(state, event);
  const onPointerDown = (event) => handlePointerDown(state, event);
  const onPointerMove = (event) => handlePointerMove(state, event);
  const onPointerUp = (event) => finishPointer(state, event);
  const onKeyDown = (event) => handleKeyDown(state, event);

  viewport.addEventListener('wheel', onWheel, { passive: false });
  viewport.addEventListener('pointerdown', onPointerDown);
  viewport.addEventListener('pointermove', onPointerMove);
  viewport.addEventListener('pointerup', onPointerUp);
  viewport.addEventListener('pointercancel', onPointerUp);
  viewport.addEventListener('keydown', onKeyDown);

  state.cleanup.push(() => {
    viewport.removeEventListener('wheel', onWheel);
    viewport.removeEventListener('pointerdown', onPointerDown);
    viewport.removeEventListener('pointermove', onPointerMove);
    viewport.removeEventListener('pointerup', onPointerUp);
    viewport.removeEventListener('pointercancel', onPointerUp);
    viewport.removeEventListener('keydown', onKeyDown);
  });
}

function handleWheel(state, event) {
  const { options } = state;
  if (!options.enabled || options.wheelMode === wheelModes.disabled) {
    return;
  }

  if (options.wheelMode === wheelModes.controlKey && !event.ctrlKey && !event.metaKey) {
    return;
  }

  if (event.deltaY === 0) {
    return;
  }

  const factor = event.deltaY < 0 ? options.zoomFactor : 1 / options.zoomFactor;
  const changed = zoomAt(state, factor, clientPoint(state, event.clientX, event.clientY));
  if (changed) {
    event.preventDefault();
  }
}

function handlePointerDown(state, event) {
  if (!state.options.enabled || event.button !== 0 || isInteractiveEvent(state, event)) {
    return;
  }

  const point = { x: event.clientX, y: event.clientY };
  state.pointers.set(event.pointerId, point);
  state.viewport.setPointerCapture?.(event.pointerId);
  if (state.pointers.size === 1) {
    state.lastPointer = point;
  } else if (state.pointers.size === 2) {
    const [first, second] = [...state.pointers.values()];
    state.pinchDistance = distance(first, second);
    state.pinchMidpoint = midpoint(first, second);
  }

  if (state.scale > 1 || state.pointers.size > 1) {
    event.preventDefault();
    state.viewport.setAttribute('data-cda-panning', 'true');
  }
}

function handlePointerMove(state, event) {
  if (!state.pointers.has(event.pointerId)) {
    return;
  }

  const point = { x: event.clientX, y: event.clientY };
  state.pointers.set(event.pointerId, point);
  if (state.pointers.size >= 2) {
    const [first, second] = [...state.pointers.values()];
    const nextDistance = distance(first, second);
    const nextMidpoint = midpoint(first, second);
    if (state.pinchDistance && state.pinchMidpoint) {
      zoomAt(
        state,
        nextDistance / Math.max(state.pinchDistance, 1),
        clientPoint(state, state.pinchMidpoint.x, state.pinchMidpoint.y));
      state.x += nextMidpoint.x - state.pinchMidpoint.x;
      state.y += nextMidpoint.y - state.pinchMidpoint.y;
      applyTransform(state);
    }

    state.pinchDistance = nextDistance;
    state.pinchMidpoint = nextMidpoint;
    event.preventDefault();
    state.viewport.setAttribute('data-cda-panning', 'true');
    return;
  }

  if (state.scale <= 1 || !state.lastPointer) {
    state.lastPointer = point;
    return;
  }

  state.x += point.x - state.lastPointer.x;
  state.y += point.y - state.lastPointer.y;
  state.lastPointer = point;
  applyTransform(state);
  event.preventDefault();
  state.viewport.setAttribute('data-cda-panning', 'true');
}

function finishPointer(state, event) {
  if (!state.pointers.delete(event.pointerId)) {
    return;
  }

  if (state.viewport.hasPointerCapture?.(event.pointerId)) {
    state.viewport.releasePointerCapture(event.pointerId);
  }

  const remaining = [...state.pointers.values()];
  state.lastPointer = remaining[0] ?? null;
  state.pinchDistance = null;
  state.pinchMidpoint = null;
  if (remaining.length < 2) {
    state.viewport.removeAttribute('data-cda-panning');
  }
}

function handleKeyDown(state, event) {
  if (!state.options.enabled || isInteractiveEvent(state, event)) {
    return;
  }

  let changed = false;
  switch (event.key) {
    case '+':
    case '=':
      changed = zoomAt(state, state.options.zoomFactor, viewportCenter(state));
      break;
    case '-':
    case '_':
      changed = zoomAt(state, 1 / state.options.zoomFactor, viewportCenter(state));
      break;
    case '0':
      changed = state.scale !== 1 || state.x !== 0 || state.y !== 0;
      resetState(state);
      applyTransform(state);
      break;
    case 'ArrowLeft':
      changed = panBy(state, state.options.keyboardPanStep, 0);
      break;
    case 'ArrowRight':
      changed = panBy(state, -state.options.keyboardPanStep, 0);
      break;
    case 'ArrowUp':
      changed = panBy(state, 0, state.options.keyboardPanStep);
      break;
    case 'ArrowDown':
      changed = panBy(state, 0, -state.options.keyboardPanStep);
      break;
    default:
      return;
  }

  if (changed) {
    event.preventDefault();
  }
}

function zoomAt(state, factor, origin) {
  if (!Number.isFinite(factor) || factor <= 0) {
    return false;
  }

  const nextScale = clamp(
    state.scale * factor,
    state.options.minimumZoom,
    state.options.maximumZoom);
  if (Math.abs(nextScale - state.scale) < Number.EPSILON) {
    return false;
  }

  const ratio = nextScale / state.scale;
  state.x = origin.x - (origin.x - state.x) * ratio;
  state.y = origin.y - (origin.y - state.y) * ratio;
  state.scale = nextScale;
  applyTransform(state);
  return true;
}

function panBy(state, x, y) {
  if (state.scale <= 1) {
    return false;
  }

  const previousX = state.x;
  const previousY = state.y;
  state.x += x;
  state.y += y;
  applyTransform(state);
  return previousX !== state.x || previousY !== state.y;
}

function applyTransform(state) {
  const viewportWidth = state.viewport.clientWidth;
  const viewportHeight = state.viewport.clientHeight;
  const contentWidth = state.content.offsetWidth;
  const contentHeight = state.content.offsetHeight;
  state.x = clampOffset(state.x, viewportWidth, contentWidth, state.scale);
  state.y = clampOffset(state.y, viewportHeight, contentHeight, state.scale);
  state.content.style.transform = `translate3d(${state.x}px, ${state.y}px, 0) scale(${state.scale})`;
  state.viewport.setAttribute('data-cda-zoom', String(state.scale));
  state.viewport.setAttribute('data-cda-pan-x', String(state.x));
  state.viewport.setAttribute('data-cda-pan-y', String(state.y));
  state.viewport.setAttribute('data-cda-zoomed', String(state.scale > 1));
}

function resetState(state) {
  state.scale = 1;
  state.x = 0;
  state.y = 0;
  state.pointers.clear();
  state.lastPointer = null;
  state.pinchDistance = null;
  state.pinchMidpoint = null;
  state.viewport.removeAttribute('data-cda-panning');
}

function normalizeOptions(options) {
  return {
    enabled: options?.enabled !== false,
    suppressContentInteraction: options?.suppressContentInteraction === true,
    wheelMode: Number(options?.wheelMode ?? wheelModes.zoom),
    minimumZoom: Number(options?.minimumZoom ?? 0.25),
    maximumZoom: Number(options?.maximumZoom ?? 8),
    zoomFactor: Number(options?.zoomFactor ?? 1.2),
    keyboardPanStep: Number(options?.keyboardPanStep ?? 48),
  };
}

function isInteractiveEvent(state, event) {
  if (state.options.suppressContentInteraction) {
    return false;
  }

  const path = typeof event.composedPath === 'function' ? event.composedPath() : [];
  return path.some((target) => target instanceof Element && target.matches(interactiveSelector));
}

function clientPoint(state, clientX, clientY) {
  const bounds = state.viewport.getBoundingClientRect();
  return {
    x: clientX - bounds.left,
    y: clientY - bounds.top,
  };
}

function viewportCenter(state) {
  return {
    x: state.viewport.clientWidth / 2,
    y: state.viewport.clientHeight / 2,
  };
}

function clampOffset(value, viewportSize, contentSize, scale) {
  const scaledSize = contentSize * scale;
  if (scaledSize <= viewportSize) {
    return (viewportSize - scaledSize) / 2;
  }

  return clamp(value, viewportSize - scaledSize, 0);
}

function clamp(value, minimum, maximum) {
  return Math.min(maximum, Math.max(minimum, value));
}

function midpoint(first, second) {
  return {
    x: (first.x + second.x) / 2,
    y: (first.y + second.y) / 2,
  };
}

function distance(first, second) {
  return Math.hypot(second.x - first.x, second.y - first.y);
}
