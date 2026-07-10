const ON_LAYOUT_CHANGED = 'OnSideMenuLayoutChanged';
const SMALL_BREAKPOINT = 768;
const LARGE_BREAKPOINT = 1280;
const MINIMUM_CAPACITY = 2;
const MAXIMUM_CAPACITY = 64;
const DISPLAY_MODE_LARGE = 0;
const DISPLAY_MODE_MEDIUM = 1;
const DISPLAY_MODE_SMALL = 2;
const VIEWPORT_PADDING = 12;
const controllers = new Map();

class SideMenuController {
    #root;
    #itemViewport;
    #measureItem;
    #dotNetReference;
    #resizeObserver;
    #animationFrame;
    #lastSignature = '';

    constructor(root, itemViewport, measureItem, dotNetReference) {
        this.#root = root;
        this.#itemViewport = itemViewport;
        this.#measureItem = measureItem;
        this.#dotNetReference = dotNetReference;
        this.#resizeObserver = new ResizeObserver(() => this.#schedulePublish());
        this.#resizeObserver.observe(root);
        this.#resizeObserver.observe(itemViewport);
        this.#resizeObserver.observe(measureItem);
        window.addEventListener('resize', this.#handleWindowResize, { passive: true });
    }

    measure() {
        const width = window.innerWidth;
        const displayMode = width < SMALL_BREAKPOINT
            ? DISPLAY_MODE_SMALL
            : width < LARGE_BREAKPOINT
                ? DISPLAY_MODE_MEDIUM
                : DISPLAY_MODE_LARGE;

        const viewportHeight = this.#itemViewport.clientHeight;
        const itemHeight = Math.max(1, this.#measureItem.getBoundingClientRect().height);
        const computedStyle = getComputedStyle(this.#itemViewport);
        const gap = Number.parseFloat(computedStyle.rowGap || computedStyle.gap || '0') || 0;
        const capacity = displayMode === DISPLAY_MODE_SMALL
            ? MAXIMUM_CAPACITY
            : Math.min(
                MAXIMUM_CAPACITY,
                Math.max(MINIMUM_CAPACITY, Math.floor((viewportHeight + gap) / (itemHeight + gap))));

        return { displayMode, visibleItemCapacity: capacity };
    }

    refresh() {
        this.#schedulePublish(true);
    }

    dispose() {
        this.#resizeObserver.disconnect();
        window.removeEventListener('resize', this.#handleWindowResize);
        if (this.#animationFrame) {
            cancelAnimationFrame(this.#animationFrame);
        }
    }

    #handleWindowResize = () => this.#schedulePublish();

    #schedulePublish(force = false) {
        if (this.#animationFrame) {
            cancelAnimationFrame(this.#animationFrame);
        }

        this.#animationFrame = requestAnimationFrame(async () => {
            this.#animationFrame = 0;
            const metrics = this.measure();
            const signature = `${metrics.displayMode}:${metrics.visibleItemCapacity}`;
            if (!force && signature === this.#lastSignature) {
                return;
            }

            this.#lastSignature = signature;
            try {
                await this.#dotNetReference.invokeMethodAsync(ON_LAYOUT_CHANGED, metrics);
            } catch {
                this.dispose();
            }
        });
    }
}

export function initialize(
    instanceId,
    root,
    itemViewport,
    measureItem,
    dotNetReference,
    storageKey,
    readStoredExpanded) {
    dispose(instanceId);
    const controller = new SideMenuController(root, itemViewport, measureItem, dotNetReference);
    controllers.set(instanceId, controller);

    let storedExpanded = null;
    if (readStoredExpanded) {
        try {
            const storedValue = localStorage.getItem(storageKey);
            storedExpanded = storedValue === 'true' ? true : storedValue === 'false' ? false : null;
        } catch {
            storedExpanded = null;
        }
    }

    return { metrics: controller.measure(), storedExpanded };
}

export function refresh(instanceId) {
    controllers.get(instanceId)?.refresh();
}

export function saveExpanded(storageKey, isExpanded) {
    try {
        localStorage.setItem(storageKey, isExpanded ? 'true' : 'false');
    } catch {
        // Storage may be disabled by browser policy; menu behavior still works in memory.
    }
}

export function positionPanel(panel) {
    if (!(panel instanceof HTMLElement) || !(panel.parentElement instanceof HTMLElement)) {
        return;
    }

    const anchor = panel.parentElement;
    const anchorRect = anchor.getBoundingClientRect();
    const panelHeight = panel.offsetHeight;
    const panelWidth = panel.offsetWidth;
    const maximumTop = Math.max(
        VIEWPORT_PADDING,
        window.innerHeight - VIEWPORT_PADDING - panelHeight);
    const maximumLeft = Math.max(
        VIEWPORT_PADDING,
        window.innerWidth - VIEWPORT_PADDING - panelWidth);
    const viewportTop = Math.min(
        Math.max(VIEWPORT_PADDING, anchorRect.top),
        maximumTop);
    const viewportLeft = Math.min(
        Math.max(VIEWPORT_PADDING, anchorRect.right + VIEWPORT_PADDING),
        maximumLeft);

    // Fixed positioning keeps the panel visible when an application shell clips its
    // contents. The coordinates are clamped to the browser viewport in both axes.
    panel.style.position = 'fixed';
    panel.style.top = `${viewportTop}px`;
    panel.style.left = `${viewportLeft}px`;
    panel.style.bottom = 'auto';
}

export function dispose(instanceId) {
    const controller = controllers.get(instanceId);
    if (!controller) {
        return;
    }

    controller.dispose();
    controllers.delete(instanceId);
}
