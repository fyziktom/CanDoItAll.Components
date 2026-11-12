(function () {
    "use strict";

    const root = window.CanDoItAll = window.CanDoItAll || {};

    // Maps to Tailwind/theme.css's --ui-charts-* tokens (CLAUDE.md rule 9). ApexCharts
    // consumes plain color strings baked into its options at construction time rather than
    // reading CSS custom properties itself, so these are resolved here (via BaseLib's
    // theme-tokens.js) and handed back to CdaChart.razor through JS interop instead of being
    // applied through CSS. Fallback literals mirror theme.css's own declared values.
    const chartsColorTokenMap = {
        series1: { cssVar: "--ui-charts-series-1", fallback: "#0f766e" },
        series2: { cssVar: "--ui-charts-series-2", fallback: "#2563eb" },
        series3: { cssVar: "--ui-charts-series-3", fallback: "#dc2626" },
        series4: { cssVar: "--ui-charts-series-4", fallback: "#d97706" },
        series5: { cssVar: "--ui-charts-series-5", fallback: "#7c3aed" },
        series6: { cssVar: "--ui-charts-series-6", fallback: "#0891b2" },
        series7: { cssVar: "--ui-charts-series-7", fallback: "#16a34a" },
        series8: { cssVar: "--ui-charts-series-8", fallback: "#db2777" },
        gridStripe: { cssVar: "--ui-charts-grid-stripe", fallback: "#f8fafc" },
        legendText: { cssVar: "--ui-charts-legend-text", fallback: "#0f172a" },
        strokeDefault: { cssVar: "--ui-charts-stroke-default", fallback: "#64748b" }
    };

    // Falls back to an inline getComputedStyle read when BaseLib's shared theme-tokens.js
    // module isn't loaded, so Charts degrades gracefully instead of throwing when used
    // without <ChartsBodyAssets /> or with IncludeThemeTokens="false".
    function readTokensInline(host, tokenMap) {
        const style = window.getComputedStyle(host);
        const resolved = {};
        for (const propertyName of Object.keys(tokenMap)) {
            const { cssVar, fallback } = tokenMap[propertyName];
            resolved[propertyName] = style.getPropertyValue(cssVar).trim() || fallback;
        }

        return resolved;
    }

    // ApexCharts' tooltip (both the hover tooltip and the x-axis crosshair label) is styled by
    // a discrete "light"/"dark" mode passed into its own options, not by CSS custom properties,
    // so — mirroring CLAUDE.md rule 11's "auto" pattern — this reads the nearest data-ui-theme
    // ancestor's attribute directly instead of resolving it as a color token.
    function resolveIsDark(host) {
        const owner = host.closest("[data-ui-theme]") || host.ownerDocument.documentElement;
        return owner.getAttribute("data-ui-theme") === "dark";
    }

    function resolveTokens(host) {
        const tokens = root.themeTokens
            ? root.themeTokens.readTokens(host, chartsColorTokenMap)
            : readTokensInline(host, chartsColorTokenMap);
        tokens.isDark = resolveIsDark(host) ? "true" : "false";
        return tokens;
    }

    const watchers = new WeakMap();

    function watchTheme(host, dotNetReference) {
        if (!root.themeTokens) {
            return;
        }

        unwatchTheme(host);
        const subscription = root.themeTokens.watchTheme(host, () => {
            dotNetReference?.invokeMethodAsync("HandleChartThemeChangedAsync");
        });
        watchers.set(host, subscription);
    }

    function unwatchTheme(host) {
        const subscription = watchers.get(host);
        if (subscription) {
            subscription.disconnect();
            watchers.delete(host);
        }
    }

    root.charts = { resolveTokens, watchTheme, unwatchTheme };
})();
