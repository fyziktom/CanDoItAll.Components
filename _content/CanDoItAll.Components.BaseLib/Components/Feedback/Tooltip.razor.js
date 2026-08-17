const VIEWPORT_PADDING = 16;

export function getAnchorPoint(element, tooltipId) {
    if (!element || !element.isConnected) {
        return { x: window.innerWidth / 2, y: window.innerHeight / 2 };
    }

    const focusedElement = element.contains(document.activeElement) ? document.activeElement : element;
    const describedBy = new Set((focusedElement.getAttribute('aria-describedby') ?? '').split(/\s+/).filter(Boolean));
    describedBy.add(tooltipId);
    focusedElement.setAttribute('aria-describedby', [...describedBy].join(' '));

    const rect = focusedElement.getBoundingClientRect();
    return {
        x: rect.left + (rect.width / 2),
        y: rect.top + (rect.height / 2)
    };
}

export function clearFocusedTarget(element, tooltipId) {
    if (!element || !element.isConnected) {
        return;
    }

    for (const target of [element, ...element.querySelectorAll('[aria-describedby]')]) {
        const describedBy = (target.getAttribute('aria-describedby') ?? '')
            .split(/\s+/)
            .filter(id => id && id !== tooltipId);

        if (describedBy.length === 0) {
            target.removeAttribute('aria-describedby');
        } else {
            target.setAttribute('aria-describedby', describedBy.join(' '));
        }
    }
}

export function clampToViewport(element) {
    if (!element || !element.isConnected) {
        return;
    }

    element.style.translate = '';

    const rect = element.getBoundingClientRect();
    let deltaX = 0;
    let deltaY = 0;

    if (rect.left < VIEWPORT_PADDING) {
        deltaX = VIEWPORT_PADDING - rect.left;
    } else if (rect.right > window.innerWidth - VIEWPORT_PADDING) {
        deltaX = window.innerWidth - VIEWPORT_PADDING - rect.right;
    }

    if (rect.top < VIEWPORT_PADDING) {
        deltaY = VIEWPORT_PADDING - rect.top;
    } else if (rect.bottom > window.innerHeight - VIEWPORT_PADDING) {
        deltaY = window.innerHeight - VIEWPORT_PADDING - rect.bottom;
    }

    element.style.translate = `${deltaX}px ${deltaY}px`;
}
