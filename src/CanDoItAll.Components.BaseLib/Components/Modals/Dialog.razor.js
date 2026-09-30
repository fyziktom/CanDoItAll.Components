const ON_DISMISS_REQUESTED = 'OnDialogDismissRequested';
const controllers = new Map();

let originalBodyOverflow;
let focusOwnershipInitialized = false;
let disabledFocusOwner;

export function initializeFocusOwnership() {
    if (focusOwnershipInitialized) {
        return;
    }

    focusOwnershipInitialized = true;
    const clearOwner = () => { disabledFocusOwner = undefined; };
    document.addEventListener('focusout', event => {
        disabledFocusOwner = event.target instanceof HTMLElement && event.target.matches(':disabled')
            ? new WeakRef(event.target)
            : undefined;
    }, true);
    document.addEventListener('focusin', clearOwner, true);
    document.addEventListener('pointerdown', clearOwner, true);
    document.addEventListener('keydown', clearOwner, true);
}

const removalObserver = new MutationObserver(() => {
    for (const [instanceId, controller] of controllers) {
        if (!controller.dialog.isConnected) {
            closeDialog(instanceId);
        }
    }
});

export function openDialog(dialog, instanceId, dotNetReference) {
    closeDialog(instanceId);
    if (!dialog.isConnected) {
        return;
    }

    const disabledOwner = disabledFocusOwner?.deref();
    const previousActiveElement = document.activeElement === document.body && disabledOwner?.isConnected
        ? disabledOwner
        : document.activeElement;
    disabledFocusOwner = undefined;
    const onCancel = async event => {
        event.preventDefault();

        try {
            await dotNetReference.invokeMethodAsync(ON_DISMISS_REQUESTED);
        } catch {
            if (controllers.get(instanceId) === controller) {
                closeDialog(instanceId);
            }
        }
    };

    const controller = { dialog, onCancel, previousActiveElement };
    dialog.addEventListener('cancel', onCancel);
    controllers.set(instanceId, controller);

    if (controllers.size === 1) {
        originalBodyOverflow = document.body.style.overflow;
        document.body.style.overflow = 'hidden';
        removalObserver.observe(document.body, { childList: true, subtree: true });
    }

    if (!dialog.open) {
        dialog.showModal();
    }

    // A dialog rendered inside another dialog's content belongs to it and stacks above it. When the owner opens after
    // such a descendant (both opened by one render, their module loads completing in either order), the owner's
    // showModal puts it on top of the top layer: raise the open descendants again, outermost first.
    for (const descendant of dialog.querySelectorAll('dialog[open]')) {
        descendant.close();
        descendant.showModal();
    }

    requestAnimationFrame(() => {
        if (controllers.get(instanceId) !== controller || !dialog.isConnected || !dialog.open) {
            return;
        }
        const initialFocus = dialog.querySelector(
            '[autofocus], button:not([disabled]), [href]:not([aria-disabled="true"]), input:not([disabled]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])');

        (initialFocus ?? dialog).focus({ preventScroll: true });
    });
}

export function closeDialog(instanceId) {
    const controller = controllers.get(instanceId);
    if (!controller) {
        return;
    }

    const { dialog, onCancel, previousActiveElement } = controller;
    const restoreFocus = dialog.contains(document.activeElement) || document.activeElement === document.body;
    dialog.removeEventListener('cancel', onCancel);

    if (dialog.open) {
        dialog.close();
    }

    controllers.delete(instanceId);

    if (controllers.size === 0) {
        removalObserver.disconnect();
        document.body.style.overflow = originalBodyOverflow ?? '';
        originalBodyOverflow = undefined;
    }

    if (restoreFocus && previousActiveElement instanceof HTMLElement && previousActiveElement.isConnected) {
        previousActiveElement.focus({ preventScroll: true });
    }
}
