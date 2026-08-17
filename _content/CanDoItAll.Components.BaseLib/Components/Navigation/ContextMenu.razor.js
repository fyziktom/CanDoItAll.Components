const VIEWPORT_PADDING = 16;
const controllers = new Map();

export function openMenu(menu, instanceId, requestedX, requestedY) {
    closeMenu(instanceId);
    controllers.set(instanceId, { previousActiveElement: document.activeElement });
    positionMenu(menu, requestedX, requestedY);
    menu.focus({ preventScroll: true });
}

export function positionMenu(menu, requestedX, requestedY) {
    menu.style.left = `${Math.max(VIEWPORT_PADDING, requestedX)}px`;
    menu.style.top = `${Math.max(VIEWPORT_PADDING, requestedY)}px`;

    const rect = menu.getBoundingClientRect();
    const left = Math.min(
        Math.max(VIEWPORT_PADDING, requestedX),
        Math.max(VIEWPORT_PADDING, window.innerWidth - VIEWPORT_PADDING - rect.width));
    const top = Math.min(
        Math.max(VIEWPORT_PADDING, requestedY),
        Math.max(VIEWPORT_PADDING, window.innerHeight - VIEWPORT_PADDING - rect.height));

    menu.style.left = `${left}px`;
    menu.style.top = `${top}px`;
}

export function closeMenu(instanceId) {
    const controller = controllers.get(instanceId);
    if (!controller) {
        return;
    }

    controllers.delete(instanceId);
    const { previousActiveElement } = controller;
    if (previousActiveElement instanceof HTMLElement && previousActiveElement.isConnected) {
        previousActiveElement.focus({ preventScroll: true });
    }
}
