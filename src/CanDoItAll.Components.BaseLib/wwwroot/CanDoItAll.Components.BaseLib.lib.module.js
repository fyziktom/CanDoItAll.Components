import { initializeFocusOwnership } from './Components/Modals/Dialog.razor.js';

export const beforeWebStart = initializeFocusOwnership;
export const beforeStart = initializeFocusOwnership;

const inputRegistrations = new WeakSet();

function registerNativeInput(blazor) {
    if (inputRegistrations.has(blazor)) {
        return;
    }
    inputRegistrations.add(blazor);
    blazor.registerCustomEventType('cadnativeinput', {
        browserEventName: 'input',
        createEventArgs: event => ({ value: event.target.value, badInput: event.target.validity.badInput })
    });
}

export const afterWebStarted = registerNativeInput;
export const afterStarted = registerNativeInput;
