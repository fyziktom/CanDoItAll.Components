import mermaid from './vendor/mermaid.esm.min.mjs';

const states = new WeakMap();
let renderQueue = Promise.resolve();

export async function render(container, dotNetReference, request) {
  const renderTask = renderQueue.then(() => renderCore(container, dotNetReference, request));
  renderQueue = renderTask.catch(() => undefined);
  return renderTask;
}

async function renderCore(container, dotNetReference, request) {
  destroy(container);

  const source = request?.source ?? '';
  const diagramId = request?.diagramId ?? `cda-mermaid-${crypto.randomUUID()}`;

  if (!container) {
    return failure(diagramId, {
      message: 'The Mermaid container element was not available.',
    });
  }

  if (!source.trim()) {
    container.innerHTML = '';
    return {
      succeeded: true,
      diagramId,
      svgElementId: null,
      nodeCount: 0,
      error: null,
    };
  }

  try {
    mermaid.initialize(buildConfig(request?.options, container));
    const svgId = `${diagramId}-svg`;
    const renderResult = await mermaid.render(svgId, source);
    stabilizeMermaidTooltip();
    removeMermaidRenderArtifacts(svgId, container);
    const state = {
      cleanupCallbacks: [],
      guardObserver: null,
      guardTimers: [],
      dotNetReference,
      diagramId,
      svgMarkup: renderResult.svg,
    };
    const installed = installRenderedSvg(container, state);
    renderResult.bindFunctions?.(container);

    if (!installed?.svg) {
      return failure(diagramId, {
        message: 'Mermaid rendered without an SVG element.',
      });
    }

    states.set(container, state);
    scheduleDomGuard(container);

    return {
      succeeded: true,
      diagramId,
      svgElementId: installed.svg.id || null,
      nodeCount: installed.nodes.length,
      error: null,
    };
  } catch (error) {
    container.innerHTML = '';
    removeMermaidRenderArtifacts(`${diagramId}-svg`, container);
    return failure(diagramId, normalizeMermaidError(error, source));
  }
}

export function hasRenderedSvg(container) {
  return Boolean(container?.querySelector('svg[data-cda-mermaid-svg]'));
}

export function destroy(container) {
  const state = states.get(container);
  if (!state) {
    return;
  }

  runCleanupCallbacks(state);
  clearGuardTimers(state);
  disconnectDomGuard(state);
  removeMermaidRenderArtifacts(`${state.diagramId}-svg`, container);
  states.delete(container);
}

function removeMermaidRenderArtifacts(svgId, container) {
  for (const id of [`d${svgId}`, svgId]) {
    const element = document.getElementById(id);
    if (element && !container?.contains(element)) {
      element.remove();
    }
  }
}

// Mermaid appends its shared tooltip directly to <body>. Its default absolute
// positioning can extend the document's scrollable area even while hidden,
// which creates an otherwise empty page scrollbar. Fixed positioning retains
// the tooltip overlay while keeping it out of document overflow calculations.
function stabilizeMermaidTooltip() {
  for (const tooltip of document.querySelectorAll('body > .mermaidTooltip')) {
    tooltip.style.position = 'fixed';
  }
}

function buildConfig(options, container) {
  const baseConfig = {
    startOnLoad: false,
    securityLevel: options?.securityLevel ?? 'strict',
    theme: resolveTheme(options?.theme, container),
    htmlLabels: options?.htmlLabels ?? false,
    flowchart: {
      useMaxWidth: options?.flowchartUseMaxWidth ?? true,
      htmlLabels: options?.htmlLabels ?? false,
    },
    architecture: {
      randomize: options?.architectureRandomize ?? false,
    },
  };

  return deepMerge(baseConfig, options?.additionalConfig ?? {});
}

// A "auto"/unset theme follows the nearest data-ui-theme ancestor's dark/light state directly
// off the attribute (a discrete mode name, not a CSS color, so this doesn't go through the
// shared theme-tokens readTokens helper). An explicit theme name always wins.
function resolveTheme(requestedTheme, container) {
  if (requestedTheme && requestedTheme !== 'auto') {
    return requestedTheme;
  }

  const owner = container?.closest?.('[data-ui-theme]');
  return owner?.getAttribute('data-ui-theme') === 'dark' ? 'dark' : 'default';
}

const themeWatchers = new WeakMap();

// Live-flips the rendered diagram's theme when data-ui-theme changes, via the shared
// window.CanDoItAll.themeTokens module (CLAUDE.md rule 8). No-ops when that module isn't loaded
// (MermaidBodyAssets IncludeThemeTokens="false") — the diagram still resolves the correct theme
// on each render via resolveTheme() above, it just won't live-update without a re-render.
export function watchTheme(container, dotNetReference) {
  if (!container || themeWatchers.has(container) || !window.CanDoItAll?.themeTokens) {
    return;
  }

  const subscription = window.CanDoItAll.themeTokens.watchTheme(container, () => {
    dotNetReference?.invokeMethodAsync('HandleThemeChangedAsync')
      .catch((error) => console.warn('Mermaid theme change callback failed.', error));
  });
  themeWatchers.set(container, subscription);
}

