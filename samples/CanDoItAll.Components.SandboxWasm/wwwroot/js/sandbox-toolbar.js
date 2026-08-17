export function observeToolbarHeight(selector) {
    const element = document.querySelector(selector);
    if (!element) {
        return;
    }

    const setHeight = () => {
        document.documentElement.style.setProperty(
            '--sandbox-toolbar-height', `${element.offsetHeight}px`);
    };

    setHeight();
    new ResizeObserver(setHeight).observe(element);
}

export function scrollToAnchor(anchor) {
    document.getElementById(anchor)?.scrollIntoView({ block: 'start' });
}

export function scrollToTop() {
    window.scrollTo({ top: 0 });
}

let hashChangeHandler;
let hashClickHandler;

export function observeHashChanges(dotNetReference) {
    const notify = () => dotNetReference.invokeMethodAsync('HandleHashChanged', window.location.hash);

    hashChangeHandler = notify;
    hashClickHandler = event => {
        const link = event.target.closest('a[data-sandbox-route]');
        const href = link?.getAttribute('href');
        if (!href || !href.startsWith('#')) {
            return;
        }

        event.preventDefault();
        window.history.pushState(null, '', href);
        notify();
    };

    window.addEventListener('hashchange', hashChangeHandler);
    document.addEventListener('click', hashClickHandler, true);
}

export function stopObservingHashChanges() {
    if (hashChangeHandler) {
        window.removeEventListener('hashchange', hashChangeHandler);
        hashChangeHandler = undefined;
    }

    if (hashClickHandler) {
        document.removeEventListener('click', hashClickHandler, true);
        hashClickHandler = undefined;
    }
}
