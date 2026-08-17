const ON_DISMISS_REQUESTED = 'OnDialogDismissRequested';
const controllers = new Map();

let originalBodyOverflow;

export function openDialog(dialog, instanceId, dotNetReference) {
    closeDialog(instanceId);

    const previousActiveElement = document.activeElement;
    const onCancel = async event => {
        event.preventDefault();

        try {
            await dotNetReference.invokeMethodAsync(ON_DISMISS_REQUESTED);
        } catch {
            // The Blazor circuit may have disconnected while the dialog was open.
        }
    };

    dialog.addEventListener('cancel', onCancel);
    controllers.set(instanceId, { dialog, onCancel, previousActiveElement });

    if (controllers.size === 1) {
        originalBodyOverflow = document.body.style.overflow;
        document.body.style.overflow = 'hidden';
    }

    if (!dialog.open) {
        dialog.showModal();
    }

    requestAnimationFrame(() => {
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
    dialog.removeEventListener('cancel', onCancel);

    if (dialog.open) {
        dialog.close();
    }

    controllers.delete(instanceId);

    if (controllers.size === 0) {
        document.body.style.overflow = originalBodyOverflow ?? '';
        originalBodyOverflow = undefined;
    }

    if (previousActiveElement instanceof HTMLElement && previousActiveElement.isConnected) {
        previousActiveElement.focus({ preventScroll: true });
    }
}
