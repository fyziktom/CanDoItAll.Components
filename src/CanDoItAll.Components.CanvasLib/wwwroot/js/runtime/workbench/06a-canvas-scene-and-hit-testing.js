(function () {
    const root = window.CanDoItAll = window.CanDoItAll || {};
    const shared = root.canvasWorkbenchModule;
    if (!shared) { throw new Error('CanvasLib workbench foundation must load before 06a-canvas-scene-and-hit-testing.js.'); }
    const workbenchInternals = new Proxy({}, {
        get(_target, property) {
            return shared.workbenchInternals?.[property];
        }
    });
    const { clamp, round, hidePopover, getNodeSize, getVisibleNodes, getProjectedNodes, getNodePosition, getHostPoint, worldToHostPoint } = shared;

    function getCanvasRuntimePrimitives() {
        return window.ZyCanvasPrimitives || null;
    }

    function createFallbackHitRegistry() {
        return {
            items: [],
            clear() {
                this.items = [];
            },
            add(bounds, metadata) {
                this.items.push({
                    bounds: {
                        x: bounds?.x || 0,
                        y: bounds?.y || 0,
                        width: bounds?.width || 0,
                        height: bounds?.height || 0
                    },
                    metadata: metadata || {}
                });
            },
            find(pointX, pointY) {
                for (let index = this.items.length - 1; index >= 0; index -= 1) {
                    const item = this.items[index];
                    const bounds = item.bounds;
                    if (pointX >= bounds.x &&
                        pointX <= bounds.x + bounds.width &&
                        pointY >= bounds.y &&
                        pointY <= bounds.y + bounds.height) {
                        return item.metadata;
                    }
                }

                return null;
            }
        };
    }

    function createCanvasHitRegistry() {
        const primitives = getCanvasRuntimePrimitives();
        return primitives?.HitRegistry
            ? new primitives.HitRegistry()
            : createFallbackHitRegistry();
    }

    function createCanvasSurfaceHost(canvas, resizeTarget) {
        const primitives = getCanvasRuntimePrimitives();
        if (primitives?.CanvasSurface) {
            return new primitives.CanvasSurface({
                canvas,
                resizeTarget
            });
        }

        const context = canvas.getContext("2d");
        return {
            canvas,
            context,
            size: {
                width: Math.max(1, Math.round(canvas.getBoundingClientRect().width || 1)),
                height: Math.max(1, Math.round(canvas.getBoundingClientRect().height || 1))
            },
            measure() {
                const rect = (resizeTarget || canvas.parentElement || canvas).getBoundingClientRect();
                const width = Math.max(1, Math.round(rect.width || 1));
                const height = Math.max(1, Math.round(rect.height || 1));
                const ratio = Math.max(1, Math.min(3, window.devicePixelRatio || 1));
                this.size.width = width;
                this.size.height = height;
                canvas.width = Math.round(width * ratio);
                canvas.height = Math.round(height * ratio);
                canvas.style.width = `${width}px`;
                canvas.style.height = `${height}px`;
                context.setTransform(ratio, 0, 0, ratio, 0, 0);
            },
            clear(fillStyle) {
                context.save();
                context.setTransform(1, 0, 0, 1, 0, 0);
                context.clearRect(0, 0, canvas.width, canvas.height);
                context.restore();
                if (fillStyle) {
                    context.save();
                    context.fillStyle = fillStyle;
                    context.fillRect(0, 0, this.size.width, this.size.height);
                    context.restore();
                }
            },
            destroy() {
            }
        };
    }

    function destroyCanvasSurfaceHost(surface) {
        surface?.destroy?.();
    }

    function hexToRgba(color, alpha) {
        if (typeof color !== "string" || color.trim().length === 0) {
            return `rgba(124, 58, 237, ${alpha})`;
        }

        const normalized = color.trim();
        if (normalized.startsWith("rgba(")) {
            return normalized.replace(/rgba\(([^,]+),([^,]+),([^,]+),([^)]+)\)/i, `rgba($1,$2,$3,${alpha})`);
        }

        if (normalized.startsWith("rgb(")) {
            return normalized.replace(/rgb\(([^,]+),([^,]+),([^)]+)\)/i, `rgba($1,$2,$3,${alpha})`);
        }

        const hex = normalized.replace("#", "");
        if (hex.length !== 3 && hex.length !== 6) {
            return normalized;
        }

        const expanded = hex.length === 3
            ? hex.split("").map(token => `${token}${token}`).join("")
            : hex;
        const red = Number.parseInt(expanded.substring(0, 2), 16);
        const green = Number.parseInt(expanded.substring(2, 4), 16);
        const blue = Number.parseInt(expanded.substring(4, 6), 16);
        if (!Number.isFinite(red) || !Number.isFinite(green) || !Number.isFinite(blue)) {
            return normalized;
        }

        return `rgba(${red}, ${green}, ${blue}, ${alpha})`;
    }

    function parseColorChannels(color) {
        if (typeof color !== "string" || color.trim().length === 0) {
            return { red: 124, green: 58, blue: 237 };
        }

        const normalized = color.trim();
        if (normalized.startsWith("rgba(") || normalized.startsWith("rgb(")) {
            const values = normalized
                .replace(/rgba?\(/i, "")
                .replace(")", "")
                .split(",")
                .map(token => Number.parseFloat(token.trim()));
            if (values.length >= 3 &&
                Number.isFinite(values[0]) &&
                Number.isFinite(values[1]) &&
                Number.isFinite(values[2])) {
                return {
                    red: clamp(Math.round(values[0]), 0, 255),
                    green: clamp(Math.round(values[1]), 0, 255),
                    blue: clamp(Math.round(values[2]), 0, 255)
                };
            }
        }

        const hex = normalized.replace("#", "");
        if (hex.length === 3 || hex.length === 6) {
            const expanded = hex.length === 3
                ? hex.split("").map(token => `${token}${token}`).join("")
                : hex;
            const red = Number.parseInt(expanded.substring(0, 2), 16);
            const green = Number.parseInt(expanded.substring(2, 4), 16);
            const blue = Number.parseInt(expanded.substring(4, 6), 16);
            if (Number.isFinite(red) && Number.isFinite(green) && Number.isFinite(blue)) {
                return { red, green, blue };
            }
        }

        return { red: 124, green: 58, blue: 237 };
    }

    function mixColorChannels(base, target, ratio) {
        const normalizedRatio = clamp(ratio, 0, 1);
        return {
            red: Math.round(base.red + ((target.red - base.red) * normalizedRatio)),
            green: Math.round(base.green + ((target.green - base.green) * normalizedRatio)),
            blue: Math.round(base.blue + ((target.blue - base.blue) * normalizedRatio))
        };
    }

    function rgbaFromChannels(channels, alpha) {
        return `rgba(${channels.red}, ${channels.green}, ${channels.blue}, ${alpha})`;
    }

    function buildAccentPalette(accent, isSelected) {
        const accentChannels = parseColorChannels(accent);
        const whiteMix = ratio => mixColorChannels(accentChannels, { red: 255, green: 255, blue: 255 }, ratio);
        const darkMix = ratio => mixColorChannels(accentChannels, { red: 15, green: 23, blue: 42 }, ratio);

        return {
            surfaceFill: rgbaFromChannels(whiteMix(0.84), 0.98),
            surfaceStroke: rgbaFromChannels(whiteMix(isSelected ? 0.08 : 0.36), 0.92),
            surfaceShadow: rgbaFromChannels(accentChannels, 0.16),
            labelText: rgbaFromChannels(darkMix(0.12), 0.72),
            titleText: rgbaFromChannels(darkMix(0.22), 0.96),
            secondaryText: rgbaFromChannels(darkMix(0.16), 0.84),
            iconFill: rgbaFromChannels(whiteMix(0.94), 0.84),
            iconStroke: rgbaFromChannels(whiteMix(0.6), 0.42),
            iconText: rgbaFromChannels(darkMix(0.12), 0.95),
            subtleFill: rgbaFromChannels(whiteMix(0.97), 0.86),
            subtleStroke: rgbaFromChannels(whiteMix(0.68), 0.44),
            subtleText: rgbaFromChannels(darkMix(0.18), 0.88),
            progressTrack: rgbaFromChannels(accentChannels, 0.22),
            progressText: rgbaFromChannels(darkMix(0.18), 0.88)
        };
    }

    function resolveNodeAccentColor(node, colors) {
        if (typeof node?.accentColor === "string" && node.accentColor.trim().length > 0) {
            return node.accentColor.trim();
        }

        const resolved = colors || {};
        switch ((node?.paletteKey || "").toLowerCase()) {
            case "violet":
                return resolved.accentViolet || "#7c3aed";
            case "mint":
                return resolved.accentMint || "#10b981";
            case "sky":
                return resolved.accentSky || "#0ea5e9";
            case "amber":
                return resolved.accentAmber || "#f59e0b";
            case "rose":
                return resolved.accentRose || "#e11d48";
            case "success":
                return resolved.accentSuccess || "#059669";
            case "warning":
            case "warn":
                return resolved.accentWarning || "#d97706";
            case "danger":
                return resolved.accentDanger || "#dc2626";
            case "info":
                return resolved.accentInfo || "#0284c7";
            case "neutral":
                return resolved.accentNeutral || "#475569";
            default:
                return resolved.accentViolet || "#7c3aed";
        }
    }

    function resolveCanvasNodePaletteStyle(node, accent, isSelected, colors) {
        const resolved = colors || {};
        if (node?.isReadOnly) {
            return {
                surfaceFill: resolved.nodeReadonlySurfaceFill || "rgba(248, 250, 252, 0.98)",
                surfaceStroke: isSelected ? (resolved.nodeReadonlySurfaceStrokeSelected || "rgba(71, 85, 105, 0.9)") : (resolved.nodeReadonlySurfaceStrokeUnselected || "rgba(148, 163, 184, 0.34)"),
                surfaceShadow: resolved.nodeReadonlySurfaceShadow || "rgba(100, 116, 139, 0.12)",
                labelText: resolved.nodeReadonlyLabelText || "rgba(71, 85, 105, 0.76)",
                titleText: resolved.nodeReadonlyTitleText || "rgba(51, 65, 85, 0.94)",
                secondaryText: resolved.nodeReadonlySecondaryText || "rgba(100, 116, 139, 0.92)",
                iconFill: resolved.nodeReadonlyIconFill || "rgba(241, 245, 249, 0.94)",
                iconStroke: resolved.nodeReadonlyIconStroke || "rgba(148, 163, 184, 0.28)",
                iconText: resolved.nodeReadonlyIconText || "rgba(71, 85, 105, 0.96)",
                subtleFill: resolved.nodeReadonlySubtleFill || "rgba(255, 255, 255, 0.86)",
                subtleStroke: resolved.nodeReadonlySubtleStroke || "rgba(148, 163, 184, 0.3)",
                subtleText: resolved.nodeReadonlySubtleText || "rgba(51, 65, 85, 0.9)",
                progressTrack: resolved.nodeReadonlyProgressTrack || "rgba(148, 163, 184, 0.28)",
                progressText: resolved.nodeReadonlyProgressText || "rgba(71, 85, 105, 0.9)"
            };
        }

        const paletteKey = (node?.paletteKey || (node?.family === "root" ? "primary" : "neutral")).toLowerCase();
        const palettes = {
            primary: {
                    surfaceFill: resolved.nodePrimarySurfaceFill || "rgba(15, 23, 42, 0.98)",
                    surfaceStroke: isSelected ? (resolved.nodePrimarySurfaceStrokeSelected || "rgba(248, 250, 252, 0.94)") : (resolved.nodePrimarySurfaceStrokeUnselected || "rgba(51, 65, 85, 0.96)"),
                    surfaceShadow: resolved.nodePrimarySurfaceShadow || "rgba(15, 23, 42, 0.28)",
                    labelText: resolved.nodePrimaryLabelText || "rgba(191, 219, 254, 0.78)",
                    titleText: resolved.nodePrimaryTitleText || "rgba(248, 250, 252, 0.98)",
                    secondaryText: resolved.nodePrimarySecondaryText || "rgba(226, 232, 240, 0.84)",
                    iconFill: resolved.nodePrimaryIconFill || "rgba(255, 255, 255, 0.14)",
                    iconStroke: resolved.nodePrimaryIconStroke || "rgba(255, 255, 255, 0.18)",
                    iconText: resolved.nodePrimaryIconText || "rgba(248, 250, 252, 0.96)",
                    subtleFill: resolved.nodePrimarySubtleFill || "rgba(255, 255, 255, 0.14)",
                    subtleStroke: resolved.nodePrimarySubtleStroke || "rgba(255, 255, 255, 0.18)",
                    subtleText: resolved.nodePrimarySubtleText || "rgba(248, 250, 252, 0.94)",
                    progressTrack: resolved.nodePrimaryProgressTrack || "rgba(248, 250, 252, 0.24)",
                    progressText: resolved.nodePrimaryProgressText || "rgba(248, 250, 252, 0.92)"
            },
            secondary: {
                    surfaceFill: resolved.nodeSecondarySurfaceFill || "rgba(237, 233, 254, 0.98)",
                    surfaceStroke: isSelected ? (resolved.nodeSecondarySurfaceStrokeSelected || "rgba(124, 58, 237, 0.92)") : (resolved.nodeSecondarySurfaceStrokeUnselected || "rgba(167, 139, 250, 0.62)"),
                    surfaceShadow: resolved.nodeSecondarySurfaceShadow || "rgba(109, 40, 217, 0.16)",
                    labelText: resolved.nodeSecondaryLabelText || "rgba(109, 40, 217, 0.72)",
                    titleText: resolved.nodeSecondaryTitleText || "rgba(88, 28, 135, 0.94)",
                    secondaryText: resolved.nodeSecondarySecondaryText || "rgba(107, 33, 168, 0.82)",
                    iconFill: resolved.nodeSecondaryIconFill || "rgba(255, 255, 255, 0.64)",
                    iconStroke: resolved.nodeSecondaryIconStroke || "rgba(167, 139, 250, 0.4)",
                    iconText: resolved.nodeSecondaryIconText || "rgba(109, 40, 217, 0.94)",
                    subtleFill: resolved.nodeSecondarySubtleFill || "rgba(255, 255, 255, 0.74)",
                    subtleStroke: resolved.nodeSecondarySubtleStroke || "rgba(196, 181, 253, 0.44)",
                    subtleText: resolved.nodeSecondarySubtleText || "rgba(88, 28, 135, 0.88)",
                    progressTrack: resolved.nodeSecondaryProgressTrack || "rgba(139, 92, 246, 0.26)",
                    progressText: resolved.nodeSecondaryProgressText || "rgba(88, 28, 135, 0.88)"
            },
            success: {
                    surfaceFill: resolved.nodeSuccessSurfaceFill || "rgba(220, 252, 231, 0.98)",
                    surfaceStroke: isSelected ? (resolved.nodeSuccessSurfaceStrokeSelected || "rgba(22, 163, 74, 0.92)") : (resolved.nodeSuccessSurfaceStrokeUnselected || "rgba(74, 222, 128, 0.62)"),
                    surfaceShadow: resolved.nodeSuccessSurfaceShadow || "rgba(22, 163, 74, 0.14)",
                    labelText: resolved.nodeSuccessLabelText || "rgba(21, 128, 61, 0.72)",
                    titleText: resolved.nodeSuccessTitleText || "rgba(20, 83, 45, 0.95)",
                    secondaryText: resolved.nodeSuccessSecondaryText || "rgba(21, 128, 61, 0.82)",
                    iconFill: resolved.nodeSuccessIconFill || "rgba(255, 255, 255, 0.62)",
                    iconStroke: resolved.nodeSuccessIconStroke || "rgba(74, 222, 128, 0.42)",
                    iconText: resolved.nodeSuccessIconText || "rgba(21, 128, 61, 0.94)",
                    subtleFill: resolved.nodeSuccessSubtleFill || "rgba(255, 255, 255, 0.76)",
                    subtleStroke: resolved.nodeSuccessSubtleStroke || "rgba(134, 239, 172, 0.46)",
                    subtleText: resolved.nodeSuccessSubtleText || "rgba(20, 83, 45, 0.88)",
                    progressTrack: resolved.nodeSuccessProgressTrack || "rgba(22, 163, 74, 0.22)",
                    progressText: resolved.nodeSuccessProgressText || "rgba(20, 83, 45, 0.88)"
            },
            info: {
                    surfaceFill: resolved.nodeInfoSurfaceFill || "rgba(224, 242, 254, 0.98)",
                    surfaceStroke: isSelected ? (resolved.nodeInfoSurfaceStrokeSelected || "rgba(2, 132, 199, 0.92)") : (resolved.nodeInfoSurfaceStrokeUnselected || "rgba(125, 211, 252, 0.64)"),
                    surfaceShadow: resolved.nodeInfoSurfaceShadow || "rgba(2, 132, 199, 0.15)",
                    labelText: resolved.nodeInfoLabelText || "rgba(14, 116, 144, 0.72)",
                    titleText: resolved.nodeInfoTitleText || "rgba(12, 74, 110, 0.95)",
                    secondaryText: resolved.nodeInfoSecondaryText || "rgba(14, 116, 144, 0.82)",
                    iconFill: resolved.nodeInfoIconFill || "rgba(255, 255, 255, 0.62)",
                    iconStroke: resolved.nodeInfoIconStroke || "rgba(125, 211, 252, 0.44)",
                    iconText: resolved.nodeInfoIconText || "rgba(2, 132, 199, 0.94)",
                    subtleFill: resolved.nodeInfoSubtleFill || "rgba(255, 255, 255, 0.76)",
                    subtleStroke: resolved.nodeInfoSubtleStroke || "rgba(125, 211, 252, 0.46)",
                    subtleText: resolved.nodeInfoSubtleText || "rgba(12, 74, 110, 0.88)",
                    progressTrack: resolved.nodeInfoProgressTrack || "rgba(2, 132, 199, 0.22)",
                    progressText: resolved.nodeInfoProgressText || "rgba(12, 74, 110, 0.88)"
            },
            warning: {
                    surfaceFill: resolved.nodeWarningSurfaceFill || "rgba(254, 243, 199, 0.98)",
                    surfaceStroke: isSelected ? (resolved.nodeWarningSurfaceStrokeSelected || "rgba(217, 119, 6, 0.92)") : (resolved.nodeWarningSurfaceStrokeUnselected || "rgba(251, 191, 36, 0.64)"),
                    surfaceShadow: resolved.nodeWarningSurfaceShadow || "rgba(217, 119, 6, 0.15)",
                    labelText: resolved.nodeWarningLabelText || "rgba(180, 83, 9, 0.72)",
                    titleText: resolved.nodeWarningTitleText || "rgba(120, 53, 15, 0.95)",
                    secondaryText: resolved.nodeWarningSecondaryText || "rgba(146, 64, 14, 0.82)",
                    iconFill: resolved.nodeWarningIconFill || "rgba(255, 255, 255, 0.58)",
                    iconStroke: resolved.nodeWarningIconStroke || "rgba(251, 191, 36, 0.42)",
                    iconText: resolved.nodeWarningIconText || "rgba(180, 83, 9, 0.94)",
                    subtleFill: resolved.nodeWarningSubtleFill || "rgba(255, 255, 255, 0.76)",
                    subtleStroke: resolved.nodeWarningSubtleStroke || "rgba(252, 211, 77, 0.48)",
                    subtleText: resolved.nodeWarningSubtleText || "rgba(120, 53, 15, 0.88)",
                    progressTrack: resolved.nodeWarningProgressTrack || "rgba(217, 119, 6, 0.22)",
                    progressText: resolved.nodeWarningProgressText || "rgba(120, 53, 15, 0.88)"
            },
            danger: {
                    surfaceFill: resolved.nodeDangerSurfaceFill || "rgba(254, 226, 226, 0.98)",
                    surfaceStroke: isSelected ? (resolved.nodeDangerSurfaceStrokeSelected || "rgba(220, 38, 38, 0.94)") : (resolved.nodeDangerSurfaceStrokeUnselected || "rgba(252, 165, 165, 0.7)"),
                    surfaceShadow: resolved.nodeDangerSurfaceShadow || "rgba(220, 38, 38, 0.16)",
                    labelText: resolved.nodeDangerLabelText || "rgba(185, 28, 28, 0.72)",
                    titleText: resolved.nodeDangerTitleText || "rgba(127, 29, 29, 0.95)",
                    secondaryText: resolved.nodeDangerSecondaryText || "rgba(153, 27, 27, 0.82)",
                    iconFill: resolved.nodeDangerIconFill || "rgba(255, 255, 255, 0.6)",
                    iconStroke: resolved.nodeDangerIconStroke || "rgba(252, 165, 165, 0.46)",
                    iconText: resolved.nodeDangerIconText || "rgba(185, 28, 28, 0.95)",
                    subtleFill: resolved.nodeDangerSubtleFill || "rgba(255, 255, 255, 0.8)",
                    subtleStroke: resolved.nodeDangerSubtleStroke || "rgba(252, 165, 165, 0.48)",
                    subtleText: resolved.nodeDangerSubtleText || "rgba(127, 29, 29, 0.88)",
                    progressTrack: resolved.nodeDangerProgressTrack || "rgba(220, 38, 38, 0.22)",
                    progressText: resolved.nodeDangerProgressText || "rgba(127, 29, 29, 0.88)"
            },
            "workflow-decision": {
                    surfaceFill: resolved.nodeWorkflowDecisionSurfaceFill || "rgba(240, 253, 250, 0.98)",
                    surfaceStroke: isSelected ? (resolved.nodeWorkflowDecisionSurfaceStrokeSelected || "rgba(15, 118, 110, 0.96)") : (resolved.nodeWorkflowDecisionSurfaceStrokeUnselected || "rgba(20, 184, 166, 0.64)"),
                    surfaceShadow: resolved.nodeWorkflowDecisionSurfaceShadow || "rgba(15, 118, 110, 0.16)",
                    labelText: resolved.nodeWorkflowDecisionLabelText || "rgba(15, 118, 110, 0.78)",
                    titleText: resolved.nodeWorkflowDecisionTitleText || "rgba(19, 78, 74, 0.96)",
                    secondaryText: resolved.nodeWorkflowDecisionSecondaryText || "rgba(15, 118, 110, 0.84)",
                    iconFill: resolved.nodeWorkflowDecisionIconFill || "rgba(255, 251, 235, 0.86)",
                    iconStroke: resolved.nodeWorkflowDecisionIconStroke || "rgba(245, 158, 11, 0.46)",
                    iconText: resolved.nodeWorkflowDecisionIconText || "rgba(15, 118, 110, 0.96)",
                    subtleFill: resolved.nodeWorkflowDecisionSubtleFill || "rgba(255, 255, 255, 0.84)",
                    subtleStroke: resolved.nodeWorkflowDecisionSubtleStroke || "rgba(20, 184, 166, 0.4)",
                    subtleText: resolved.nodeWorkflowDecisionSubtleText || "rgba(15, 118, 110, 0.9)",
                    progressTrack: resolved.nodeWorkflowDecisionProgressTrack || "rgba(20, 184, 166, 0.24)",
                    progressText: resolved.nodeWorkflowDecisionProgressText || "rgba(19, 78, 74, 0.9)"
            },
            neutral: {
                    surfaceFill: resolved.nodeNeutralSurfaceFill || "rgba(241, 245, 249, 0.98)",
                    surfaceStroke: isSelected ? (resolved.nodeNeutralSurfaceStrokeSelected || "rgba(71, 85, 105, 0.92)") : (resolved.nodeNeutralSurfaceStrokeUnselected || "rgba(148, 163, 184, 0.44)"),
                    surfaceShadow: resolved.nodeNeutralSurfaceShadow || "rgba(71, 85, 105, 0.1)",
                    labelText: resolved.nodeNeutralLabelText || "rgba(71, 85, 105, 0.7)",
                    titleText: resolved.nodeNeutralTitleText || "rgba(15, 23, 42, 0.94)",
                    secondaryText: resolved.nodeNeutralSecondaryText || "rgba(71, 85, 105, 0.84)",
                    iconFill: resolved.nodeNeutralIconFill || "rgba(255, 255, 255, 0.72)",
                    iconStroke: resolved.nodeNeutralIconStroke || "rgba(148, 163, 184, 0.34)",
                    iconText: resolved.nodeNeutralIconText || "rgba(71, 85, 105, 0.94)",
                    subtleFill: resolved.nodeNeutralSubtleFill || "rgba(255, 255, 255, 0.82)",
                    subtleStroke: resolved.nodeNeutralSubtleStroke || "rgba(148, 163, 184, 0.38)",
                    subtleText: resolved.nodeNeutralSubtleText || "rgba(30, 41, 59, 0.88)",
                    progressTrack: resolved.nodeNeutralProgressTrack || "rgba(148, 163, 184, 0.28)",
                    progressText: resolved.nodeNeutralProgressText || "rgba(51, 65, 85, 0.88)"
            }
        };

        return palettes[paletteKey] || buildAccentPalette(accent, isSelected);
    }

    function resolveAnchorRect(anchor) {
        if (!anchor) {
            return null;
        }

        if (typeof anchor.getBoundingClientRect === "function") {
            const rect = anchor.getBoundingClientRect();
            if (Number.isFinite(rect?.left) &&
                Number.isFinite(rect?.top) &&
                Number.isFinite(rect?.width) &&
                Number.isFinite(rect?.height) &&
                Number.isFinite(rect?.right) &&
                Number.isFinite(rect?.bottom)) {
                return rect;
            }

            return null;
        }

        const left = typeof anchor.left === "number" ? anchor.left : anchor.x;
        const top = typeof anchor.top === "number" ? anchor.top : anchor.y;
        if (Number.isFinite(left) &&
            Number.isFinite(top) &&
            Number.isFinite(anchor.width) &&
            Number.isFinite(anchor.height)) {
            return {
                left,
                top,
                width: anchor.width,
                height: anchor.height,
                right: left + anchor.width,
                bottom: top + anchor.height
            };
        }

        return null;
    }

    function buildRect(left, top, width, height) {
        return {
            left,
            top,
            width,
            height,
            right: left + width,
            bottom: top + height
        };
    }

    function boundsToHitRect(bounds) {
        return {
            x: round(bounds.left),
            y: round(bounds.top),
            width: round(bounds.width),
            height: round(bounds.height)
        };
    }

    function projectSceneBounds(state, bounds) {
        const topLeft = worldToHostPoint(state, { x: bounds.left, y: bounds.top });
        const width = bounds.width * state.ui.zoom;
        const height = bounds.height * state.ui.zoom;
        return buildRect(topLeft.x, topLeft.y, width, height);
    }

    function getNodeSceneBounds(state, node) {
        const position = getNodePosition(state, node);
        const size = getNodeSize(state, node);
        return {
            left: position.x - (size.width / 2),
            top: position.y - (size.height / 2),
            width: size.width,
            height: size.height,
            centerX: position.x,
            centerY: position.y
        };
    }

    function clearSceneHotZones(state) {
        state.sceneHitRegistry?.clear?.();
        state.sceneHotZones = [];
    }

    function registerSceneHotZone(state, bounds, metadata) {
        if (!bounds || bounds.width <= 0 || bounds.height <= 0) {
            return;
        }

        const hitRect = boundsToHitRect(bounds);
        const entry = {
            bounds: hitRect,
            metadata: {
                ...metadata,
                bounds: hitRect
            }
        };
        state.sceneHotZones.push(entry);
        state.sceneHitRegistry?.add?.(hitRect, entry.metadata);
    }

    function getSceneHitAtPoint(state, pointX, pointY) {
        return state.sceneHitRegistry?.find?.(pointX, pointY) || null;
    }

    function getSceneHitAtEvent(state, event) {
        const point = getHostPoint(state, event.clientX, event.clientY);
        return getSceneHitAtPoint(state, point.x, point.y);
    }

    function resolveHitNode(state, hitTarget) {
        return hitTarget?.nodeId
            ? state.lookups.byId.get(hitTarget.nodeId) || null
            : null;
    }

    function clearScenePopoverHover(state) {
        state.hoveredAnnotationKey = "";
        hidePopover(state);
    }

    function areAnchorRectsEquivalent(left, right) {
        if (!left || !right) {
            return false;
        }

        return round(left.left) === round(right.left) &&
            round(left.top) === round(right.top) &&
            round(left.width) === round(right.width) &&
            round(left.height) === round(right.height);
    }

    function shouldRefreshScenePopover(state, annotationKey, anchorRect) {
        if (state.hoveredAnnotationKey !== annotationKey) {
            return true;
        }

        if (!state?.popover || !state.popover.isConnected || state.popover.style.display === "none") {
            return true;
        }

        return !areAnchorRectsEquivalent(resolveAnchorRect(state.popoverAnchor), anchorRect);
    }

    function showScenePopover(state, anchorRect, annotation) {
        const showPopoverFn = shared.showPopover;
        if (typeof showPopoverFn !== "function") {
            return false;
        }

        return showPopoverFn(state, anchorRect, annotation) !== false;
    }

    function renderConnectorAnchors(state, visibleNodes) {
        const renderConnectorAnchorOverlayFn =
            workbenchInternals.overlayRenderer?.renderConnectorAnchorOverlay ||
            shared.renderConnectorAnchorOverlay ||
            shared.legacyRenderConnectorAnchorOverlay;
        if (typeof renderConnectorAnchorOverlayFn === "function") {
            renderConnectorAnchorOverlayFn(state, visibleNodes);
        }
    }

    function syncSceneHoverState(state, event) {
        if (!state?.host?.isConnected || !state.lookups?.byId || !state.sceneHitRegistry) {
            clearScenePopoverHover(state);
            return;
        }

        const hitTarget = getSceneHitAtEvent(state, event);
        const nextNodeId = hitTarget?.nodeId || null;
        if ((state.hoveredNodeId || null) !== nextNodeId) {
            state.hoveredNodeId = nextNodeId;
            const visibleNodes = getVisibleNodes(state);
            renderConnectorAnchors(state, Array.isArray(visibleNodes) ? getProjectedNodes(state, visibleNodes) : []);
        }

        if (hitTarget?.type === "annotation" && hitTarget.annotation) {
            const annotationKey = `${hitTarget.nodeId}:${hitTarget.annotation.id || hitTarget.annotation.kind || hitTarget.annotation.label || hitTarget.annotationIndex || 0}`;
            const anchorRect = resolveAnchorRect(hitTarget.bounds);
            if (!anchorRect) {
                clearScenePopoverHover(state);
                return;
            }

            if (shouldRefreshScenePopover(state, annotationKey, anchorRect)) {
                if (!showScenePopover(state, anchorRect, hitTarget.annotation)) {
                    clearScenePopoverHover(state);
                    return;
                }

                state.hoveredAnnotationKey = annotationKey;
            }
            return;
        }

        clearScenePopoverHover(state);
    }

    Object.assign(shared, {
        getCanvasRuntimePrimitives,
        createFallbackHitRegistry,
        createCanvasHitRegistry,
        createCanvasSurfaceHost,
        destroyCanvasSurfaceHost,
        hexToRgba,
        resolveNodeAccentColor,
        resolveCanvasNodePaletteStyle,
        resolveAnchorRect,
        buildRect,
        boundsToHitRect,
        projectSceneBounds,
        getNodeSceneBounds,
        clearSceneHotZones,
        registerSceneHotZone,
        getSceneHitAtPoint,
        getSceneHitAtEvent,
        resolveHitNode,
        clearScenePopoverHover,
        syncSceneHoverState
    });
})();
