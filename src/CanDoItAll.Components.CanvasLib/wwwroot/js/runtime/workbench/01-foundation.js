(function () {
    const root = window.CanDoItAll = window.CanDoItAll || {};
    const contextSubmenuHoverDelayMs = 500;
    const MIN_ZOOM = 0.15;
    const MAX_ZOOM = 1.75;
    function getRequiredRootService(name) {
        const service = root[name];
        if (!service) {
            throw new Error(`CanDoItAll.${name} must be loaded before the CanvasLib workbench runtime.`);
        }

        return service;
    }
    function getTextMeasureService() {
        return root.textMeasureService || null;
    }
    function getViewportControllerService() {
        return root.viewportController || null;
    }
    function getAnimationTimelineService() {
        return root.animationTimeline || null;
    }
    const selectionModel = getRequiredRootService("selectionModel");
    function clear(element) {
        while (element.firstChild) {
            element.removeChild(element.firstChild);
        }
    }

    function createElement(document, tagName, className, text) {
        const element = document.createElement(tagName);
        if (className) {
            element.className = className;
        }

        if (typeof text === "string") {
            element.textContent = text;
        }

        return element;
    }

    function createSvgElement(document, tagName, className) {
        const element = document.createElementNS("http://www.w3.org/2000/svg", tagName);
        if (className) {
            element.setAttribute("class", className);
        }

        return element;
    }

    function normalizeInputField(field) {
        return {
            key: field?.key || "",
            sectionKey: field?.sectionKey || "",
            sectionTitle: field?.sectionTitle || "",
            sectionDescription: field?.sectionDescription || "",
            label: field?.label || field?.key || "Value",
            placeholder: field?.placeholder || "",
            inputMode: field?.inputMode || "text",
            isRequired: !!field?.isRequired,
            options: Array.isArray(field?.options)
                ? field.options
                    .filter(option => option && (option.value || option.label))
                    .map(option => ({
                        value: option.value || "",
                        label: option.label || option.value || ""
                    }))
                : []
        };
    }

    function normalizeInputValue(value) {
        return {
            key: value?.key || "",
            value: value?.value || ""
        };
    }

    function clamp(value, min, max) {
        return Math.max(min, Math.min(max, value));
    }

    function debounce(callback, delayMs) {
        let handle = 0;
        const debounced = (...args) => {
            window.clearTimeout(handle);
            handle = window.setTimeout(() => {
                handle = 0;
                callback(...args);
            }, delayMs);
        };

        debounced.cancel = () => {
            window.clearTimeout(handle);
            handle = 0;
        };

        return debounced;
    }

    function now() {
        return typeof performance !== "undefined" && typeof performance.now === "function"
            ? performance.now()
            : Date.now();
    }

    function createWorkbenchMetrics() {
        return {
            renderCount: 0,
            totalRenderDurationMs: 0,
            lastRenderDurationMs: 0,
            maxRenderDurationMs: 0,
            frameLayerRebuildCount: 0,
            linkLayerRebuildCount: 0,
            nodeLayerRebuildCount: 0,
            lastRenderedFrameCount: 0,
            lastRenderedLinkCount: 0,
            lastRenderedNodeCount: 0,
            lastVisibleNodeCount: 0,
            statePublishRequestCount: 0,
            statePublishImmediateCount: 0,
            statePublishCommitCount: 0,
            viewportCommitScheduleCount: 0,
            viewportCommitCount: 0,
            lastStatePublishMode: "",
            lastCommittedStateSize: 0,
            movePublishRequestCount: 0,
            movePublishSuccessCount: 0,
            movePublishFailureCount: 0,
            lastMovePublishStatus: "",
            lastResolvedDragDeltaX: 0,
            lastResolvedDragDeltaY: 0,
            lastReleasedInteractionKind: "",
            lastReleasedInteractionMoved: false,
            dragPatchCount: 0,
            totalDragPatchedNodeCount: 0,
            totalDragPatchedLinkCount: 0,
            totalDragPatchedFrameCount: 0,
            lastDragPatchedNodeCount: 0,
            lastDragPatchedLinkCount: 0,
            lastDragPatchedFrameCount: 0
        };
    }

    function formatMetricDuration(value) {
        if (typeof value !== "number" || !Number.isFinite(value)) {
            return "0 ms";
        }

        return `${round(value)} ms`;
    }

    function cloneWorkbenchMetrics(metrics) {
        return {
            renderCount: metrics?.renderCount || 0,
            totalRenderDurationMs: round(metrics?.totalRenderDurationMs || 0),
            lastRenderDurationMs: round(metrics?.lastRenderDurationMs || 0),
            maxRenderDurationMs: round(metrics?.maxRenderDurationMs || 0),
            frameLayerRebuildCount: metrics?.frameLayerRebuildCount || 0,
            linkLayerRebuildCount: metrics?.linkLayerRebuildCount || 0,
            nodeLayerRebuildCount: metrics?.nodeLayerRebuildCount || 0,
            lastRenderedFrameCount: metrics?.lastRenderedFrameCount || 0,
            lastRenderedLinkCount: metrics?.lastRenderedLinkCount || 0,
            lastRenderedNodeCount: metrics?.lastRenderedNodeCount || 0,
            lastVisibleNodeCount: metrics?.lastVisibleNodeCount || 0,
            statePublishRequestCount: metrics?.statePublishRequestCount || 0,
            statePublishImmediateCount: metrics?.statePublishImmediateCount || 0,
            statePublishCommitCount: metrics?.statePublishCommitCount || 0,
            viewportCommitScheduleCount: metrics?.viewportCommitScheduleCount || 0,
            viewportCommitCount: metrics?.viewportCommitCount || 0,
            lastStatePublishMode: metrics?.lastStatePublishMode || "",
            lastCommittedStateSize: metrics?.lastCommittedStateSize || 0,
            movePublishRequestCount: metrics?.movePublishRequestCount || 0,
            movePublishSuccessCount: metrics?.movePublishSuccessCount || 0,
            movePublishFailureCount: metrics?.movePublishFailureCount || 0,
            lastMovePublishStatus: metrics?.lastMovePublishStatus || "",
            lastResolvedDragDeltaX: round(metrics?.lastResolvedDragDeltaX || 0),
            lastResolvedDragDeltaY: round(metrics?.lastResolvedDragDeltaY || 0),
            lastReleasedInteractionKind: metrics?.lastReleasedInteractionKind || "",
            lastReleasedInteractionMoved: !!metrics?.lastReleasedInteractionMoved,
            dragPatchCount: metrics?.dragPatchCount || 0,
            totalDragPatchedNodeCount: metrics?.totalDragPatchedNodeCount || 0,
            totalDragPatchedLinkCount: metrics?.totalDragPatchedLinkCount || 0,
            totalDragPatchedFrameCount: metrics?.totalDragPatchedFrameCount || 0,
            lastDragPatchedNodeCount: metrics?.lastDragPatchedNodeCount || 0,
            lastDragPatchedLinkCount: metrics?.lastDragPatchedLinkCount || 0,
            lastDragPatchedFrameCount: metrics?.lastDragPatchedFrameCount || 0
        };
    }

    function incrementMetric(metrics, key) {
        if (!metrics || typeof metrics[key] !== "number") {
            return;
        }

        metrics[key] += 1;
    }

    function resetLastDragPatchMetrics(metrics) {
        if (!metrics) {
            return;
        }

        metrics.lastDragPatchedNodeCount = 0;
        metrics.lastDragPatchedLinkCount = 0;
        metrics.lastDragPatchedFrameCount = 0;
    }

    function recordDragPatchMetrics(metrics, nodeCount, linkCount, frameCount) {
        if (!metrics) {
            return;
        }

        metrics.dragPatchCount += 1;
        metrics.totalDragPatchedNodeCount += nodeCount;
        metrics.totalDragPatchedLinkCount += linkCount;
        metrics.totalDragPatchedFrameCount += frameCount;
        metrics.lastDragPatchedNodeCount = nodeCount;
        metrics.lastDragPatchedLinkCount = linkCount;
        metrics.lastDragPatchedFrameCount = frameCount;
    }

    function round(value) {
        return Math.round(value * 100) / 100;
    }

    function normalizeAction(action) {
        return {
            ...action,
            description: action?.description || "",
            menuLabel: action?.menuLabel || "",
            menuSize: action?.menuSize || "normal",
            submenuLayout: action?.submenuLayout || "",
            requiresInput: !!action?.requiresInput,
            setupRendererKey: action?.setupRendererKey || "",
            createMode: action?.createMode || "command",
            objectSubtype: action?.objectSubtype || "",
            titleLabel: action?.titleLabel || "Title",
            titlePlaceholder: action?.titlePlaceholder || "",
            subtitleLabel: action?.subtitleLabel || "Subtitle",
            subtitlePlaceholder: action?.subtitlePlaceholder || "",
            notesLabel: action?.notesLabel || "Notes",
            notesPlaceholder: action?.notesPlaceholder || "",
            showDefaultTextFields: action?.showDefaultTextFields !== false,
            submitLabel: action?.submitLabel || action?.label || "Create",
            requiresFile: !!action?.requiresFile,
            acceptedFileTypes: action?.acceptedFileTypes || "",
            filePrompt: action?.filePrompt || "Drop a file here or choose one.",
            supportsDragDrop: action?.supportsDragDrop !== false,
            inputFields: Array.isArray(action?.inputFields) ? action.inputFields.map(normalizeInputField) : [],
            defaultInputValues: Array.isArray(action?.defaultInputValues) ? action.defaultInputValues.map(normalizeInputValue) : [],
            children: Array.isArray(action?.children) ? action.children.map(normalizeAction) : []
        };
    }

    function normalizeAnnotation(annotation) {
        return {
            id: annotation?.id || "",
            kind: annotation?.kind || "info",
            tone: annotation?.tone || "accent",
            label: annotation?.label || "",
            description: annotation?.description || "",
            icon: annotation?.icon || "",
            actionId: annotation?.actionId || ""
        };
    }

    function normalizeCompactPath(path) {
        const fullPath = typeof path?.fullPath === "string" ? path.fullPath.trim() : "";
        if (!fullPath) {
            return null;
        }

        return {
            label: path?.label || "Path",
            displayText: path?.displayText || fullPath,
            fullPath,
            promotedText: path?.promotedText || ""
        };
    }

    function normalizeDiagnosticsOptions(options) {
        return {
            isEnabled: !!options?.isEnabled,
            showNodeBounds: options?.showNodeBounds !== false,
            showConnectorAnchors: options?.showConnectorAnchors !== false,
            showViewportStats: options?.showViewportStats !== false
        };
    }

    function normalizeMinimapOptions(options) {
        return {
            isEnabled: options?.isEnabled !== false,
            title: options?.title || "Scene overview"
        };
    }

    function normalizeClipboardOptions(options) {
        return {
            isEnabled: options?.isEnabled !== false,
            allowCopy: options?.allowCopy !== false,
            allowCut: options?.allowCut !== false,
            allowPaste: options?.allowPaste !== false,
            allowDuplicate: options?.allowDuplicate !== false,
            format: options?.format || "application/vnd.candoitall.canvas+json"
        };
    }

    function normalizeTooltipPopoverOptions(options) {
        return {
            isEnabled: options?.isEnabled !== false,
            focusTriggers: options?.focusTriggers !== false,
            supportsRichPreview: options?.supportsRichPreview !== false
        };
    }

    function normalizeMarqueeOptions(options) {
        const modifierKey = (options?.modifierKey || "Alt").toString().trim().toLowerCase();
        const selectionMode = (options?.selectionMode || "Intersect").toString().trim().toLowerCase();
        return {
            isEnabled: options?.isEnabled !== false,
            modifierKey: modifierKey || "alt",
            selectionMode: selectionMode === "contain" ? "contain" : "intersect"
        };
    }

    function normalizeSnapGuideOptions(options) {
        const tolerance = typeof options?.tolerance === "number" && Number.isFinite(options.tolerance)
            ? Math.max(0, options.tolerance)
            : 18;
        const modifierPolicy = (options?.modifierPolicy || "ShiftBypassesSnap").toString().trim().toLowerCase();
        return {
            isEnabled: options?.isEnabled !== false,
            tolerance,
            modifierPolicy: modifierPolicy === "none" ? "none" : "shift-bypasses-snap"
        };
    }

    function normalizeConnectorAnchorOptions(options) {
        const placementMode = (options?.placementMode || "Edges").toString().trim().toLowerCase();
        return {
            isEnabled: options?.isEnabled !== false,
            showOnHover: options?.showOnHover !== false,
            showOnSelection: options?.showOnSelection !== false,
            placementMode: placementMode || "edges"
        };
    }

    function normalizeTransformHandleOptions(options) {
        const placementMode = (options?.placementMode || "SelectionBounds").toString().trim().toLowerCase();
        return {
            isEnabled: options?.isEnabled !== false,
            showResizeHandles: options?.showResizeHandles !== false,
            showRotateHandle: options?.showRotateHandle !== false,
            placementMode: placementMode || "selectionbounds"
        };
    }

    function normalizeGroupFrame(frame) {
        return {
            id: frame?.id || "",
            label: frame?.label || "Group",
            tone: frame?.tone || "accent",
            anchorNodeIds: Array.isArray(frame?.anchorNodeIds) ? frame.anchorNodeIds.filter(Boolean) : []
        };
    }

    function normalizeProgressPercent(value) {
        if (typeof value !== "number" || Number.isNaN(value)) {
            return 0;
        }

        return clamp(Math.round(value), 0, 100);
    }

    function normalizeMenuActionScale(value) {
        if (typeof value !== "number" || Number.isNaN(value)) {
            return 1;
        }

        return clamp(round(value), 0.8, 1.4);
    }

    function normalizePortCollection(ports, defaultSide) {
        if (!Array.isArray(ports)) {
            return [];
        }

        return ports
            .filter(Boolean)
            .map(port => ({
                id: (port?.id || port?.label || defaultSide || "port").toString(),
                label: (port?.label || port?.id || "Port").toString(),
                side: (port?.side || defaultSide || "").toString().trim().toLowerCase() || defaultSide,
                tone: (port?.tone || "neutral").toString(),
                categoryKey: (port?.categoryKey || "").toString(),
                accentColor: (port?.accentColor || "").toString(),
                kind: (port?.kind || "").toString(),
                isRequired: !!port?.isRequired
            }));
    }

    function normalizeSurface(surface) {
        const normalizedSelection = selectionModel.normalize(
            surface?.uiState?.selectedNodeIds,
            Array.isArray(surface?.uiState?.selectedNodeIds) ? surface.uiState.selectedNodeIds[0] : null);
        const viewportController = getViewportControllerService();
        const normalizedViewport = viewportController?.normalizeUiState?.(surface?.uiState) || {
            zoom: typeof surface?.uiState?.zoom === "number" ? clamp(surface.uiState.zoom, MIN_ZOOM, MAX_ZOOM) : 1,
            panX: typeof surface?.uiState?.panX === "number" ? surface.uiState.panX : 90,
            panY: typeof surface?.uiState?.panY === "number" ? surface.uiState.panY : 110
        };

        return {
            surfaceId: surface?.surfaceId || "canvas-surface",
            mode: surface?.mode || "authoring",
            dependencySourceId: surface?.dependencySourceId || "",
            nodes: Array.isArray(surface?.nodes) ? surface.nodes.map(node => ({
                ...node,
                x: typeof node.x === "number" ? node.x : 120,
                y: typeof node.y === "number" ? node.y : 120,
                chips: Array.isArray(node.chips) ? node.chips : [],
                footerChips: Array.isArray(node.footerChips) ? node.footerChips : [],
                contextActions: Array.isArray(node.contextActions) ? node.contextActions.map(normalizeAction) : [],
                isInlineTextNode: !!node?.isInlineTextNode,
                inlineText: node?.inlineText || "",
                inlineTextPlaceholder: node?.inlineTextPlaceholder || "Write note",
                mediaKind: node?.mediaKind || "",
                mediaPreviewUrl: node?.mediaPreviewUrl || "",
                mediaPreviewAlt: node?.mediaPreviewAlt || node?.title || "",
                mediaContentType: node?.mediaContentType || "",
                mediaFileName: node?.mediaFileName || "",
                compactPath: normalizeCompactPath(node?.compactPath),
                progressMode: node?.progressMode || "na",
                progressPercent: normalizeProgressPercent(node?.progressPercent),
                markerIcon: node?.markerIcon || "",
                markerTone: node?.markerTone || "",
                markerLabel: node?.markerLabel || "",
                priority: typeof node?.priority === "number" ? clamp(Math.round(node.priority), 0, 6) : 0,
                annotations: Array.isArray(node?.annotations) ? node.annotations.map(normalizeAnnotation) : [],
                inputPorts: normalizePortCollection(node?.inputPorts, "left"),
                outputPorts: normalizePortCollection(node?.outputPorts, "right")
            })) : [],
            links: Array.isArray(surface?.links) ? surface.links.map(link => ({
                ...link,
                sourcePortId: link?.sourcePortId || "",
                targetPortId: link?.targetPortId || "",
                kind: link?.kind || "",
                label: link?.label || "",
                summary: link?.summary || "",
                tone: link?.tone || "neutral",
                isUserAuthored: !!link?.isUserAuthored
            })) : [],
            uiState: {
                version: surface?.uiState?.version || "canvas-workbench.v1",
                selectedNodeIds: normalizedSelection.selectedNodeIds,
                highlightedNodeIds: Array.isArray(surface?.uiState?.highlightedNodeIds)
                    ? [...new Set(surface.uiState.highlightedNodeIds.filter(Boolean))]
                    : [],
                collapsedNodeIds: Array.isArray(surface?.uiState?.collapsedNodeIds) ? [...surface.uiState.collapsedNodeIds] : [],
                groupFrames: Array.isArray(surface?.uiState?.groupFrames) ? surface.uiState.groupFrames.map(normalizeGroupFrame) : [],
                manualPositions: surface?.uiState?.manualPositions || {},
                windowStates: surface?.uiState?.windowStates || {},
                zoom: normalizedViewport.zoom,
                panX: normalizedViewport.panX,
                panY: normalizedViewport.panY,
                menuActionScale: normalizeMenuActionScale(surface?.uiState?.menuActionScale),
                isMaximized: !!surface?.uiState?.isMaximized,
                activeInspectorTab: surface?.uiState?.activeInspectorTab || "",
                showDiagnostics: !!surface?.uiState?.showDiagnostics,
                showMinimap: surface?.uiState?.showMinimap !== false
            },
            chrome: {
                quickCreateActions: Array.isArray(surface?.chrome?.quickCreateActions) ? surface.chrome.quickCreateActions.map(normalizeAction) : [],
                groupContextActions: Array.isArray(surface?.chrome?.groupContextActions) ? surface.chrome.groupContextActions.map(normalizeAction) : [],
                showQuickCreateRail: surface?.chrome?.showQuickCreateRail !== false,
                childNoteActionId: surface?.chrome?.childNoteActionId || "",
                siblingNoteActionId: surface?.chrome?.siblingNoteActionId || "",
                inlineNotePlaceholder: surface?.chrome?.inlineNotePlaceholder || "Write note",
                collapseOnDoubleClick: surface?.chrome?.collapseOnDoubleClick !== false,
                hintText: surface?.chrome?.hintText || "",
                emptyStateKicker: surface?.chrome?.emptyStateKicker || "Canvas",
                emptyStateTitle: surface?.chrome?.emptyStateTitle || "No nodes yet",
                emptyStateDescription: surface?.chrome?.emptyStateDescription || "Use quick create to start building the scene.",
                diagnostics: normalizeDiagnosticsOptions(surface?.chrome?.diagnostics),
                minimap: normalizeMinimapOptions(surface?.chrome?.minimap),
                clipboard: normalizeClipboardOptions(surface?.chrome?.clipboard),
                tooltipPopover: normalizeTooltipPopoverOptions(surface?.chrome?.tooltipPopover),
                marqueeSelection: normalizeMarqueeOptions(surface?.chrome?.marqueeSelection),
                snapGuides: normalizeSnapGuideOptions(surface?.chrome?.snapGuides),
                connectorAnchors: normalizeConnectorAnchorOptions(surface?.chrome?.connectorAnchors),
                transformHandles: normalizeTransformHandleOptions(surface?.chrome?.transformHandles)
            }
        };
    }

    function toSelectionSet(selectedNodeIds) {
        return new Set((selectedNodeIds || []).filter(Boolean));
    }

    function toCollapsedSet(collapsedNodeIds) {
        return new Set((collapsedNodeIds || []).filter(Boolean));
    }

    function getDefaultNodeSize(node) {
        const baseSize = resolveBaseNodeSize(node);
        return estimateNodeSizeFromText(node, baseSize);
    }

    function normalizeInlineTextParagraphs(text) {
        const normalized = typeof text === "string"
            ? text.replace(/\r\n/g, "\n").replace(/\r/g, "\n")
            : "";
        const paragraphs = normalized.split("\n");
        return paragraphs.length > 0
            ? paragraphs
            : [""];
    }

    function countInlineTextNodeMetaItems(node) {
        const markerCount = Array.isArray(node?.markers) && node.markers.length > 0
            ? Math.min(3, node.markers.length)
            : node?.markerIcon
                ? 1
                : 0;
        const annotationCount = Array.isArray(node?.annotations)
            ? Math.min(3, node.annotations.length)
            : 0;
        const statusCount = node?.statusPill ? 1 : 0;
        const priorityCount = node?.priority > 0 ? 1 : 0;

        return 1 + markerCount + annotationCount + statusCount + priorityCount;
    }

    function measureInlineTextNodeParagraphs(measureService, text, maxWidth, font, maxLines) {
        const paragraphs = normalizeInlineTextParagraphs(text);
        const lineHeightPx = Math.max(16, font?.lineHeightPx || 20);
        const lineLimit = Math.max(1, maxLines || 1);
        let remainingLines = lineLimit;
        let lineCount = 0;
        let estimatedWidth = 0;
        let isTruncated = false;

        for (let index = 0; index < paragraphs.length; index += 1) {
            if (remainingLines <= 0) {
                isTruncated = true;
                break;
            }

            const paragraph = paragraphs[index];
            if (!paragraph.trim()) {
                lineCount += 1;
                remainingLines -= 1;
                continue;
            }

            const measure = measureService.measure({
                text: paragraph,
                maxWidth,
                maxLines: remainingLines,
                font
            });
            const measuredLineCount = Math.max(1, measure?.lineCount || 1);
            lineCount += measuredLineCount;
            remainingLines -= measuredLineCount;
            estimatedWidth = Math.max(estimatedWidth, measure?.estimatedWidth || 0);

            if (measure?.isTruncated) {
                isTruncated = true;
                break;
            }
        }

        return {
            estimatedWidth,
            estimatedHeight: Math.ceil(Math.max(1, lineCount) * lineHeightPx),
            lineCount: Math.max(1, lineCount),
            isTruncated
        };
    }

    function estimateInlineTextNodeSize(node, baseSize, measureService) {
        const noteText = node.inlineText || node.title || node.subtitle || "Write note";
        const tokens = noteText.split(/\s+/).filter(Boolean);
        const longestTokenLength = tokens.reduce((longest, token) => Math.max(longest, token.length), 0);
        const noteFont = {
            family: "\"DM Sans\", sans-serif",
            sizePx: 14,
            weight: 700,
            lineHeightPx: 20
        };
        const minWidth = 148;
        const maxWidth = 420;
        const maxLines = 12;
        const widthBias = Math.max(0, Math.min(220, (noteText.length - 18) * 2.25));
        const longWordBias = Math.max(0, longestTokenLength - 12) * 4.5;
        let estimatedWidth = clamp(
            Math.ceil(baseSize.width + widthBias + longWordBias),
            minWidth,
            maxWidth);
        let textMeasure = measureInlineTextNodeParagraphs(
            measureService,
            noteText,
            Math.max(108, estimatedWidth - 40),
            noteFont,
            maxLines);
        estimatedWidth = clamp(
            Math.ceil(Math.max(minWidth, textMeasure.estimatedWidth + 40)),
            minWidth,
            maxWidth);
        textMeasure = measureInlineTextNodeParagraphs(
            measureService,
            noteText,
            Math.max(108, estimatedWidth - 40),
            noteFont,
            maxLines);

        const metaRows = Math.max(1, Math.ceil(countInlineTextNodeMetaItems(node) / 4));
        const metaHeight = 26 + ((metaRows - 1) * 18);
        const estimatedHeight = clamp(
            Math.ceil(30 + textMeasure.estimatedHeight + metaHeight),
            76,
            304);

        return {
            width: estimatedWidth,
            height: estimatedHeight
        };
    }

    function resolveBaseNodeSize(node) {
        if (node.isInlineTextNode) {
            return { width: 164, height: 76 };
        }

        if ((node?.family || "").toLowerCase() === "workflow-decision" ||
            (node?.paletteKey || "").toLowerCase() === "workflow-decision") {
            return { width: 178, height: 178 };
        }

        const inputPorts = Array.isArray(node?.inputPorts) ? node.inputPorts : [];
        const outputPorts = Array.isArray(node?.outputPorts) ? node.outputPorts : [];
        const portRows = Math.max(inputPorts.length, outputPorts.length);
        if (portRows > 0) {
            return {
                width: 336,
                height: Math.max(212, 188 + (Math.max(0, portRows - 1) * 28))
            };
        }

        switch ((node.family || "item").toLowerCase()) {
            case "root":
                return { width: 288, height: 210 };
            case "group":
                return { width: 272, height: 196 };
            case "special":
                return { width: 248, height: 178 };
            default:
                return { width: 256, height: 190 };
        }
    }

    function estimateNodeSizeFromText(node, baseSize) {
        const measureService = getTextMeasureService();
        if (!measureService || typeof measureService.measure !== "function") {
            return baseSize;
        }

        if (node.isInlineTextNode) {
            return estimateInlineTextNodeSize(node, baseSize, measureService);
        }

        const family = (node.family || "item").toLowerCase();
        if (family === "workflow-decision" || (node.paletteKey || "").toLowerCase() === "workflow-decision") {
            return baseSize;
        }

        const titleText = node.title || "Untitled";
        const subtitleText = node.subtitle || node.leadText || "";
        const chipText = Array.isArray(node.chips) && node.chips.length > 0
            ? node.chips[0].text
            : Array.isArray(node.footerChips) && node.footerChips.length > 0
                ? node.footerChips[0].text
                : "";
        const titleWidth = Math.max(124, baseSize.width - (family === "root" ? 86 : 72));
        const subtitleWidth = Math.max(120, baseSize.width - 72);
        const titleMeasure = measureService.measure({
            text: titleText,
            maxWidth: titleWidth,
            maxLines: 2,
            font: {
                family: "\"DM Sans\", sans-serif",
                sizePx: family === "root" ? 18 : 15,
                weight: 700,
                lineHeightPx: family === "root" ? 22 : 18
            }
        });
        const subtitleMeasure = subtitleText
            ? measureService.measure({
                text: subtitleText,
                maxWidth: subtitleWidth,
                maxLines: 2,
                font: {
                    family: "\"DM Sans\", sans-serif",
                    sizePx: 12,
                    weight: 600,
                    lineHeightPx: 16
                }
            })
            : null;
        const chipMeasure = chipText
            ? measureService.measure({
                text: chipText,
                maxWidth: Math.max(96, baseSize.width - 96),
                maxLines: 1,
                font: {
                    family: "\"DM Sans\", sans-serif",
                    sizePx: 11,
                    weight: 700,
                    lineHeightPx: 12
                }
            })
            : null;
        const inputPorts = Array.isArray(node?.inputPorts) ? node.inputPorts : [];
        const outputPorts = Array.isArray(node?.outputPorts) ? node.outputPorts : [];
        const longestInputPort = inputPorts.reduce((longest, port) => Math.max(longest, (port?.label || port?.id || "").length), 0);
        const longestOutputPort = outputPorts.reduce((longest, port) => Math.max(longest, (port?.label || port?.id || "").length), 0);
        const portRows = Math.max(inputPorts.length, outputPorts.length);
        const portWidthBudget = portRows > 0
            ? Math.min(420, Math.max(baseSize.width, 224 + (longestInputPort + longestOutputPort) * 5.8))
            : 0;
        const portHeight = portRows > 0
            ? Math.max(52, 28 + (portRows * 26))
            : 0;
        const annotationHeight = Array.isArray(node.annotations) ? Math.ceil(node.annotations.length / 2) * 14 : 0;
        const footerHeight = (Array.isArray(node.chips) && node.chips.length > 0 ? 18 : 0) +
            (Array.isArray(node.footerChips) && node.footerChips.length > 0 ? 18 : 0);
        const mediaHeight = node.mediaPreviewUrl ? 62 : 0;
        const bodyHeight = 92 +
            titleMeasure.estimatedHeight +
            (subtitleMeasure ? subtitleMeasure.estimatedHeight + 8 : 0) +
            annotationHeight +
            footerHeight +
            mediaHeight +
            portHeight;
        const estimatedWidth = Math.ceil(Math.max(
            baseSize.width,
            portWidthBudget,
            Math.min(
                356,
                Math.max(
                    titleMeasure.estimatedWidth + (family === "root" ? 82 : 68),
                    subtitleMeasure ? subtitleMeasure.estimatedWidth + 68 : 0,
                    chipMeasure ? chipMeasure.estimatedWidth + 96 : 0))));
        const estimatedHeight = Math.ceil(Math.max(baseSize.height, Math.min(340, bodyHeight)));

        return {
            width: estimatedWidth,
            height: estimatedHeight
        };
    }

    function getNodeSize(state, node) {
        const measured = state?.measuredNodeSizes?.get(node.id);
        if (measured?.width > 0 && measured?.height > 0) {
            return measured;
        }

        // Node content is replaced at the refresh boundary, where this cache is cleared.
        // Dragging only changes positions, so repeated geometry reads can reuse the estimate.
        const cached = state?.estimatedNodeSizes?.get(node.id);
        if (cached?.node === node) {
            return cached.size;
        }

        const size = getDefaultNodeSize(node);
        state?.estimatedNodeSizes?.set(node.id, { node, size });
        return size;
    }

    function buildNodeLookup(nodes) {
        const byId = new Map();
        const children = new Map();

        for (const node of nodes) {
            byId.set(node.id, node);
            if (node.parentId) {
                if (!children.has(node.parentId)) {
                    children.set(node.parentId, []);
                }

                children.get(node.parentId).push(node.id);
            }
        }

        return { byId, children };
    }

    function isNodeVisible(state, nodeId) {
        let current = state.lookups.byId.get(nodeId);
        while (current) {
            if (current.parentId && state.collapsedIds.has(current.parentId)) {
                return false;
            }

            current = current.parentId ? state.lookups.byId.get(current.parentId) : null;
        }

        return true;
    }

    function getVisibleNodes(state) {
        return state.surface.nodes.filter(node => isNodeVisible(state, node.id));
    }

    function getProjectionOverscanPx(state, hostRect) {
        const rect = hostRect || state.host?.getBoundingClientRect?.() || { width: 0, height: 0 };
        const baseOverscan = Math.max(180, Math.min(rect.width || 0, rect.height || 0) * 0.24);
        return state?.interaction
            ? Math.max(baseOverscan, 320)
            : baseOverscan;
    }

    function collectProjectedContextNodeIds(state) {
        const contextNodeIds = new Set(state.ui?.selectedNodeIds || []);
        if (Array.isArray(state.interaction?.nodeIds)) {
            for (const nodeId of state.interaction.nodeIds) {
                contextNodeIds.add(nodeId);
            }
        }

        if (state.hoveredNodeId) {
            contextNodeIds.add(state.hoveredNodeId);
        }

        for (const nodeId of [...contextNodeIds]) {
            let current = state.lookups.byId.get(nodeId) || null;
            while (current?.parentId) {
                contextNodeIds.add(current.parentId);
                current = state.lookups.byId.get(current.parentId) || null;
            }
        }

        return contextNodeIds;
    }

    function isNodeProjectedInViewport(state, node, visibleNodes, overscanPx, projectionContext) {
        const rect = projectionContext?.hostRect || state.host?.getBoundingClientRect?.();
        if (!rect) {
            return true;
        }

        const projectPoint = shared.worldToHostPoint;
        if (typeof projectPoint !== "function") {
            return true;
        }

        const resolved = projectionContext?.layoutPositions?.get?.(node.id);
        const nodePosition = resolved
            ? { x: resolved.x, y: resolved.y }
            : getNodePosition(state, node, visibleNodes);
        const position = projectPoint(state, nodePosition);
        const size = getNodeSize(state, node);
        const halfWidth = (size.width * state.ui.zoom) / 2;
        const halfHeight = (size.height * state.ui.zoom) / 2;
        return position.x + halfWidth >= -overscanPx &&
            position.x - halfWidth <= rect.width + overscanPx &&
            position.y + halfHeight >= -overscanPx &&
            position.y - halfHeight <= rect.height + overscanPx;
    }

    function getProjectedNodes(state, visibleNodes) {
        if (!Array.isArray(visibleNodes) || visibleNodes.length === 0) {
            return [];
        }

        const layoutPositions = ensureLayoutPositions(state, visibleNodes);
        const hostRect = state.host?.getBoundingClientRect?.();
        const overscanPx = getProjectionOverscanPx(state, hostRect);
        const contextNodeIds = collectProjectedContextNodeIds(state);
        const projectionContext = { hostRect, layoutPositions };
        const projectedNodes = visibleNodes.filter(node =>
            contextNodeIds.has(node.id) ||
            isNodeProjectedInViewport(state, node, visibleNodes, overscanPx, projectionContext));
        if (projectedNodes.length > 0) {
            return projectedNodes;
        }

        const fallbackNodeId = state.ui?.selectedNodeIds?.[0] || visibleNodes[0]?.id || null;
        const fallbackNode = fallbackNodeId
            ? visibleNodes.find(node => node.id === fallbackNodeId) || visibleNodes[0]
            : visibleNodes[0];
        return fallbackNode ? [fallbackNode] : [];
    }

    function getBaseNodePosition(state, node) {
        const manual = state.ui.manualPositions?.[node.id];
        return manual && typeof manual.x === "number" && typeof manual.y === "number"
            ? { x: manual.x, y: manual.y }
            : { x: node.x, y: node.y };
    }

    function getNodeDepth(state, nodeId, cache) {
        if (cache.has(nodeId)) {
            return cache.get(nodeId);
        }

        const node = state.lookups.byId.get(nodeId);
        if (!node || !node.parentId) {
            cache.set(nodeId, 0);
            return 0;
        }

        const depth = getNodeDepth(state, node.parentId, cache) + 1;
        cache.set(nodeId, depth);
        return depth;
    }

    function getNodeMobility(state, node) {
        if (Array.isArray(state.interaction?.nodeIds) &&
            state.interaction.nodeIds.includes(node.id)) {
            return 1;
        }

        if ((node.family || "").toLowerCase() === "root") {
            return 0.04;
        }

        if (node.isRequired) {
            return 0.18;
        }

        if (state.ui.manualPositions?.[node.id]) {
            return 0.08;
        }

        return 1;
    }

    function buildResolvedLayoutKey(state, visibleNodes) {
        return visibleNodes.map(node => {
            const base = getBaseNodePosition(state, node);
            const size = getNodeSize(state, node);
            const isActiveDragNode = Array.isArray(state.interaction?.nodeIds) &&
                state.interaction.nodeIds.includes(node.id);
            return [
                node.id,
                node.parentId || "",
                round(base.x),
                round(base.y),
                round(size.width),
                round(size.height),
                node.family || "",
                node.isInlineTextNode ? "1" : "0",
                isActiveDragNode ? "1" : "0"
            ].join("|");
        }).join(";");
    }

    function buildLayoutItems(state, visibleNodes) {
        const depthCache = new Map();
        const basePositions = new Map();
        for (const node of visibleNodes) {
            basePositions.set(node.id, getBaseNodePosition(state, node));
        }

        return visibleNodes.map(node => {
            const base = basePositions.get(node.id);
            const parentBase = node.parentId ? basePositions.get(node.parentId) : null;
            const horizontalDelta = parentBase ? (base.x - parentBase.x) : 0;
            const verticalDelta = parentBase ? (base.y - parentBase.y) : 0;
            return {
                id: node.id,
                node,
                parentId: node.parentId || null,
                size: getNodeSize(state, node),
                base,
                depth: getNodeDepth(state, node.id, depthCache),
                preferredSideX: horizontalDelta >= 0 ? 1 : -1,
                preferredSideY: Math.abs(verticalDelta) > 4 ? Math.sign(verticalDelta) : 1,
                mobility: getNodeMobility(state, node),
                isActiveDragNode: Array.isArray(state.interaction?.nodeIds) &&
                    state.interaction.nodeIds.includes(node.id)
            };
        });
    }

    function getCollisionPaddingX(first, second) {
        let padding = 28;
        if (first.parentId === second.id || second.parentId === first.id) {
            padding += 26;
        }
        else if (first.parentId && first.parentId === second.parentId) {
            padding += 12;
        }

        if ((first.node.family || "").toLowerCase() === "root" || (second.node.family || "").toLowerCase() === "root") {
            padding += 18;
        }

        return padding;
    }

    function getCollisionPaddingY(first, second) {
        let padding = 24;
        if (first.parentId && first.parentId === second.parentId) {
            padding += 16;
        }

        return padding;
    }

    function getOverlapDelta(first, second, firstPosition, secondPosition) {
        const deltaX = secondPosition.x - firstPosition.x;
        const deltaY = secondPosition.y - firstPosition.y;
        const overlapX = ((first.size.width + second.size.width) / 2) + getCollisionPaddingX(first, second) - Math.abs(deltaX);
        const overlapY = ((first.size.height + second.size.height) / 2) + getCollisionPaddingY(first, second) - Math.abs(deltaY);
        return { deltaX, deltaY, overlapX, overlapY };
    }

    function chooseCollisionAxis(first, second, overlap) {
        if (first.parentId === second.id || second.parentId === first.id) {
            return "x";
        }

        if (first.parentId && first.parentId === second.parentId) {
            return overlap.overlapY <= (overlap.overlapX * 1.35) ? "y" : "x";
        }

        return overlap.overlapX <= overlap.overlapY ? "x" : "y";
    }

    function resolveCollisionDirection(first, second, axis, overlap) {
        const delta = axis === "x" ? overlap.deltaX : overlap.deltaY;
        if (Math.abs(delta) > 0.5) {
            return Math.sign(delta);
        }

        if (axis === "x") {
            if (first.parentId === second.id) {
                return -(first.preferredSideX || 1);
            }

            if (second.parentId === first.id) {
                return second.preferredSideX || 1;
            }

            const baseDeltaX = second.base.x - first.base.x;
            if (Math.abs(baseDeltaX) > 0.5) {
                return Math.sign(baseDeltaX);
            }
        }
        else {
            const baseDeltaY = second.base.y - first.base.y;
            if (Math.abs(baseDeltaY) > 0.5) {
                return Math.sign(baseDeltaY);
            }
        }

        return first.id.localeCompare(second.id) <= 0 ? 1 : -1;
    }

    function applyCollisionSeparation(first, second, firstPosition, secondPosition, axis, amount, direction) {
        if (amount <= 0) {
            return false;
        }

        const firstActive = !!first.isActiveDragNode;
        const secondActive = !!second.isActiveDragNode;
        let firstDelta = 0;
        let secondDelta = 0;

        if (firstActive !== secondActive) {
            firstDelta = firstActive ? amount : 0;
            secondDelta = secondActive ? amount : 0;
        }
        else {
            const totalMobility = Math.max(0.001, first.mobility + second.mobility);
            firstDelta = amount * (first.mobility / totalMobility);
            secondDelta = amount * (second.mobility / totalMobility);
        }

        if (axis === "x") {
            if (first.mobility > 0) {
                firstPosition.x -= direction * firstDelta;
            }

            if (second.mobility > 0) {
                secondPosition.x += direction * secondDelta;
            }
        }
        else {
            if (first.mobility > 0) {
                firstPosition.y -= direction * firstDelta;
            }

            if (second.mobility > 0) {
                secondPosition.y += direction * secondDelta;
            }
        }

        return firstDelta > 0 || secondDelta > 0;
    }

    function separateOverlappingPairs(items, positions) {
        let moved = false;

        for (let index = 0; index < items.length; index++) {
            for (let compareIndex = index + 1; compareIndex < items.length; compareIndex++) {
                const first = items[index];
                const second = items[compareIndex];
                const firstPosition = positions.get(first.id);
                const secondPosition = positions.get(second.id);
                const overlap = getOverlapDelta(first, second, firstPosition, secondPosition);
                if (overlap.overlapX <= 0 || overlap.overlapY <= 0) {
                    continue;
                }

                const axis = chooseCollisionAxis(first, second, overlap);
                const direction = resolveCollisionDirection(first, second, axis, overlap);
                const amount = (axis === "x" ? overlap.overlapX : overlap.overlapY) + 10;
                moved = applyCollisionSeparation(first, second, firstPosition, secondPosition, axis, amount, direction) || moved;
            }
        }

        return moved;
    }

    function enforceParentClearance(itemsById, positions) {
        let moved = false;

        for (const item of itemsById.values()) {
            if (!item.parentId || !positions.has(item.parentId)) {
                continue;
            }

            const parent = itemsById.get(item.parentId);
            if (!parent) {
                continue;
            }

            const parentPosition = positions.get(parent.id);
            const itemPosition = positions.get(item.id);
            const preferredSide = item.preferredSideX || 1;
            const requiredDistance = ((parent.size.width + item.size.width) / 2) + 42;
            const targetX = parentPosition.x + (preferredSide * requiredDistance);

            if (preferredSide > 0 && itemPosition.x < targetX) {
                itemPosition.x = targetX;
                moved = true;
            }
            else if (preferredSide < 0 && itemPosition.x > targetX) {
                itemPosition.x = targetX;
                moved = true;
            }
        }

        return moved;
    }

    function enforceSiblingSpacing(itemsById, positions) {
        const groups = new Map();
        for (const item of itemsById.values()) {
            if (!item.parentId || !positions.has(item.parentId)) {
                continue;
            }

            if (!groups.has(item.parentId)) {
                groups.set(item.parentId, []);
            }

            groups.get(item.parentId).push(item);
        }

        let moved = false;
        for (const siblings of groups.values()) {
            siblings.sort((first, second) => {
                const firstPosition = positions.get(first.id);
                const secondPosition = positions.get(second.id);
                if (Math.abs(firstPosition.y - secondPosition.y) > 0.5) {
                    return firstPosition.y - secondPosition.y;
                }

                return first.base.y - second.base.y;
            });

            for (let index = 1; index < siblings.length; index++) {
                const previous = siblings[index - 1];
                const current = siblings[index];
                const previousPosition = positions.get(previous.id);
                const currentPosition = positions.get(current.id);
                const horizontalGap = Math.abs(currentPosition.x - previousPosition.x);
                const requiredHorizontalGap = ((previous.size.width + current.size.width) / 2) + 24;
                if (horizontalGap >= requiredHorizontalGap) {
                    continue;
                }

                const requiredVerticalGap = ((previous.size.height + current.size.height) / 2) + 28;
                const currentGap = currentPosition.y - previousPosition.y;
                if (currentGap < requiredVerticalGap) {
                    currentPosition.y = previousPosition.y + requiredVerticalGap;
                    moved = true;
                }
            }

            const desiredCenter = siblings.reduce((total, item) => total + item.base.y, 0) / Math.max(1, siblings.length);
            const actualCenter = siblings.reduce((total, item) => total + positions.get(item.id).y, 0) / Math.max(1, siblings.length);
            const shift = clamp(desiredCenter - actualCenter, -44, 44);
            if (Math.abs(shift) <= 0.5) {
                continue;
            }

            for (const item of siblings) {
                if (item.mobility <= 0) {
                    continue;
                }

                positions.get(item.id).y += shift * 0.18 * item.mobility;
                moved = true;
            }
        }

        return moved;
    }

    function relaxTowardBase(items, positions) {
        let moved = false;

        for (const item of items) {
            if (item.mobility <= 0) {
                continue;
            }

            const position = positions.get(item.id);
            const nextX = position.x + ((item.base.x - position.x) * 0.12 * item.mobility);
            const nextY = position.y + ((item.base.y - position.y) * 0.16 * item.mobility);
            if (Math.abs(nextX - position.x) > 0.4 || Math.abs(nextY - position.y) > 0.4) {
                position.x = nextX;
                position.y = nextY;
                moved = true;
            }
        }

        return moved;
    }

    function computeResolvedNodePositions(state, visibleNodes) {
        const items = buildLayoutItems(state, visibleNodes);
        const positions = new Map(items.map(item => [item.id, { x: item.base.x, y: item.base.y }]));
        const itemsById = new Map(items.map(item => [item.id, item]));

        for (let iteration = 0; iteration < 14; iteration++) {
            let moved = false;
            moved = enforceParentClearance(itemsById, positions) || moved;
            moved = enforceSiblingSpacing(itemsById, positions) || moved;
            moved = separateOverlappingPairs(items, positions) || moved;
            moved = relaxTowardBase(items, positions) || moved;
            if (!moved) {
                break;
            }
        }

        enforceParentClearance(itemsById, positions);
        enforceSiblingSpacing(itemsById, positions);
        for (let iteration = 0; iteration < 6; iteration++) {
            if (!separateOverlappingPairs(items, positions)) {
                break;
            }
        }

        return positions;
    }

    function ensureLayoutPositions(state, visibleNodes) {
        const nodes = Array.isArray(visibleNodes) ? visibleNodes : getVisibleNodes(state);
        const key = buildResolvedLayoutKey(state, nodes);
        if (state.layoutPositions && state.layoutKey === key) {
            return state.layoutPositions;
        }

        state.layoutPositions = computeResolvedNodePositions(state, nodes);
        state.layoutKey = key;
        state.sceneBounds = null;
        state.sceneBoundsKey = "";
        return state.layoutPositions;
    }

    function getNodePosition(state, node, visibleNodes) {
        const resolved = ensureLayoutPositions(state, visibleNodes).get(node.id);
        return resolved
            ? { x: resolved.x, y: resolved.y }
            : getBaseNodePosition(state, node);
    }

    function getSceneBounds(state, visibleNodes) {
        const nodes = Array.isArray(visibleNodes) ? visibleNodes : getVisibleNodes(state);
        if (!nodes.length) {
            return null;
        }

        const positions = ensureLayoutPositions(state, nodes);
        if (state.sceneBounds && state.sceneBoundsKey === state.layoutKey) {
            return state.sceneBounds;
        }

        let minX = Number.POSITIVE_INFINITY;
        let maxX = Number.NEGATIVE_INFINITY;
        let minY = Number.POSITIVE_INFINITY;
        let maxY = Number.NEGATIVE_INFINITY;

        for (const node of nodes) {
            const resolved = positions.get(node.id);
            const position = resolved
                ? { x: resolved.x, y: resolved.y }
                : getBaseNodePosition(state, node);
            const size = getNodeSize(state, node);
            minX = Math.min(minX, position.x - (size.width / 2));
            maxX = Math.max(maxX, position.x + (size.width / 2));
            minY = Math.min(minY, position.y - (size.height / 2));
            maxY = Math.max(maxY, position.y + (size.height / 2));
        }

        state.sceneBounds = { minX, maxX, minY, maxY };
        state.sceneBoundsKey = state.layoutKey;
        return state.sceneBounds;
    }

    function clampPanToScene(state, panX, panY, zoom) {
        const bounds = getSceneBounds(state);
        const rect = state.host.getBoundingClientRect();
        const viewportController = getViewportControllerService();
        if (viewportController?.clampPanToScene) {
            const clamped = viewportController.clampPanToScene({
                bounds,
                hostWidth: rect.width,
                hostHeight: rect.height,
                panX,
                panY,
                zoom: zoom || state.ui.zoom
            });

            return {
                x: round(clamped.panX),
                y: round(clamped.panY)
            };
        }

        if (!bounds || rect.width <= 0 || rect.height <= 0) {
            return { x: panX, y: panY };
        }

        const nextZoom = zoom || state.ui.zoom;
        const marginX = Math.max(160, rect.width * 0.5);
        const marginY = Math.max(140, rect.height * 0.5);
        const contentWidth = (bounds.maxX - bounds.minX) * nextZoom;
        const contentHeight = (bounds.maxY - bounds.minY) * nextZoom;

        let x = panX;
        let y = panY;

        const minPanX = rect.width - marginX - (bounds.maxX * nextZoom);
        const maxPanX = marginX - (bounds.minX * nextZoom);
        const minPanY = rect.height - marginY - (bounds.maxY * nextZoom);
        const maxPanY = marginY - (bounds.minY * nextZoom);

        x = clamp(panX, Math.min(minPanX, maxPanX), Math.max(minPanX, maxPanX));
        y = clamp(panY, Math.min(minPanY, maxPanY), Math.max(minPanY, maxPanY));

        return { x: round(x), y: round(y) };
    }

    function setPan(state, panX, panY, zoom, options) {
        if (options?.skipClamp) {
            state.ui.panX = round(panX);
            state.ui.panY = round(panY);
            return;
        }

        const clamped = clampPanToScene(state, panX, panY, zoom);
        state.ui.panX = clamped.x;
        state.ui.panY = clamped.y;
    }

    function syncMenuScaleCss(state) {
        if (!state?.host) {
            return;
        }

        state.host.style.setProperty("--cw-menu-scale", `${normalizeMenuActionScale(state.ui?.menuActionScale)}`);
    }

    function serializeState(state) {
        return JSON.stringify({
            version: state.ui.version || "canvas-workbench.v1",
            selectedNodeIds: [...state.selectedIds],
            highlightedNodeIds: [...(state.highlightedIds || new Set())],
            collapsedNodeIds: [...state.collapsedIds],
            groupFrames: Array.isArray(state.ui.groupFrames) ? state.ui.groupFrames.map(normalizeGroupFrame) : [],
            manualPositions: state.ui.manualPositions || {},
            windowStates: state.ui.windowStates || {},
            zoom: round(state.ui.zoom),
            panX: round(state.ui.panX),
            panY: round(state.ui.panY),
            menuActionScale: normalizeMenuActionScale(state.ui.menuActionScale),
            isMaximized: !!state.ui.isMaximized,
            activeInspectorTab: state.ui.activeInspectorTab || "",
            showDiagnostics: !!state.ui.showDiagnostics,
            showMinimap: state.ui.showMinimap !== false
        });
    }

    function legacyApplySceneTransform(state) {
        state.scene.style.transform = `translate(${state.ui.panX}px, ${state.ui.panY}px) scale(${state.ui.zoom})`;
    }

    function cancelViewportAnimation(state) {
        state.animationTimeline?.cancel?.("viewport");
    }

    function applySceneTransformNow(state) {
        const applySceneTransformFn = shared.applySceneTransform;
        if (typeof applySceneTransformFn === "function") {
            applySceneTransformFn(state);
            return;
        }

        legacyApplySceneTransform(state);
    }

    function renderNow(state) {
        const renderFn = shared.render;
        if (typeof renderFn === "function") {
            renderFn(state);
        }
    }

    function publishStateNow(state) {
        const publishStateFn = shared.publishState;
        if (typeof publishStateFn === "function") {
            publishStateFn(state);
        }
    }

    function updateViewportTransform(state, viewport, options) {
        state.ui.zoom = viewport.zoom;
        setPan(state, viewport.panX, viewport.panY, viewport.zoom, options);
        applySceneTransformNow(state);
    }

    function animateViewportTransition(state, target, options) {
        if (!target) {
            return false;
        }

        const current = {
            panX: state.ui.panX,
            panY: state.ui.panY,
            zoom: state.ui.zoom
        };

        const hasMeaningfulChange = Math.abs(current.panX - target.panX) > 0.5
            || Math.abs(current.panY - target.panY) > 0.5
            || Math.abs(current.zoom - target.zoom) > 0.001;

        if (!hasMeaningfulChange) {
            updateViewportTransform(state, target, options);
            renderNow(state);
            if (options?.publish !== false) {
                publishStateNow(state);
            }

            return false;
        }

        if (!state.animationTimeline) {
            updateViewportTransform(state, target, options);
            renderNow(state);
            if (options?.publish !== false) {
                publishStateNow(state);
            }

            return true;
        }

        state.animationTimeline.animateViewport({
            key: options?.key || "viewport",
            from: current,
            to: target,
            durationMs: options?.durationMs ?? 320,
            easing: options?.easing || "softInOut",
            apply(next) {
                updateViewportTransform(state, next, options);
            },
            complete() {
                renderNow(state);
                if (options?.publish !== false) {
                    publishStateNow(state);
                }
            }
        });

        return true;
    }

    // CanvasLib's canvas-painted colors -- the 10 named node accent colors
    // (resolveNodeAccentColor), resolveCanvasNodePaletteStyle's per-tone surface/text/icon
    // shades, and the remaining tone-keyed literals across 06-canvas-renderers.js (group
    // frames, ports, links, link labels, markers, progress badge, media preview, decision
    // node/cues, advanced/standard node) -- read through BaseLib's shared theme-tokens.js
    // module when present (CLAUDE.md rule 8), falling back to an inline getComputedStyle
    // read so callers degrade gracefully when it isn't loaded. Fallback literals mirror
    // the --ui-canvas-* tokens declared in Tailwind/theme.css.
    const canvasColorTokenMap = {
        accentViolet: { cssVar: "--ui-canvas-accent-violet", fallback: "#7c3aed" },
        accentMint: { cssVar: "--ui-canvas-tone-mint", fallback: "#10b981" },
        accentSky: { cssVar: "--ui-canvas-tone-sky-strong", fallback: "#0ea5e9" },
        accentAmber: { cssVar: "--ui-canvas-tone-amber", fallback: "#f59e0b" },
        accentRose: { cssVar: "--ui-canvas-tone-danger-strong", fallback: "#e11d48" },
        accentSuccess: { cssVar: "--ui-canvas-tone-mint-deep", fallback: "#059669" },
        accentWarning: { cssVar: "--ui-canvas-tone-amber-deep", fallback: "#d97706" },
        accentDanger: { cssVar: "--ui-canvas-tone-red", fallback: "#dc2626" },
        accentInfo: { cssVar: "--ui-canvas-tone-sky-deep", fallback: "#0284c7" },
        accentNeutral: { cssVar: "--color-chrome-600", fallback: "#475569" },
        nodeReadonlySurfaceFill: { cssVar: "--ui-canvas-node-readonly-surface-fill", fallback: "rgba(248, 250, 252, 0.98)" },
        nodeReadonlySurfaceStrokeSelected: { cssVar: "--ui-canvas-node-readonly-surface-stroke-selected", fallback: "rgba(71, 85, 105, 0.9)" },
        nodeReadonlySurfaceStrokeUnselected: { cssVar: "--ui-canvas-node-readonly-surface-stroke", fallback: "rgba(148, 163, 184, 0.34)" },
        nodeReadonlySurfaceShadow: { cssVar: "--ui-canvas-node-readonly-surface-shadow", fallback: "rgba(100, 116, 139, 0.12)" },
        nodeReadonlyLabelText: { cssVar: "--ui-canvas-node-readonly-label-text", fallback: "rgba(71, 85, 105, 0.76)" },
        nodeReadonlyTitleText: { cssVar: "--ui-canvas-node-readonly-title-text", fallback: "rgba(51, 65, 85, 0.94)" },
        nodeReadonlySecondaryText: { cssVar: "--ui-canvas-node-readonly-secondary-text", fallback: "rgba(100, 116, 139, 0.92)" },
        nodeReadonlyIconFill: { cssVar: "--ui-canvas-node-readonly-icon-fill", fallback: "rgba(241, 245, 249, 0.94)" },
        nodeReadonlyIconStroke: { cssVar: "--ui-canvas-node-readonly-icon-stroke", fallback: "rgba(148, 163, 184, 0.28)" },
        nodeReadonlyIconText: { cssVar: "--ui-canvas-node-readonly-icon-text", fallback: "rgba(71, 85, 105, 0.96)" },
        nodeReadonlySubtleFill: { cssVar: "--ui-canvas-node-readonly-subtle-fill", fallback: "rgba(255, 255, 255, 0.86)" },
        nodeReadonlySubtleStroke: { cssVar: "--ui-canvas-node-readonly-subtle-stroke", fallback: "rgba(148, 163, 184, 0.3)" },
        nodeReadonlySubtleText: { cssVar: "--ui-canvas-node-readonly-subtle-text", fallback: "rgba(51, 65, 85, 0.9)" },
        nodeReadonlyProgressTrack: { cssVar: "--ui-canvas-node-readonly-progress-track", fallback: "rgba(148, 163, 184, 0.28)" },
        nodeReadonlyProgressText: { cssVar: "--ui-canvas-node-readonly-progress-text", fallback: "rgba(71, 85, 105, 0.9)" },
        nodePrimarySurfaceFill: { cssVar: "--ui-canvas-node-primary-surface-fill", fallback: "rgba(15, 23, 42, 0.98)" },
        nodePrimarySurfaceStrokeSelected: { cssVar: "--ui-canvas-node-primary-surface-stroke-selected", fallback: "rgba(248, 250, 252, 0.94)" },
        nodePrimarySurfaceStrokeUnselected: { cssVar: "--ui-canvas-node-primary-surface-stroke", fallback: "rgba(51, 65, 85, 0.96)" },
        nodePrimarySurfaceShadow: { cssVar: "--ui-canvas-node-primary-surface-shadow", fallback: "rgba(15, 23, 42, 0.28)" },
        nodePrimaryLabelText: { cssVar: "--ui-canvas-node-primary-label-text", fallback: "rgba(191, 219, 254, 0.78)" },
        nodePrimaryTitleText: { cssVar: "--ui-canvas-node-primary-title-text", fallback: "rgba(248, 250, 252, 0.98)" },
        nodePrimarySecondaryText: { cssVar: "--ui-canvas-node-primary-secondary-text", fallback: "rgba(226, 232, 240, 0.84)" },
        nodePrimaryIconFill: { cssVar: "--ui-canvas-node-primary-icon-fill", fallback: "rgba(255, 255, 255, 0.14)" },
        nodePrimaryIconStroke: { cssVar: "--ui-canvas-node-primary-icon-stroke", fallback: "rgba(255, 255, 255, 0.18)" },
        nodePrimaryIconText: { cssVar: "--ui-canvas-node-primary-icon-text", fallback: "rgba(248, 250, 252, 0.96)" },
        nodePrimarySubtleFill: { cssVar: "--ui-canvas-node-primary-subtle-fill", fallback: "rgba(255, 255, 255, 0.14)" },
        nodePrimarySubtleStroke: { cssVar: "--ui-canvas-node-primary-subtle-stroke", fallback: "rgba(255, 255, 255, 0.18)" },
        nodePrimarySubtleText: { cssVar: "--ui-canvas-node-primary-subtle-text", fallback: "rgba(248, 250, 252, 0.94)" },
        nodePrimaryProgressTrack: { cssVar: "--ui-canvas-node-primary-progress-track", fallback: "rgba(248, 250, 252, 0.24)" },
        nodePrimaryProgressText: { cssVar: "--ui-canvas-node-primary-progress-text", fallback: "rgba(248, 250, 252, 0.92)" },
        nodeSecondarySurfaceFill: { cssVar: "--ui-canvas-node-secondary-surface-fill", fallback: "rgba(237, 233, 254, 0.98)" },
        nodeSecondarySurfaceStrokeSelected: { cssVar: "--ui-canvas-node-secondary-surface-stroke-selected", fallback: "rgba(124, 58, 237, 0.92)" },
        nodeSecondarySurfaceStrokeUnselected: { cssVar: "--ui-canvas-node-secondary-surface-stroke", fallback: "rgba(167, 139, 250, 0.62)" },
        nodeSecondarySurfaceShadow: { cssVar: "--ui-canvas-node-secondary-surface-shadow", fallback: "rgba(109, 40, 217, 0.16)" },
        nodeSecondaryLabelText: { cssVar: "--ui-canvas-node-secondary-label-text", fallback: "rgba(109, 40, 217, 0.72)" },
        nodeSecondaryTitleText: { cssVar: "--ui-canvas-node-secondary-title-text", fallback: "rgba(88, 28, 135, 0.94)" },
        nodeSecondarySecondaryText: { cssVar: "--ui-canvas-node-secondary-secondary-text", fallback: "rgba(107, 33, 168, 0.82)" },
        nodeSecondaryIconFill: { cssVar: "--ui-canvas-node-secondary-icon-fill", fallback: "rgba(255, 255, 255, 0.64)" },
        nodeSecondaryIconStroke: { cssVar: "--ui-canvas-node-secondary-icon-stroke", fallback: "rgba(167, 139, 250, 0.4)" },
        nodeSecondaryIconText: { cssVar: "--ui-canvas-node-secondary-icon-text", fallback: "rgba(109, 40, 217, 0.94)" },
        nodeSecondarySubtleFill: { cssVar: "--ui-canvas-node-secondary-subtle-fill", fallback: "rgba(255, 255, 255, 0.74)" },
        nodeSecondarySubtleStroke: { cssVar: "--ui-canvas-node-secondary-subtle-stroke", fallback: "rgba(196, 181, 253, 0.44)" },
        nodeSecondarySubtleText: { cssVar: "--ui-canvas-node-secondary-subtle-text", fallback: "rgba(88, 28, 135, 0.88)" },
        nodeSecondaryProgressTrack: { cssVar: "--ui-canvas-node-secondary-progress-track", fallback: "rgba(139, 92, 246, 0.26)" },
        nodeSecondaryProgressText: { cssVar: "--ui-canvas-node-secondary-progress-text", fallback: "rgba(88, 28, 135, 0.88)" },
        nodeSuccessSurfaceFill: { cssVar: "--ui-canvas-node-success-surface-fill", fallback: "rgba(220, 252, 231, 0.98)" },
        nodeSuccessSurfaceStrokeSelected: { cssVar: "--ui-canvas-node-success-surface-stroke-selected", fallback: "rgba(22, 163, 74, 0.92)" },
        nodeSuccessSurfaceStrokeUnselected: { cssVar: "--ui-canvas-node-success-surface-stroke", fallback: "rgba(74, 222, 128, 0.62)" },
        nodeSuccessSurfaceShadow: { cssVar: "--ui-canvas-node-success-surface-shadow", fallback: "rgba(22, 163, 74, 0.14)" },
        nodeSuccessLabelText: { cssVar: "--ui-canvas-node-success-label-text", fallback: "rgba(21, 128, 61, 0.72)" },
        nodeSuccessTitleText: { cssVar: "--ui-canvas-node-success-title-text", fallback: "rgba(20, 83, 45, 0.95)" },
        nodeSuccessSecondaryText: { cssVar: "--ui-canvas-node-success-secondary-text", fallback: "rgba(21, 128, 61, 0.82)" },
        nodeSuccessIconFill: { cssVar: "--ui-canvas-node-success-icon-fill", fallback: "rgba(255, 255, 255, 0.62)" },
        nodeSuccessIconStroke: { cssVar: "--ui-canvas-node-success-icon-stroke", fallback: "rgba(74, 222, 128, 0.42)" },
        nodeSuccessIconText: { cssVar: "--ui-canvas-node-success-icon-text", fallback: "rgba(21, 128, 61, 0.94)" },
        nodeSuccessSubtleFill: { cssVar: "--ui-canvas-node-success-subtle-fill", fallback: "rgba(255, 255, 255, 0.76)" },
        nodeSuccessSubtleStroke: { cssVar: "--ui-canvas-node-success-subtle-stroke", fallback: "rgba(134, 239, 172, 0.46)" },
        nodeSuccessSubtleText: { cssVar: "--ui-canvas-node-success-subtle-text", fallback: "rgba(20, 83, 45, 0.88)" },
        nodeSuccessProgressTrack: { cssVar: "--ui-canvas-node-success-progress-track", fallback: "rgba(22, 163, 74, 0.22)" },
        nodeSuccessProgressText: { cssVar: "--ui-canvas-node-success-progress-text", fallback: "rgba(20, 83, 45, 0.88)" },
        nodeInfoSurfaceFill: { cssVar: "--ui-canvas-node-info-surface-fill", fallback: "rgba(224, 242, 254, 0.98)" },
        nodeInfoSurfaceStrokeSelected: { cssVar: "--ui-canvas-node-info-surface-stroke-selected", fallback: "rgba(2, 132, 199, 0.92)" },
        nodeInfoSurfaceStrokeUnselected: { cssVar: "--ui-canvas-node-info-surface-stroke", fallback: "rgba(125, 211, 252, 0.64)" },
        nodeInfoSurfaceShadow: { cssVar: "--ui-canvas-node-info-surface-shadow", fallback: "rgba(2, 132, 199, 0.15)" },
        nodeInfoLabelText: { cssVar: "--ui-canvas-node-info-label-text", fallback: "rgba(14, 116, 144, 0.72)" },
        nodeInfoTitleText: { cssVar: "--ui-canvas-node-info-title-text", fallback: "rgba(12, 74, 110, 0.95)" },
        nodeInfoSecondaryText: { cssVar: "--ui-canvas-node-info-secondary-text", fallback: "rgba(14, 116, 144, 0.82)" },
        nodeInfoIconFill: { cssVar: "--ui-canvas-node-info-icon-fill", fallback: "rgba(255, 255, 255, 0.62)" },
        nodeInfoIconStroke: { cssVar: "--ui-canvas-node-info-icon-stroke", fallback: "rgba(125, 211, 252, 0.44)" },
        nodeInfoIconText: { cssVar: "--ui-canvas-node-info-icon-text", fallback: "rgba(2, 132, 199, 0.94)" },
        nodeInfoSubtleFill: { cssVar: "--ui-canvas-node-info-subtle-fill", fallback: "rgba(255, 255, 255, 0.76)" },
        nodeInfoSubtleStroke: { cssVar: "--ui-canvas-node-info-subtle-stroke", fallback: "rgba(125, 211, 252, 0.46)" },
        nodeInfoSubtleText: { cssVar: "--ui-canvas-node-info-subtle-text", fallback: "rgba(12, 74, 110, 0.88)" },
        nodeInfoProgressTrack: { cssVar: "--ui-canvas-node-info-progress-track", fallback: "rgba(2, 132, 199, 0.22)" },
        nodeInfoProgressText: { cssVar: "--ui-canvas-node-info-progress-text", fallback: "rgba(12, 74, 110, 0.88)" },
        nodeWarningSurfaceFill: { cssVar: "--ui-canvas-node-warning-surface-fill", fallback: "rgba(254, 243, 199, 0.98)" },
        nodeWarningSurfaceStrokeSelected: { cssVar: "--ui-canvas-node-warning-surface-stroke-selected", fallback: "rgba(217, 119, 6, 0.92)" },
        nodeWarningSurfaceStrokeUnselected: { cssVar: "--ui-canvas-node-warning-surface-stroke", fallback: "rgba(251, 191, 36, 0.64)" },
        nodeWarningSurfaceShadow: { cssVar: "--ui-canvas-node-warning-surface-shadow", fallback: "rgba(217, 119, 6, 0.15)" },
        nodeWarningLabelText: { cssVar: "--ui-canvas-node-warning-label-text", fallback: "rgba(180, 83, 9, 0.72)" },
        nodeWarningTitleText: { cssVar: "--ui-canvas-node-warning-title-text", fallback: "rgba(120, 53, 15, 0.95)" },
        nodeWarningSecondaryText: { cssVar: "--ui-canvas-node-warning-secondary-text", fallback: "rgba(146, 64, 14, 0.82)" },
        nodeWarningIconFill: { cssVar: "--ui-canvas-node-warning-icon-fill", fallback: "rgba(255, 255, 255, 0.58)" },
        nodeWarningIconStroke: { cssVar: "--ui-canvas-node-warning-icon-stroke", fallback: "rgba(251, 191, 36, 0.42)" },
        nodeWarningIconText: { cssVar: "--ui-canvas-node-warning-icon-text", fallback: "rgba(180, 83, 9, 0.94)" },
        nodeWarningSubtleFill: { cssVar: "--ui-canvas-node-warning-subtle-fill", fallback: "rgba(255, 255, 255, 0.76)" },
        nodeWarningSubtleStroke: { cssVar: "--ui-canvas-node-warning-subtle-stroke", fallback: "rgba(252, 211, 77, 0.48)" },
        nodeWarningSubtleText: { cssVar: "--ui-canvas-node-warning-subtle-text", fallback: "rgba(120, 53, 15, 0.88)" },
        nodeWarningProgressTrack: { cssVar: "--ui-canvas-node-warning-progress-track", fallback: "rgba(217, 119, 6, 0.22)" },
        nodeWarningProgressText: { cssVar: "--ui-canvas-node-warning-progress-text", fallback: "rgba(120, 53, 15, 0.88)" },
        nodeDangerSurfaceFill: { cssVar: "--ui-canvas-node-danger-surface-fill", fallback: "rgba(254, 226, 226, 0.98)" },
        nodeDangerSurfaceStrokeSelected: { cssVar: "--ui-canvas-node-danger-surface-stroke-selected", fallback: "rgba(220, 38, 38, 0.94)" },
        nodeDangerSurfaceStrokeUnselected: { cssVar: "--ui-canvas-node-danger-surface-stroke", fallback: "rgba(252, 165, 165, 0.7)" },
        nodeDangerSurfaceShadow: { cssVar: "--ui-canvas-node-danger-surface-shadow", fallback: "rgba(220, 38, 38, 0.16)" },
        nodeDangerLabelText: { cssVar: "--ui-canvas-node-danger-label-text", fallback: "rgba(185, 28, 28, 0.72)" },
        nodeDangerTitleText: { cssVar: "--ui-canvas-node-danger-title-text", fallback: "rgba(127, 29, 29, 0.95)" },
        nodeDangerSecondaryText: { cssVar: "--ui-canvas-node-danger-secondary-text", fallback: "rgba(153, 27, 27, 0.82)" },
        nodeDangerIconFill: { cssVar: "--ui-canvas-node-danger-icon-fill", fallback: "rgba(255, 255, 255, 0.6)" },
        nodeDangerIconStroke: { cssVar: "--ui-canvas-node-danger-icon-stroke", fallback: "rgba(252, 165, 165, 0.46)" },
        nodeDangerIconText: { cssVar: "--ui-canvas-node-danger-icon-text", fallback: "rgba(185, 28, 28, 0.95)" },
        nodeDangerSubtleFill: { cssVar: "--ui-canvas-node-danger-subtle-fill", fallback: "rgba(255, 255, 255, 0.8)" },
        nodeDangerSubtleStroke: { cssVar: "--ui-canvas-node-danger-subtle-stroke", fallback: "rgba(252, 165, 165, 0.48)" },
        nodeDangerSubtleText: { cssVar: "--ui-canvas-node-danger-subtle-text", fallback: "rgba(127, 29, 29, 0.88)" },
        nodeDangerProgressTrack: { cssVar: "--ui-canvas-node-danger-progress-track", fallback: "rgba(220, 38, 38, 0.22)" },
        nodeDangerProgressText: { cssVar: "--ui-canvas-node-danger-progress-text", fallback: "rgba(127, 29, 29, 0.88)" },
        nodeWorkflowDecisionSurfaceFill: { cssVar: "--ui-canvas-node-workflow-decision-surface-fill", fallback: "rgba(240, 253, 250, 0.98)" },
        nodeWorkflowDecisionSurfaceStrokeSelected: { cssVar: "--ui-canvas-node-workflow-decision-surface-stroke-selected", fallback: "rgba(15, 118, 110, 0.96)" },
        nodeWorkflowDecisionSurfaceStrokeUnselected: { cssVar: "--ui-canvas-node-workflow-decision-surface-stroke", fallback: "rgba(20, 184, 166, 0.64)" },
        nodeWorkflowDecisionSurfaceShadow: { cssVar: "--ui-canvas-node-workflow-decision-surface-shadow", fallback: "rgba(15, 118, 110, 0.16)" },
        nodeWorkflowDecisionLabelText: { cssVar: "--ui-canvas-node-workflow-decision-label-text", fallback: "rgba(15, 118, 110, 0.78)" },
        nodeWorkflowDecisionTitleText: { cssVar: "--ui-canvas-node-workflow-decision-title-text", fallback: "rgba(19, 78, 74, 0.96)" },
        nodeWorkflowDecisionSecondaryText: { cssVar: "--ui-canvas-node-workflow-decision-secondary-text", fallback: "rgba(15, 118, 110, 0.84)" },
        nodeWorkflowDecisionIconFill: { cssVar: "--ui-canvas-node-workflow-decision-icon-fill", fallback: "rgba(255, 251, 235, 0.86)" },
        nodeWorkflowDecisionIconStroke: { cssVar: "--ui-canvas-node-workflow-decision-icon-stroke", fallback: "rgba(245, 158, 11, 0.46)" },
        nodeWorkflowDecisionIconText: { cssVar: "--ui-canvas-node-workflow-decision-icon-text", fallback: "rgba(15, 118, 110, 0.96)" },
        nodeWorkflowDecisionSubtleFill: { cssVar: "--ui-canvas-node-workflow-decision-subtle-fill", fallback: "rgba(255, 255, 255, 0.84)" },
        nodeWorkflowDecisionSubtleStroke: { cssVar: "--ui-canvas-node-workflow-decision-subtle-stroke", fallback: "rgba(20, 184, 166, 0.4)" },
        nodeWorkflowDecisionSubtleText: { cssVar: "--ui-canvas-node-workflow-decision-subtle-text", fallback: "rgba(15, 118, 110, 0.9)" },
        nodeWorkflowDecisionProgressTrack: { cssVar: "--ui-canvas-node-workflow-decision-progress-track", fallback: "rgba(20, 184, 166, 0.24)" },
        nodeWorkflowDecisionProgressText: { cssVar: "--ui-canvas-node-workflow-decision-progress-text", fallback: "rgba(19, 78, 74, 0.9)" },
        nodeNeutralSurfaceFill: { cssVar: "--ui-canvas-node-neutral-surface-fill", fallback: "rgba(241, 245, 249, 0.98)" },
        nodeNeutralSurfaceStrokeSelected: { cssVar: "--ui-canvas-node-neutral-surface-stroke-selected", fallback: "rgba(71, 85, 105, 0.92)" },
        nodeNeutralSurfaceStrokeUnselected: { cssVar: "--ui-canvas-node-neutral-surface-stroke", fallback: "rgba(148, 163, 184, 0.44)" },
        nodeNeutralSurfaceShadow: { cssVar: "--ui-canvas-node-neutral-surface-shadow", fallback: "rgba(71, 85, 105, 0.1)" },
        nodeNeutralLabelText: { cssVar: "--ui-canvas-node-neutral-label-text", fallback: "rgba(71, 85, 105, 0.7)" },
        nodeNeutralTitleText: { cssVar: "--ui-canvas-node-neutral-title-text", fallback: "rgba(15, 23, 42, 0.94)" },
        nodeNeutralSecondaryText: { cssVar: "--ui-canvas-node-neutral-secondary-text", fallback: "rgba(71, 85, 105, 0.84)" },
        nodeNeutralIconFill: { cssVar: "--ui-canvas-node-neutral-icon-fill", fallback: "rgba(255, 255, 255, 0.72)" },
        nodeNeutralIconStroke: { cssVar: "--ui-canvas-node-neutral-icon-stroke", fallback: "rgba(148, 163, 184, 0.34)" },
        nodeNeutralIconText: { cssVar: "--ui-canvas-node-neutral-icon-text", fallback: "rgba(71, 85, 105, 0.94)" },
        nodeNeutralSubtleFill: { cssVar: "--ui-canvas-node-neutral-subtle-fill", fallback: "rgba(255, 255, 255, 0.82)" },
        nodeNeutralSubtleStroke: { cssVar: "--ui-canvas-node-neutral-subtle-stroke", fallback: "rgba(148, 163, 184, 0.38)" },
        nodeNeutralSubtleText: { cssVar: "--ui-canvas-node-neutral-subtle-text", fallback: "rgba(30, 41, 59, 0.88)" },
        nodeNeutralProgressTrack: { cssVar: "--ui-canvas-node-neutral-progress-track", fallback: "rgba(148, 163, 184, 0.28)" },
        nodeNeutralProgressText: { cssVar: "--ui-canvas-node-neutral-progress-text", fallback: "rgba(51, 65, 85, 0.88)" },
        annotationBadgeFill: { cssVar: "--ui-canvas-annotation-badge-fill", fallback: "rgba(255, 255, 255, 0.94)" },
        annotationBadgeStroke: { cssVar: "--ui-canvas-annotation-badge-stroke", fallback: "rgba(148, 163, 184, 0.24)" },
        annotationBadgeText: { cssVar: "--ui-canvas-annotation-badge-text", fallback: "rgba(15, 23, 42, 0.78)" },
        decisionAnchorStrokeDefault: { cssVar: "--ui-canvas-decision-anchor-stroke-default", fallback: "rgba(20, 184, 166, 0.92)" },
        decisionBodyFill: { cssVar: "--ui-canvas-decision-body-fill", fallback: "rgba(255, 255, 255, 0.98)" },
        decisionBranchPillFill: { cssVar: "--ui-canvas-decision-branch-pill-fill", fallback: "rgba(240, 253, 250, 0.98)" },
        decisionBranchPillStroke: { cssVar: "--ui-canvas-decision-branch-pill-stroke", fallback: "rgba(20, 184, 166, 0.38)" },
        decisionBranchPillText: { cssVar: "--ui-canvas-decision-branch-pill-text", fallback: "rgba(15, 118, 110, 0.98)" },
        decisionCueForkDotFill: { cssVar: "--ui-canvas-decision-cue-fork-dot-fill", fallback: "rgba(15, 118, 110, 0.78)" },
        decisionCueForkStroke: { cssVar: "--ui-canvas-decision-cue-fork-stroke", fallback: "rgba(245, 158, 11, 0.58)" },
        decisionCueIconFillDefault: { cssVar: "--ui-canvas-decision-cue-icon-fill-default", fallback: "rgba(255, 251, 235, 0.9)" },
        decisionCueIconStrokeDefault: { cssVar: "--ui-canvas-decision-cue-icon-stroke-default", fallback: "rgba(245, 158, 11, 0.56)" },
        decisionCueShadow: { cssVar: "--ui-canvas-decision-cue-shadow", fallback: "rgba(15, 118, 110, 0.16)" },
        decisionGlyphStroke: { cssVar: "--ui-canvas-decision-glyph-stroke", fallback: "rgba(51, 65, 85, 0.9)" },
        decisionShadowDefault: { cssVar: "--ui-canvas-decision-shadow-default", fallback: "rgba(15, 23, 42, 0.12)" },
        decisionShadowSelected: { cssVar: "--ui-canvas-decision-shadow-selected", fallback: "rgba(15, 118, 110, 0.2)" },
        decisionStrokeDefault: { cssVar: "--ui-canvas-decision-stroke-default", fallback: "rgba(20, 184, 166, 0.9)" },
        decisionTitleText: { cssVar: "--ui-canvas-decision-title-text", fallback: "rgba(15, 23, 42, 0.92)" },
        frameAccentFill: { cssVar: "--ui-canvas-frame-accent-fill", fallback: "rgba(124, 58, 237, 0.08)" },
        frameAccentStroke: { cssVar: "--ui-canvas-frame-accent-stroke", fallback: "rgba(124, 58, 237, 0.72)" },
        frameChipBg: { cssVar: "--ui-canvas-frame-chip-bg", fallback: "rgba(255, 255, 255, 0.96)" },
        frameDangerFill: { cssVar: "--ui-canvas-frame-danger-fill", fallback: "rgba(220, 38, 38, 0.08)" },
        frameDangerStroke: { cssVar: "--ui-canvas-frame-danger-stroke", fallback: "rgba(220, 38, 38, 0.72)" },
        frameLabelText: { cssVar: "--ui-canvas-frame-label-text", fallback: "rgba(15, 23, 42, 0.74)" },
        frameMemberCountText: { cssVar: "--ui-canvas-frame-member-count-text", fallback: "rgba(71, 85, 105, 0.84)" },
        frameSuccessFill: { cssVar: "--ui-canvas-frame-success-fill", fallback: "rgba(5, 150, 105, 0.08)" },
        frameSuccessStroke: { cssVar: "--ui-canvas-frame-success-stroke", fallback: "rgba(5, 150, 105, 0.72)" },
        frameWarningFill: { cssVar: "--ui-canvas-frame-warning-fill", fallback: "rgba(217, 119, 6, 0.08)" },
        frameWarningStroke: { cssVar: "--ui-canvas-frame-warning-stroke", fallback: "rgba(217, 119, 6, 0.72)" },
        linkDangerArrowFill: { cssVar: "--ui-canvas-link-danger-arrow-fill", fallback: "rgba(225, 29, 72, 0.96)" },
        linkDangerStroke: { cssVar: "--ui-canvas-link-danger-stroke", fallback: "rgba(244, 63, 94, 0.9)" },
        linkDefaultArrowFill: { cssVar: "--ui-canvas-link-default-arrow-fill", fallback: "rgba(100, 116, 139, 0.58)" },
        linkDefaultStroke: { cssVar: "--ui-canvas-link-default-stroke", fallback: "rgba(100, 116, 139, 0.44)" },
        linkDependencyArrowFill: { cssVar: "--ui-canvas-link-dependency-arrow-fill", fallback: "rgba(29, 78, 216, 0.98)" },
        linkDependencyStroke: { cssVar: "--ui-canvas-link-dependency-stroke", fallback: "rgba(37, 99, 235, 0.94)" },
        linkHoveredArrowFill: { cssVar: "--ui-canvas-link-hovered-arrow-fill", fallback: "rgba(220, 38, 38, 0.96)" },
        linkHoveredStroke: { cssVar: "--ui-canvas-link-hovered-stroke", fallback: "rgba(239, 68, 68, 0.94)" },
        linkInfoArrowFill: { cssVar: "--ui-canvas-link-info-arrow-fill", fallback: "rgba(2, 132, 199, 0.98)" },
        linkInfoStroke: { cssVar: "--ui-canvas-link-info-stroke", fallback: "rgba(14, 165, 233, 0.9)" },
        linkLabelDangerFill: { cssVar: "--ui-canvas-link-label-danger-fill", fallback: "rgba(254, 242, 242, 0.98)" },
        linkLabelDangerStroke: { cssVar: "--ui-canvas-link-label-danger-stroke", fallback: "rgba(244, 63, 94, 0.44)" },
        linkLabelDangerText: { cssVar: "--ui-canvas-link-label-danger-text", fallback: "rgba(159, 18, 57, 0.98)" },
        linkLabelDefaultFill: { cssVar: "--ui-canvas-link-label-default-fill", fallback: "rgba(255, 255, 255, 0.96)" },
        linkLabelDefaultStroke: { cssVar: "--ui-canvas-link-label-default-stroke", fallback: "rgba(100, 116, 139, 0.32)" },
        linkLabelDefaultText: { cssVar: "--ui-canvas-link-label-default-text", fallback: "rgba(30, 41, 59, 0.9)" },
        linkLabelInfoFill: { cssVar: "--ui-canvas-link-label-info-fill", fallback: "rgba(240, 249, 255, 0.98)" },
        linkLabelInfoStroke: { cssVar: "--ui-canvas-link-label-info-stroke", fallback: "rgba(14, 165, 233, 0.42)" },
        linkLabelInfoText: { cssVar: "--ui-canvas-link-label-info-text", fallback: "rgba(3, 105, 161, 0.98)" },
        linkLabelPanelShadow: { cssVar: "--ui-canvas-link-label-panel-shadow", fallback: "rgba(15, 23, 42, 0.1)" },
        linkLabelSuccessFill: { cssVar: "--ui-canvas-link-label-success-fill", fallback: "rgba(236, 253, 245, 0.97)" },
        linkLabelSuccessStroke: { cssVar: "--ui-canvas-link-label-success-stroke", fallback: "rgba(20, 184, 166, 0.42)" },
        linkLabelSuccessText: { cssVar: "--ui-canvas-link-label-success-text", fallback: "rgba(15, 118, 110, 0.98)" },
        linkLabelWarningFill: { cssVar: "--ui-canvas-link-label-warning-fill", fallback: "rgba(255, 251, 235, 0.98)" },
        linkLabelWarningStroke: { cssVar: "--ui-canvas-link-label-warning-stroke", fallback: "rgba(245, 158, 11, 0.46)" },
        linkLabelWarningText: { cssVar: "--ui-canvas-link-label-warning-text", fallback: "rgba(146, 64, 14, 0.98)" },
        linkPreviewArrowFill: { cssVar: "--ui-canvas-link-preview-arrow-fill", fallback: "rgba(109, 40, 217, 0.96)" },
        linkPreviewStroke: { cssVar: "--ui-canvas-link-preview-stroke", fallback: "rgba(124, 58, 237, 0.92)" },
        linkSuccessArrowFill: { cssVar: "--ui-canvas-link-success-arrow-fill", fallback: "rgba(15, 118, 110, 0.96)" },
        linkSuccessStroke: { cssVar: "--ui-canvas-link-success-stroke", fallback: "rgba(20, 184, 166, 0.9)" },
        linkUserAuthoredArrowFill: { cssVar: "--ui-canvas-link-user-authored-arrow-fill", fallback: "rgba(14, 165, 233, 0.88)" },
        linkUserAuthoredStroke: { cssVar: "--ui-canvas-link-user-authored-stroke", fallback: "rgba(14, 165, 233, 0.82)" },
        linkWarningArrowFill: { cssVar: "--ui-canvas-link-warning-arrow-fill", fallback: "rgba(217, 119, 6, 0.98)" },
        linkWarningStroke: { cssVar: "--ui-canvas-link-warning-stroke", fallback: "rgba(245, 158, 11, 0.94)" },
        markerDanger: { cssVar: "--ui-canvas-marker-danger", fallback: "#e11d48" },
        markerDefaultFallback: { cssVar: "--ui-canvas-marker-default-fallback", fallback: "#8b5cf6" },
        markerGhost: { cssVar: "--ui-canvas-marker-ghost", fallback: "#94a3b8" },
        markerMint: { cssVar: "--ui-canvas-marker-mint", fallback: "#10b981" },
        markerPrimary: { cssVar: "--ui-canvas-marker-primary", fallback: "#0f172a" },
        markerSky: { cssVar: "--ui-canvas-marker-sky", fallback: "#38bdf8" },
        markerWarn: { cssVar: "--ui-canvas-marker-warn", fallback: "#f97316" },
        mediaPreviewCaptionText: { cssVar: "--ui-canvas-media-preview-caption-text", fallback: "rgba(15, 23, 42, 0.76)" },
        mediaPreviewPanelFill: { cssVar: "--ui-canvas-media-preview-panel-fill", fallback: "rgba(241, 245, 249, 0.95)" },
        mediaPreviewPanelStroke: { cssVar: "--ui-canvas-media-preview-panel-stroke", fallback: "rgba(148, 163, 184, 0.22)" },
        portAccentAccentColor: { cssVar: "--ui-canvas-port-accent-accent-color", fallback: "#3b82f6" },
        portAccentAnchor: { cssVar: "--ui-canvas-port-accent-anchor", fallback: "rgba(59, 130, 246, 0.94)" },
        portAccentFill: { cssVar: "--ui-canvas-port-accent-fill", fallback: "rgba(59, 130, 246, 0.18)" },
        portAccentHalo: { cssVar: "--ui-canvas-port-accent-halo", fallback: "rgba(59, 130, 246, 0.18)" },
        portAccentLine: { cssVar: "--ui-canvas-port-accent-line", fallback: "rgba(59, 130, 246, 0.88)" },
        portAccentStroke: { cssVar: "--ui-canvas-port-accent-stroke", fallback: "rgba(37, 99, 235, 0.48)" },
        portAccentText: { cssVar: "--ui-canvas-port-accent-text", fallback: "rgba(219, 234, 254, 0.98)" },
        portDangerAccentColor: { cssVar: "--ui-canvas-port-danger-accent-color", fallback: "#ef4444" },
        portDangerAnchor: { cssVar: "--ui-canvas-port-danger-anchor", fallback: "rgba(239, 68, 68, 0.92)" },
        portDangerFill: { cssVar: "--ui-canvas-port-danger-fill", fallback: "rgba(239, 68, 68, 0.18)" },
        portDangerHalo: { cssVar: "--ui-canvas-port-danger-halo", fallback: "rgba(239, 68, 68, 0.18)" },
        portDangerLine: { cssVar: "--ui-canvas-port-danger-line", fallback: "rgba(239, 68, 68, 0.88)" },
        portDangerStroke: { cssVar: "--ui-canvas-port-danger-stroke", fallback: "rgba(220, 38, 38, 0.48)" },
        portDangerText: { cssVar: "--ui-canvas-port-danger-text", fallback: "rgba(254, 226, 226, 0.98)" },
        portDefaultAnchor: { cssVar: "--ui-canvas-port-default-anchor", fallback: "rgba(71, 85, 105, 0.88)" },
        portDefaultFill: { cssVar: "--ui-canvas-port-default-fill", fallback: "rgba(255, 255, 255, 0.94)" },
        portDefaultHalo: { cssVar: "--ui-canvas-port-default-halo", fallback: "rgba(148, 163, 184, 0.16)" },
        portDefaultLine: { cssVar: "--ui-canvas-port-default-line", fallback: "rgba(71, 85, 105, 0.82)" },
        portDefaultStroke: { cssVar: "--ui-canvas-port-default-stroke", fallback: "rgba(148, 163, 184, 0.34)" },
        portDefaultText: { cssVar: "--ui-canvas-port-default-text", fallback: "rgba(71, 85, 105, 0.96)" },
        portSuccessAccentColor: { cssVar: "--ui-canvas-port-success-accent-color", fallback: "#22c55e" },
        portSuccessAnchor: { cssVar: "--ui-canvas-port-success-anchor", fallback: "rgba(34, 197, 94, 0.92)" },
        portSuccessFill: { cssVar: "--ui-canvas-port-success-fill", fallback: "rgba(34, 197, 94, 0.18)" },
        portSuccessHalo: { cssVar: "--ui-canvas-port-success-halo", fallback: "rgba(34, 197, 94, 0.18)" },
        portSuccessLine: { cssVar: "--ui-canvas-port-success-line", fallback: "rgba(34, 197, 94, 0.86)" },
        portSuccessStroke: { cssVar: "--ui-canvas-port-success-stroke", fallback: "rgba(22, 163, 74, 0.48)" },
        portSuccessText: { cssVar: "--ui-canvas-port-success-text", fallback: "rgba(220, 252, 231, 0.98)" },
        portWarningAccentColor: { cssVar: "--ui-canvas-port-warning-accent-color", fallback: "#f59e0b" },
        portWarningAnchor: { cssVar: "--ui-canvas-port-warning-anchor", fallback: "rgba(245, 158, 11, 0.92)" },
        portWarningFill: { cssVar: "--ui-canvas-port-warning-fill", fallback: "rgba(245, 158, 11, 0.18)" },
        portWarningHalo: { cssVar: "--ui-canvas-port-warning-halo", fallback: "rgba(245, 158, 11, 0.18)" },
        portWarningLine: { cssVar: "--ui-canvas-port-warning-line", fallback: "rgba(245, 158, 11, 0.88)" },
        portWarningStroke: { cssVar: "--ui-canvas-port-warning-stroke", fallback: "rgba(217, 119, 6, 0.48)" },
        portWarningText: { cssVar: "--ui-canvas-port-warning-text", fallback: "rgba(254, 243, 199, 0.98)" },
        progressCompleteArc: { cssVar: "--ui-canvas-progress-complete-arc", fallback: "rgba(5, 150, 105, 0.92)" },
        progressInProgressArc: { cssVar: "--ui-canvas-progress-in-progress-arc", fallback: "rgba(124, 58, 237, 0.92)" },
        progressTextDefault: { cssVar: "--ui-canvas-progress-text-default", fallback: "rgba(15, 23, 42, 0.78)" },
        progressTrackDefault: { cssVar: "--ui-canvas-progress-track-default", fallback: "rgba(148, 163, 184, 0.28)" },
    };

    function readCanvasColorsInline(host, tokenMap) {
        const style = window.getComputedStyle(host);
        const resolved = {};
        for (const propertyName of Object.keys(tokenMap)) {
            const { cssVar, fallback } = tokenMap[propertyName];
            resolved[propertyName] = style.getPropertyValue(cssVar).trim() || fallback;
        }

        return resolved;
    }

    function resolveCanvasColors(host) {
        return root.themeTokens
            ? root.themeTokens.readTokens(host, canvasColorTokenMap)
            : readCanvasColorsInline(host, canvasColorTokenMap);
    }

    const shared = root.canvasWorkbenchModule = root.canvasWorkbenchModule || {};
    Object.assign(shared, { contextSubmenuHoverDelayMs, MIN_ZOOM, MAX_ZOOM, selectionModel, getRequiredRootService, getTextMeasureService, getViewportControllerService, getAnimationTimelineService, clear, createElement, createSvgElement, normalizeInputField, normalizeInputValue, clamp, debounce, now, createWorkbenchMetrics, formatMetricDuration, cloneWorkbenchMetrics, incrementMetric, resetLastDragPatchMetrics, recordDragPatchMetrics, round, normalizeAction, normalizeAnnotation, normalizeCompactPath, normalizeDiagnosticsOptions, normalizeMinimapOptions, normalizeClipboardOptions, normalizeTooltipPopoverOptions, normalizeMarqueeOptions, normalizeSnapGuideOptions, normalizeConnectorAnchorOptions, normalizeTransformHandleOptions, normalizeGroupFrame, normalizeProgressPercent, normalizeMenuActionScale, normalizeSurface, toSelectionSet, toCollapsedSet, getDefaultNodeSize, resolveBaseNodeSize, estimateNodeSizeFromText, getNodeSize, buildNodeLookup, isNodeVisible, getVisibleNodes, getProjectionOverscanPx, collectProjectedContextNodeIds, isNodeProjectedInViewport, getProjectedNodes, getBaseNodePosition, getNodeDepth, getNodeMobility, buildResolvedLayoutKey, buildLayoutItems, getCollisionPaddingX, getCollisionPaddingY, getOverlapDelta, chooseCollisionAxis, resolveCollisionDirection, applyCollisionSeparation, enforceParentClearance, enforceSiblingSpacing, relaxTowardBase, computeResolvedNodePositions, ensureLayoutPositions, getNodePosition, getSceneBounds, clampPanToScene, setPan, syncMenuScaleCss, serializeState, legacyApplySceneTransform, cancelViewportAnimation, updateViewportTransform, animateViewportTransition, canvasColorTokenMap, resolveCanvasColors });
})();
