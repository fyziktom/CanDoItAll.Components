// PeriodicTable: keyboard plumbing only. The elements, the roving tab stop, the matches and the choice stay in .NET;
// this module moves the focus where .NET asks and keeps the keys .NET handles from doing their browser default:
// arrow keys, Home and End must not scroll the page, Space on an option must not scroll it either, and Enter in the
// search field must not submit an enclosing form. Enter and Space on an element button keep their native click.

const elementKeys = new Set(['ArrowLeft', 'ArrowRight', 'ArrowUp', 'ArrowDown', 'Home', 'End']);
const optionKeys = new Set(['ArrowUp', 'ArrowDown', 'Enter', ' ', 'Escape']);
const searchKeys = new Set(['ArrowDown', 'Enter']);
const attached = new WeakSet();

function keysOf(target) {
    if (!(target instanceof Element)) {
        return null;
    }
    if (target.matches('button[data-z]')) {
        return elementKeys;
    }
    if (target.matches('[role="option"]')) {
        return optionKeys;
    }
    if (target.matches('[role="combobox"]')) {
        return searchKeys;
    }
    return null;
}

function onKeyDown(event) {
    if (event.defaultPrevented || event.altKey || event.ctrlKey || event.metaKey || event.isComposing) {
        return;
    }
    const keys = keysOf(event.target);
    if (!keys || !keys.has(event.key) || (keys === searchKeys && event.shiftKey)) {
        return;
    }
    event.preventDefault();
}

export function attach(root) {
    if (!root || attached.has(root)) {
        return;
    }
    root.addEventListener('keydown', onKeyDown);
    attached.add(root);
}

export function detach(root) {
    if (!root || !attached.has(root)) {
        return;
    }
    root.removeEventListener('keydown', onKeyDown);
    attached.delete(root);
}

// Focuses the element inside the table that the selector names (an element button, an option or the search field).
export function focus(root, selector) {
    const target = root ? root.querySelector(selector) : null;
    if (target instanceof HTMLElement) {
        target.focus();
    }
}
