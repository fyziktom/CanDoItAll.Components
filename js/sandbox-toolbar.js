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
