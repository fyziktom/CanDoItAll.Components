(function () {
    const root = window.CanDoItAll = window.CanDoItAll || {};
    const shared = root.canvasWorkbenchModule;
    if (!shared) { throw new Error("CanvasLib workbench foundation must load before keyboard commands."); }

    function selectedToolbarNode(state) {
        if (state.selectedIds?.size !== 1) {
            return null;
        }

        const node = state.lookups?.byId?.get([...state.selectedIds][0]);
        return node?.toolbarActions?.length && shared.isNodeVisible(state, node.id) ? node : null;
    }

    function closeNodeToolbar(state, restoreFocus = false) {
        if (!state.nodeToolbarNodeId) {
            return false;
        }

        state.nodeToolbarNodeId = null;
        state.nodeToolbarActions = null;
        if (state.nodeToolbar) {
            state.nodeToolbar.hidden = true;
            state.nodeToolbar.replaceChildren();
        }
        if (restoreFocus) {
            shared.ensureHostFocus(state);
        }
        return true;
    }

    function invokeToolbarAction(state, node, action, anchor) {
        closeNodeToolbar(state, true);
        if (action.children?.length) {
            shared.openNodeMetadataMenu(state, node, action.actionId, anchor);
            return;
        }

        shared.executeContextAction(state, node, action, anchor.left, anchor.top, "child");
    }

    function createNodeToolbar(state) {
        const toolbar = state.document.createElement("div");
        toolbar.className = "cw-node-toolbar";
        toolbar.setAttribute("role", "toolbar");
        toolbar.setAttribute("aria-label", "Selected node actions");
        toolbar.hidden = true;
        toolbar.addEventListener("pointerdown", event => event.stopPropagation());
        toolbar.addEventListener("click", event => {
            const button = event.target.closest?.("button[data-action-id]");
            const node = selectedToolbarNode(state);
            if (!button || !node || node.id !== state.nodeToolbarNodeId) {
                return;
            }

            const action = node.toolbarActions.find(candidate => candidate.actionId === button.dataset.actionId);
            if (!action) {
                return;
            }

            event.preventDefault();
            event.stopPropagation();
            invokeToolbarAction(state, node, action, button.getBoundingClientRect());
        });
        state.host.appendChild(toolbar);
        state.nodeToolbar = toolbar;
        state.nodeToolbarOutside = event => {
            if (!toolbar.contains(event.target) && !event.target.closest?.("[data-cw-node-actions]")) {
                closeNodeToolbar(state);
            }
        };
        state.nodeToolbarScroll = () => updateNodeToolbar(state);
        state.document.addEventListener("pointerdown", state.nodeToolbarOutside, true);
        state.document.addEventListener("scroll", state.nodeToolbarScroll, true);
    }

    function updateNodeToolbar(state) {
        if (!state.nodeToolbarNodeId) {
            return;
        }

        const node = selectedToolbarNode(state);
        if (!node || node.id !== state.nodeToolbarNodeId) {
            closeNodeToolbar(state, state.nodeToolbar?.contains(state.document.activeElement));
            return;
        }

        const toolbar = state.nodeToolbar;
        if (state.nodeToolbarActions !== node.toolbarActions) {
            const focusedAction = toolbar.contains(state.document.activeElement)
                ? state.document.activeElement.dataset.actionId
                : null;
            toolbar.replaceChildren();
            for (const action of node.toolbarActions) {
                const button = state.document.createElement("button");
                button.type = "button";
                button.className = "cw-toolbar-action cw-node-toolbar__action";
                button.dataset.actionId = action.actionId;
                button.dataset.tone = action.tone || "neutral";
                button.setAttribute("aria-label", action.label);
                button.title = action.keyboardCommand === "CopyIdentifier"
                    ? `${action.description || action.label} (Ctrl+I)`
                    : action.description || action.label;
                if (action.icon) {
                    const icon = shared.createMenuActionIcon(state,
                        action.icon.toLowerCase() === "delete" ? { ...action, icon: "clear" } : action);
                    icon.setAttribute("aria-hidden", "true");
                    button.appendChild(icon);
                }
                const label = state.document.createElement("span");
                label.textContent = action.label;
                button.appendChild(label);
                toolbar.appendChild(button);
            }
            state.nodeToolbarActions = node.toolbarActions;
            if (focusedAction) {
                const nextFocus = [...toolbar.children].find(button => button.dataset.actionId === focusedAction)
                    || toolbar.querySelector("button");
                nextFocus?.focus({ preventScroll: true });
            }
        }

        toolbar.hidden = false;
        const point = shared.worldToHostPoint(state, shared.getNodePosition(state, node));
        const size = shared.getNodeSize(state, node);
        const hostRect = state.host.getBoundingClientRect();
        const margin = 12;
        const viewport = state.document.documentElement;
        const minX = Math.max(margin, margin - hostRect.left);
        const maxX = Math.min(hostRect.width, viewport.clientWidth - hostRect.left) - margin;
        const minY = Math.max(margin, margin - hostRect.top, shared.resolveContextMenuSafeTop(state));
        const maxY = Math.min(hostRect.height, viewport.clientHeight - hostRect.top) - margin;
        toolbar.style.maxWidth = `${Math.max(0, maxX - minX)}px`;
        const width = toolbar.offsetWidth;
        const height = toolbar.offsetHeight;
        const above = point.y - size.height * state.ui.zoom / 2 - height - margin;
        const below = point.y + size.height * state.ui.zoom / 2 + margin;
        toolbar.style.left = `${Math.max(minX, Math.min(point.x - width / 2, maxX - width))}px`;
        toolbar.style.top = `${Math.max(minY, Math.min(above >= minY ? above : below, maxY - height))}px`;
    }

    function toggleNodeToolbar(state, shouldDispatch = true) {
        const node = selectedToolbarNode(state);
        if (!node) {
            return false;
        }
        if (!shouldDispatch) {
            return true;
        }
        if (closeNodeToolbar(state, true)) {
            return true;
        }

        shared.clearContextMenu(state);
        shared.hidePopover(state);
        state.nodeToolbarNodeId = node.id;
        updateNodeToolbar(state);
        state.nodeToolbar.querySelector("button")?.focus({ preventScroll: true });
        return true;
    }

    function requestIdentifierCopy(state, shouldDispatch) {
        const node = selectedToolbarNode(state);
        const action = node?.toolbarActions.find(candidate => candidate.keyboardCommand === "CopyIdentifier");
        if (!action) {
            return false;
        }
        if (shouldDispatch) {
            const anchor = state.host.getBoundingClientRect();
            invokeToolbarAction(state, node, action, anchor);
        }
        return true;
    }

    const commandBindings = Object.freeze([
        { key: "x", command: (state, dispatch) => shared.requestClipboardCut(state, dispatch) },
        { key: "c", command: (state, dispatch) => shared.copySelectionToClipboard(state, dispatch) },
        { key: "v", command: (state, dispatch) => shared.requestClipboardPaste(state, dispatch) },
        { key: "d", command: (state, dispatch) => shared.requestClipboardDuplicate(state, dispatch) },
        { key: "m", controlOnly: true, command: toggleNodeToolbar },
        { key: "i", controlOnly: true, command: requestIdentifierCopy }
    ]);

    function dismissCanvasTransientUi(state) {
        if (closeNodeToolbar(state, true)) {
            return;
        }
        if (state.helpOpen) {
            shared.toggleHelp(state);
            shared.render(state);
            shared.ensureHostFocus(state);
            return;
        }

        const activeMode = shared.resolveSurfaceMode(state);
        const hadContextMenu = state.contextMenu?.style.display !== "none";
        const hadComposer = !!state.composer;
        const hadConnectionDraft = !!state.connectionDraft;
        shared.clearContextMenu(state);
        shared.closeComposer(state);
        shared.clearConnectionDraft?.(state, false);
        shared.clearNodeHighlights?.(state, { render: true, publish: true });
        if (activeMode !== "authoring") {
            shared.setWorkbenchToolMode(state, "authoring");
            shared.dispatchContextActionRequest(state, {
                nodeId: null, actionId: "tool-mode:select", x: 0, y: 0, targetKind: "canvas"
            });
        } else if (!hadContextMenu && !hadComposer && !hadConnectionDraft) {
            shared.setSelection(state, [], true);
        }
        shared.render(state);
        shared.ensureHostFocus(state);
    }

    function routeWorkbenchKeyDown(state, event) {
        const target = event.target;
        if (state.isDisposed || event.defaultPrevented || event.isComposing ||
            !state.host.contains(target) || target.closest?.(".cw-canvas-host") !== state.host) {
            return false;
        }

        const editable = target.closest?.("input,textarea,select,[contenteditable]:not([contenteditable='false']),[role='textbox']");
        if (editable || target.isContentEditable) {
            if (event.key === "Escape" && state.composer?.element?.contains(target)) {
                event.preventDefault();
                if (!event.repeat) {
                    shared.closeComposer(state);
                    shared.render(state);
                    shared.ensureHostFocus(state);
                }
                return true;
            }
            return false;
        }

        const inToolbar = state.nodeToolbar?.contains(target);
        const inMenu = state.contextMenu?.contains(target);
        const inComposer = state.composer?.element?.contains(target);
        const overlay = target.closest?.("dialog,[role='dialog'],.cw-floating-window");
        if (overlay && state.host.contains(overlay) && !inComposer) {
            return false;
        }
        if (!inToolbar && !inComposer && shared.routeContextMenuShortcut?.(state, event)) {
            return true;
        }

        if ((event.ctrlKey || event.metaKey) && !event.altKey && !event.shiftKey && !inComposer && !inMenu) {
            const binding = commandBindings.find(candidate => candidate.key === event.key.toLowerCase() &&
                (!candidate.controlOnly || event.ctrlKey && !event.metaKey));
            if (binding?.command(state, !event.repeat)) {
                event.preventDefault();
                event.stopPropagation();
                return true;
            }
            return false;
        }
        if (event.ctrlKey || event.metaKey || event.altKey) {
            return false;
        }
        if (event.key === "Escape") {
            event.preventDefault();
            if (!event.repeat) {
                dismissCanvasTransientUi(state);
            }
            return true;
        }
        if (inToolbar || inMenu || inComposer || target.closest?.("button,a[href],[role='button'],[role='tab']")) {
            return false;
        }

        const selectedId = state.ui?.selectedNodeIds?.length === 1 ? state.ui.selectedNodeIds[0] : null;
        const selectedNode = selectedId ? state.lookups?.byId?.get(selectedId) : null;
        if ((event.key === "Delete" || event.key === "Backspace") && !event.shiftKey && selectedNode?.isInlineTextNode) {
            event.preventDefault();
            if (!event.repeat) {
                shared.clearContextMenu(state);
                shared.dispatchContextActionRequest(state, { nodeId: selectedId, actionId: "delete", x: 0, y: 0, targetKind: "node" });
            }
            return true;
        }

        const key = event.key.toLowerCase();
        const stageCommands = {
            "+": () => shared.setZoomPercent(state, state.ui.zoom * 100 + 10),
            "=": () => shared.setZoomPercent(state, state.ui.zoom * 100 + 10),
            "-": () => shared.setZoomPercent(state, state.ui.zoom * 100 - 10),
            "0": () => shared.fitView(state),
            "?": () => shared.toggleHelp(state),
            "h": () => shared.toggleHelp(state),
            "d": () => shared.toggleDiagnostics(state),
            "m": () => shared.toggleMinimap(state),
            "tab": () => shared.openKeyboardNoteComposer(state, "child"),
            "enter": () => shared.openKeyboardNoteComposer(state, "sibling")
        };
        if ((key === "tab" || key === "enter") && event.shiftKey) {
            return false;
        }
        const command = stageCommands[key];
        if (!command) {
            return false;
        }
        event.preventDefault();
        if (!event.repeat || key === "+" || key === "=" || key === "-") {
            command();
        }
        return true;
    }

    Object.assign(shared, { createNodeToolbar, updateNodeToolbar, closeNodeToolbar, toggleNodeToolbar, routeWorkbenchKeyDown });
})();
