"use strict";

const assert = require("node:assert/strict");

global.window = { CanDoItAll: {} };

const {
    resolveTickLabelInterval
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

process.stdout.write("Gantt grid labels remain readable without changing quarter-hour, hourly, or daily grid resolution.\n");
