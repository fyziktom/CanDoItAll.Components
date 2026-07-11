export function getCenter(element) {
    if (!element || !element.isConnected) {
        return { x: window.innerWidth / 2, y: window.innerHeight / 2 };
    }

    const rect = element.getBoundingClientRect();
    return {
        x: rect.left + (rect.width / 2),
        y: rect.top + (rect.height / 2)
    };
}
