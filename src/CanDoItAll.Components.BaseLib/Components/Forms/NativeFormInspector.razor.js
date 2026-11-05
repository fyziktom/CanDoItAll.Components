export function inspect(formId, sectionIndexAttribute, panelsSelector) {
    const form = requireForm(formId);
    const invalid = Array.from(form.elements).find(element => element.willValidate && !element.validity.valid);
    let sectionIndex = null;
    if (invalid && sectionIndexAttribute) {
        const section = invalid.closest('[' + sectionIndexAttribute + ']');
        if (section) {
            const value = Number(section.getAttribute(sectionIndexAttribute));
            if (!Number.isSafeInteger(value) || value < 0) {
                throw new Error('The form section index must be a nonnegative integer.');
            }
            sectionIndex = value;
        }
    }
    if (invalid && sectionIndex === null && panelsSelector) {
        const panels = form.querySelector(panelsSelector);
        const index = panels ? Array.from(panels.children).findIndex(panel => panel.contains(invalid)) : -1;
        sectionIndex = index < 0 ? null : index;
    }
    return { isValid: !invalid, sectionIndex };
}

export function focusInvalid(formId) {
    requireForm(formId).reportValidity();
}

function requireForm(formId) {
    const form = document.getElementById(formId);
    if (!(form instanceof HTMLFormElement)) {
        throw new Error('The active editor form is unavailable.');
    }
    return form;
}
