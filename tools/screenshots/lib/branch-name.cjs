"use strict";

// Single source of truth for the screenshots_{yyyy-MM-dd}_{HH-mm} (UTC) branch-name format,
// so git-storage.cjs's branch-listing glob, snapshot.cjs's branch creation, and report.cjs's
// "baseline branch ... (N minutes ago)" text all agree on one format.

const BRANCH_PREFIX = "screenshots_";
const BRANCH_PATTERN = /^screenshots_(\d{4})-(\d{2})-(\d{2})_(\d{2})-(\d{2})$/;

function generateBranchName(date = new Date()) {
  const pad = value => String(value).padStart(2, "0");
  const datePart = `${date.getUTCFullYear()}-${pad(date.getUTCMonth() + 1)}-${pad(date.getUTCDate())}`;
  const timePart = `${pad(date.getUTCHours())}-${pad(date.getUTCMinutes())}`;
  return `${BRANCH_PREFIX}${datePart}_${timePart}`;
}

// Returns the UTC Date encoded in a screenshots_* branch name, or null if it doesn't match
// (e.g. a branch from before this naming scheme, or a manually created branch).
function parseBranchTimestamp(branchName) {
  const match = BRANCH_PATTERN.exec(branchName ?? "");
  if (!match) {
    return null;
  }

  const [, year, month, day, hour, minute] = match.map(Number);
  return new Date(Date.UTC(year, month - 1, day, hour, minute));
}

function humanizeAgo(pastDate, now = new Date()) {
  const diffMs = Math.max(0, now.getTime() - pastDate.getTime());
  const minutes = Math.round(diffMs / 60000);

  if (minutes < 1) {
    return "just now";
  }
  if (minutes < 60) {
    return `${minutes} minute${minutes === 1 ? "" : "s"} ago`;
  }

  const hours = Math.round(minutes / 60);
  if (hours < 24) {
    return `${hours} hour${hours === 1 ? "" : "s"} ago`;
  }

  const days = Math.round(hours / 24);
  return `${days} day${days === 1 ? "" : "s"} ago`;
}

module.exports = { BRANCH_PREFIX, generateBranchName, parseBranchTimestamp, humanizeAgo };
