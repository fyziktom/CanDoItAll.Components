// WorkspaceTabStrip: geometry helpers only. Tab state, selection and commands stay in .NET.

function triggerOf(root, trigger, tabId) {
    if (!root) {
        return null;
    }
    if (trigger === 'tab') {
        for (const tab of root.querySelectorAll('[data-tab-id]')) {
            if (tab.getAttribute('data-tab-id') === tabId) {
                return tab.querySelector('.ui-workspace-tab__menu-trigger') ?? tab;
            }
        }
        return null;
    }
    return root.querySelector(`[data-menu="${trigger === 'overview' ? 'overview' : 'recent'}"]`);
}

// Viewport position under the control that opened a menu, so keyboard activation places it like a click.
export function anchorOf(root, trigger, tabId) {
    const element = triggerOf(root, trigger, tabId);
    if (!element) {
        return null;
    }
    const rect = element.getBoundingClientRect();
    return [Math.round(rect.left), Math.round(rect.bottom + 4)];
}

// Scrolls the strip (never the page) so the shown tab is visible.
export function reveal(root, tabId) {
    if (!root || !tabId) {
        return;
    }
    const list = root.querySelector('.ui-workspace-tabs__list');
    let target = null;
    for (const tab of root.querySelectorAll('[data-tab-id]')) {
        if (tab.getAttribute('data-tab-id') === tabId) {
            target = tab;
            break;
        }
    }
    if (!list || !target) {
        return;
    }
    const listRect = list.getBoundingClientRect();
    const tabRect = target.getBoundingClientRect();
    if (tabRect.left < listRect.left) {
        list.scrollLeft -= listRect.left - tabRect.left + 8;
    } else if (tabRect.right > listRect.right) {
        list.scrollLeft += tabRect.right - listRect.right + 8;
    }
}