export function unwatchTheme(container) {
  const subscription = themeWatchers.get(container);
  if (!subscription) {
    return;
  }

  subscription.disconnect();
  themeWatchers.delete(container);
}

function deepMerge(target, source) {
  if (!source || typeof source !== 'object') {
    return target;
  }

  for (const [key, value] of Object.entries(source)) {
    if (value && typeof value === 'object' && !Array.isArray(value)) {
      target[key] = deepMerge({ ...(target[key] ?? {}) }, value);
    } else {
      target[key] = value;
    }
  }

  return target;
}

function attachNodeClicks(svg, diagramId, dotNetReference, cleanupCallbacks) {
  const selectors = [
    'g.node',
    '.node',
    'g[id^="flowchart-"]',
    'g[id*="architecture"]',
    'g[class*="node"]',
    'g[class*="service"]',
    'g[class*="junction"]',
  ];
  const candidates = Array.from(new Set([...svg.querySelectorAll(selectors.join(','))]))
    .filter(isNodeCandidate);
  const nodes = candidates.filter(candidate => !candidates.some(
    descendant => descendant !== candidate && candidate.contains(descendant)));

  for (const node of nodes) {
    node.style.cursor = 'pointer';
    node.setAttribute('tabindex', node.getAttribute('tabindex') ?? '0');
    node.setAttribute('role', node.getAttribute('role') ?? 'button');
    node.setAttribute('data-cda-mermaid-node', 'true');
    node.setAttribute('data-cda-zoom-pan-interactive', 'true');
    const clickTargets = getNodeClickTargets(node);

    for (const target of clickTargets) {
      target.setAttribute('data-cda-mermaid-node-target', 'true');
      target.setAttribute('data-cda-zoom-pan-interactive', 'true');
    }

    const onClick = (event) => {
      event.stopPropagation();
      const payload = extractNodePayload(node, diagramId);
      dotNetReference?.invokeMethodAsync('HandleNodeClickedAsync', payload)
        .catch((error) => console.warn('Mermaid node click callback failed.', error));
    };

    const onKeyDown = (event) => {
      if (event.key === 'Enter' || event.key === ' ') {
        event.preventDefault();
        onClick(event);
      }
    };

    for (const target of clickTargets) {
      target.addEventListener('click', onClick);
    }

    node.addEventListener('keydown', onKeyDown);
    cleanupCallbacks.push(() => {
      for (const target of clickTargets) {
        target.removeEventListener('click', onClick);
      }

      node.removeEventListener('keydown', onKeyDown);
    });
  }

  return nodes;
}

function installRenderedSvg(container, state) {
  runCleanupCallbacks(state);
  container.innerHTML = state.svgMarkup;
  const svg = container.querySelector('svg');
  if (!svg) {
    return null;
  }

  svg.setAttribute('data-cda-mermaid-svg', state.diagramId);
  svg.setAttribute('preserveAspectRatio', 'xMidYMid meet');

  const nodes = attachNodeClicks(svg, state.diagramId, state.dotNetReference, state.cleanupCallbacks);
  return { svg, nodes };
}

function scheduleDomGuard(container) {
  const state = states.get(container);
  if (!state?.svgMarkup) {
    return;
  }

  state.guardObserver?.disconnect();
  state.guardObserver = new MutationObserver(() => {
    window.queueMicrotask(() => restoreRenderedSvgIfMissing(container));
  });
  state.guardObserver.observe(container, { childList: true });

  clearGuardTimers(state);
  for (const delay of [0, 50, 250, 1000, 2500]) {
    state.guardTimers.push(window.setTimeout(() => restoreRenderedSvgIfMissing(container), delay));
  }
}

function restoreRenderedSvgIfMissing(container) {
  const state = states.get(container);
  if (!state?.svgMarkup || !container.isConnected || container.querySelector('svg[data-cda-mermaid-svg]')) {
    return;
  }

  installRenderedSvg(container, state);
}

function runCleanupCallbacks(state) {
  for (const cleanup of state.cleanupCallbacks ?? []) {
    cleanup();
  }

  state.cleanupCallbacks = [];
}

function clearGuardTimers(state) {
  for (const timer of state.guardTimers ?? []) {
    window.clearTimeout(timer);
  }

  state.guardTimers = [];
}

function disconnectDomGuard(state) {
  state.guardObserver?.disconnect();
  state.guardObserver = null;
}

