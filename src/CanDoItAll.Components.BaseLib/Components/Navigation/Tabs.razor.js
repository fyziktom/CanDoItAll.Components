// Tabs: keeps the navigation keys of the tab list (the arrows along its orientation, Home, End and Space) from scrolling
// the page; the .NET key handler moves the selection. A listener added by this module instead of an inline attribute, so
// pages with a strict Content Security Policy (no 'unsafe-inline' scripts) keep the behaviour.

const guarded = new WeakSet();

export function guardNavigationKeys(list) {
    if (!list || guarded.has(list)) {
        return;
    }

    guarded.add(list);
    list.addEventListener('keydown', (event) => {
        const vertical = list.getAttribute('aria-orientation') === 'vertical';
        const keys = vertical
            ? ['ArrowUp', 'ArrowDown', 'Home', 'End', ' ']
            : ['ArrowLeft', 'ArrowRight', 'Home', 'End', ' '];
        if (keys.includes(event.key)) {
            event.preventDefault();
        }
    });
}
