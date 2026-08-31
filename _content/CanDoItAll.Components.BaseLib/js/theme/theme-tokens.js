(function () {
    "use strict";

    const root = window.CanDoItAll = window.CanDoItAll || {};
    const themeAttribute = "data-ui-theme";

    function requireElement(value, name) {
        if (!(value instanceof HTMLElement)) {
            throw new Error(`theme-tokens ${name} must be an HTML element.`);
        }

        return value;
    }

    // Resolves from the passed-in host, never document.documentElement: data-ui-theme
    // can be scoped to any subtree via ThemeHost, so a component must read its own ancestor.
    function readTokens(hostValue, tokenMap) {
        const host = requireElement(hostValue, "host");
        const style = window.getComputedStyle(host);
        const resolved = {};
        for (const propertyName of Object.keys(tokenMap)) {
            const { cssVar, fallback } = tokenMap[propertyName];
            resolved[propertyName] = style.getPropertyValue(cssVar).trim() || fallback;
        }

        return resolved;
    }

    function findThemeOwner(hostValue) {
        const host = requireElement(hostValue, "host");
        return host.closest(`[${themeAttribute}]`) || host.ownerDocument.documentElement;
    }

    // Watches the nearest data-ui-theme ancestor (which may be host itself, or an
    // ancestor stamped by ThemeHost) and invokes onChange whenever it flips.
    function watchTheme(hostValue, onChange) {
        const host = requireElement(hostValue, "host");
        const owner = findThemeOwner(host);
        const observer = new MutationObserver(mutations => {
            for (const mutation of mutations) {
                if (mutation.type === "attributes" && mutation.attributeName === themeAttribute) {
                    onChange();
                    return;
                }
            }
        });

        observer.observe(owner, { attributes: true, attributeFilter: [themeAttribute], subtree: true });

        return {
            disconnect() {
                observer.disconnect();
            }
        };
    }

    // Resolves the nearest data-ui-theme ancestor's current value directly ("light"/"dark"),
    // for callers that need the discrete mode name rather than a CSS color via readTokens.
    function readThemeMode(hostValue) {
        const owner = findThemeOwner(hostValue);
        return owner.getAttribute(themeAttribute) || "light";
    }

    // Bridges watchTheme's JS-only callback to a Blazor component: invokes methodName on
    // dotNetRef whenever the nearest data-ui-theme ancestor changes. Returns the same
    // { disconnect() } handle as watchTheme, capturable as an IJSObjectReference.
    function watchThemeMode(hostValue, dotNetRef, methodName) {
        return watchTheme(hostValue, () => {
            dotNetRef.invokeMethodAsync(methodName);
        });
    }

    root.themeTokens = { readTokens, watchTheme, readThemeMode, watchThemeMode };
})();
