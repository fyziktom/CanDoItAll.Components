"use strict";

const assert = require("node:assert/strict");

global.window = { CanDoItAll: {} };

const {
    dependencyArrowLength,
    resolveObstacleSafeRoute,
    assertDependencyRouteAvoidsUnrelatedTasks
} = require("../../src/CanDoItAll.Components.Gantt/wwwroot/js/gantt-chart.js");

const options = Object.freeze({
    dependencyRouteClearance: 4,
    headerHeight: 40,
    rowHeight: 48,
    timelineWidth: 1_000
});

function taskRect(taskId, index, x, width) {
    return {
        taskId,
        x,
        y: options.headerHeight + (index * options.rowHeight) + 10,
        width,
        height: 28
    };
}

function endpoint(attachmentX, handleX, y) {
    return { attachmentX, handleX, y };
}

function assertSafeRoute(
    id,
    taskRects,
    predecessorIndex,
    successorIndex,
    source,
    target,
    originX = 0) {
    const routePoints = resolveObstacleSafeRoute(
        options,
        taskRects,
        predecessorIndex,
        successorIndex,
        source,
        target,
        originX);
    const geometry = {
        dependency: { id },
        routePoints,
        targetAttachmentX: target.attachmentX,
        targetY: target.y
    };

    assert.doesNotThrow(() => assertDependencyRouteAvoidsUnrelatedTasks(
        geometry,
        taskRects,
        taskRects[predecessorIndex].taskId,
        taskRects[successorIndex].taskId));
    assert.deepEqual(routePoints.at(-1), {
        x: target.attachmentX - dependencyArrowLength,
        y: target.y
    });
    return routePoints;
}

const spanningRects = [
    taskRect("source", 0, 100, 100),
    taskRect("intervening", 1, 250, 180),
    taskRect("target", 2, 420, 120)
];
const spanningRoute = assertSafeRoute(
    "spanning",
    spanningRects,
    0,
    2,
    endpoint(200, 206, 64),
    endpoint(420, 414, 160));
assert.ok(
    spanningRoute.some(point => point.x <= 246),
    "The forward route must move its vertical lane outside the intervening bar.");

const fullyBlockedRects = [
    taskRect("blocked-source", 0, 100, 100),
    taskRect("blocked-intervening", 1, 80, 420),
    taskRect("blocked-target", 2, 420, 120)
];
const fullyBlockedRoute = assertSafeRoute(
    "fully-blocked",
    fullyBlockedRects,
    0,
    2,
    endpoint(200, 206, 64),
    endpoint(420, 414, 160));
assert.ok(
    fullyBlockedRoute.some(point => point.x <= 76 || point.x >= 504),
    "A bar covering every direct lane must force an outer-spine route.");

const touchingRects = [
    taskRect("touching-source", 0, 100, 100),
    taskRect("touching-intervening", 1, 140, 160),
    taskRect("touching-target", 2, 200, 80)
];
const touchingRoute = assertSafeRoute(
    "touching",
    touchingRects,
    0,
    2,
    endpoint(200, 206, 64),
    endpoint(200, 194, 160));
assert.ok(
    touchingRoute.length >= 7,
    "Tasks meeting at the same timestamp need a boundary dogleg instead of a bar-crossing elbow.");

const adjacentRects = [
    taskRect("adjacent-source", 0, 100, 100),
    taskRect("adjacent-target", 1, 200, 100)
];
const adjacentRoute = assertSafeRoute(
    "adjacent-touching",
    adjacentRects,
    0,
    1,
    endpoint(200, 206, 64),
    endpoint(200, 194, 112));
assert.ok(
    adjacentRoute.every(point => point.x >= 192 && point.x <= 206),
    "Adjacent tasks need a compact row-boundary dogleg instead of a long outer-spine detour.");

const denseRects = [];
for (let index = 0; index < 8; index += 1) {
    denseRects.push(taskRect(`predecessor-${index}`, index, 80, 100));
}
denseRects.push(taskRect("hub", 8, 180, 80));
for (let index = 0; index < 8; index += 1) {
    denseRects.push(taskRect(`successor-${index}`, index + 9, 260, 80));
}

for (let index = 0; index < 8; index += 1) {
    assertSafeRoute(
        `incoming-${index}`,
        denseRects,
        index,
        8,
        endpoint(180, 186 + index, 64 + (index * options.rowHeight)),
        endpoint(180, 174 - index, 448));
    assertSafeRoute(
        `outgoing-${index}`,
        denseRects,
        8,
        index + 9,
        endpoint(260, 266 + index, 448),
        endpoint(260, 254 - index, 496 + (index * options.rowHeight)));
}

const exportOriginX = 600;
const exportRects = [
    taskRect("export-source", 0, 1_200, 100),
    taskRect("export-intervening", 1, 700, 600),
    taskRect("export-target", 2, 1_200, 100)
];
const exportRoute = assertSafeRoute(
    "task-table-export",
    exportRects,
    0,
    2,
    endpoint(1_300, 1_306, 64),
    endpoint(1_200, 1_194, 160),
    exportOriginX);
assert.ok(
    exportRoute.some(point => point.x >= 1_304),
    "Export routing must clamp its right spine against the shifted timeline edge, not the unshifted canvas edge.");

process.stdout.write("Gantt dependency routing: spanning, touching, adjacent, dense 8-in/8-out, and task-table export routes are obstacle-safe.\n");
