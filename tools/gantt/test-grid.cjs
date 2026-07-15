"use strict";

const assert = require("node:assert/strict");

global.window = { CanDoItAll: {} };

const {
    resolveTickLabelInterval,
    resolveTimelineDoubleClick
} = require("../../src/CanDoItAll.Components.Gantt/wwwroot/js/gantt-chart.js");

const minuteMs = 60_000;
const hourMs = 60 * minuteMs;
const dayMs = 24 * hourMs;

function model(pixelsPerHour) {
    return {
        options: {
            timelineStartMs: Date.UTC(2026, 6, 15),
            timelineEndMs: Date.UTC(2026, 6, 18),
            pixelsPerHour
        }
    };
}

const labelWidth = () => 96;

assert.equal(
    resolveTickLabelInterval(model(96), 15 * minuteMs, labelWidth),
    2 * hourMs,
    "Quarter-hour grids need a coarser two-hour label cadence when full UTC labels are 96px wide.");
assert.equal(
    resolveTickLabelInterval(model(32), hourMs, labelWidth),
    4 * hourMs,
    "Hourly grids need a four-hour label cadence when full UTC labels are 96px wide.");
assert.equal(
    resolveTickLabelInterval(model(14), dayMs, labelWidth),
    dayMs,
    "Daily grids already have enough space to label every grid line.");
assert.equal(
    resolveTickLabelInterval({
        options: {
            timelineStartMs: Date.UTC(2026, 6, 15),
            timelineEndMs: Date.UTC(2026, 6, 15, 0, 30),
            pixelsPerHour: 32
        }
    }, 15 * minuteMs, labelWidth),
    4 * hourMs,
    "Short timelines still need a label interval wide enough for the rendered text.");

const timelineStartMs = Date.UTC(2026, 6, 15, 8);
const interactionModel = {
    tasks: [
        { id: "first" },
        { id: "second" }
    ],
    options: {
        headerHeight: 40,
        rowHeight: 48,
        canvasHeight: 136,
        timelineWidth: 1_000,
        timelineGutter: 20,
        timelineStartMs,
        timelineEndMs: timelineStartMs + (30 * hourMs),
        pixelsPerHour: 32,
        snapMs: hourMs,
        snapOriginMs: 0
    }
};

assert.deepEqual(
    resolveTimelineDoubleClick(interactionModel, {
        x: 20 + (10.4 * 32),
        y: 40 + 48 + 24
    }),
    {
        rowTaskId: "second",
        clickedAtMs: timelineStartMs + (10 * hourMs)
    },
    "An empty-row double click must resolve the visible row and snap its UTC time to the configured grid.");
assert.equal(
    resolveTimelineDoubleClick(interactionModel, { x: 200, y: 39 }),
    null,
    "The timeline header is not a task-creation row.");
assert.equal(
    resolveTimelineDoubleClick(interactionModel, { x: 19, y: 64 }),
    null,
    "The dependency gutter is not a datetime target.");
assert.equal(
    resolveTimelineDoubleClick(interactionModel, { x: 981, y: 64 }),
    null,
    "The trailing dependency gutter is not a datetime target.");
assert.equal(
    resolveTimelineDoubleClick(interactionModel, { x: 200, y: 136 }),
    null,
    "Space below the last task row has no task anchor.");

process.stdout.write("Gantt grid labels and empty-row UTC activation remain aligned to the configured timeline.\n");
