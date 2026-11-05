import { activeNotificationOutlet, DIALOG_STACK_CHANGED } from '../Modals/Dialog.razor.js';

const roots = new Map();

export function connect(id, reference) {
    disconnect(id);
    const changed = () => reference.invokeMethodAsync('OnNotificationLayerChanged', activeNotificationOutlet()).catch(error => {
        if (roots.get(id) === changed) {
            console.error('Notification outlet synchronization failed.', error);
        }
    });
    roots.set(id, changed);
    document.addEventListener(DIALOG_STACK_CHANGED, changed);
    changed();
}

export function disconnect(id) {
    const changed = roots.get(id);
    if (changed) {
        document.removeEventListener(DIALOG_STACK_CHANGED, changed);
        roots.delete(id);
    }
}