function getNodeClickTargets(node) {
  return Array.from(new Set([
    node,
    ...node.querySelectorAll('foreignObject, foreignObject *'),
  ]));
}

function isNodeCandidate(element) {
  const className = element.getAttribute('class') ?? '';
  if (/edge|edgeLabel|label/.test(className) && !/node|service|junction/.test(className)) {
    return false;
  }

  return Boolean(element.id || element.getAttribute('data-id') || element.querySelector('text'));
}

function extractNodePayload(element, diagramId) {
  const elementId = element.id || element.getAttribute('id') || null;
  const dataId = element.getAttribute('data-id') || element.getAttribute('data-node-id');

  return {
    diagramId,
    nodeId: dataId || normalizeNodeId(elementId),
    text: normalizeText(element.textContent),
    svgElementId: elementId,
    tagName: element.tagName,
    className: element.getAttribute('class') || null,
  };
}

function normalizeNodeId(value) {
  if (!value) {
    return null;
  }

  return value
    .replace(/^flowchart-/, '')
    .replace(/^node-/, '')
    .replace(/-\d+$/, '')
    .replace(/^[-_]+|[-_]+$/g, '') || value;
}

function normalizeText(value) {
  const text = (value ?? '').replace(/\s+/g, ' ').trim();
  return text.length > 0 ? text : null;
}

function failure(diagramId, error) {
  return {
    succeeded: false,
    diagramId,
    svgElementId: null,
    nodeCount: 0,
    error,
  };
}

function normalizeMermaidError(error, source) {
  const hash = error?.hash ?? {};
  const loc = hash.loc ?? hash.location ?? error?.location ?? {};
  const rawMessage = error?.str || error?.message || 'Mermaid could not parse this diagram.';
  const message = formatErrorMessage(rawMessage);
  const messageLine = parseMessageLine(rawMessage);
  const line = messageLine
    ?? normalizeLine(loc.first_line, false)
    ?? normalizeLine(loc.start?.line, false)
    ?? normalizeLine(error?.line, false);
  const rawColumn = normalizeColumn(loc.first_column, true) ?? normalizeColumn(loc.start?.character, true) ?? normalizeColumn(error?.column, false);
  const column = normalizeColumnForSource(source, line, rawColumn);
  const expectedTokens = Array.isArray(hash.expected)
    ? hash.expected.map(cleanExpectedToken).filter(Boolean)
    : [];

  return {
    message,
    line,
    column,
    token: hash.token || error?.token || null,
    text: hash.text || error?.text || null,
    excerpt: buildExcerpt(source, line, column),
    expectedTokens,
    raw: error?.stack || safeJson(error),
  };
}

function formatErrorMessage(message) {
  const text = String(message ?? '').trim();
  if (!text) {
    return 'Mermaid could not parse this diagram.';
  }

  if (/parse error/i.test(text)) {
    return 'Mermaid could not parse this diagram.';
  }

  return text.split(/\r?\n/)[0]?.replace(/\s+/g, ' ').trim()
    || 'Mermaid could not parse this diagram.';
}

function parseMessageLine(message) {
  const match = String(message ?? '').match(/\bline\s+(\d+)\b/i);
  if (!match) {
    return null;
  }

  const line = Number(match[1]);
  return Number.isFinite(line) && line > 0 ? line : null;
}

function normalizeLine(value, addOne) {
  if (!Number.isFinite(value)) {
    return null;
  }

  const line = Number(value) + (addOne ? 1 : 0);
  return line > 0 ? line : 1;
}

function normalizeColumn(value, addOne) {
  if (!Number.isFinite(value)) {
    return null;
  }

  const column = Number(value) + (addOne ? 1 : 0);
  return column > 0 ? column : 1;
}

function normalizeColumnForSource(source, line, column) {
  if (!line || !column) {
    return column;
  }

  const text = source.split(/\r?\n/)[line - 1];
  if (text === undefined) {
    return column;
  }

  return Math.min(Math.max(column, 1), text.length + 1);
}

function buildExcerpt(source, line, column) {
  if (!line) {
    return null;
  }

  const lines = source.split(/\r?\n/);
  const text = lines[line - 1];
  if (text === undefined) {
    return null;
  }

  const lineLabel = `${line}: `;
  const caretOffset = lineLabel.length + Math.max((column ?? 1) - 1, 0);
  return `${lineLabel}${text}\n${' '.repeat(caretOffset)}^`;
}

function cleanExpectedToken(value) {
  return String(value ?? '')
    .replace(/^"|"$/g, '')
    .replace(/^'|'$/g, '')
    .trim();
}

function safeJson(value) {
  try {
    return JSON.stringify(value, Object.getOwnPropertyNames(value));
  } catch {
    return String(value);
  }
}
