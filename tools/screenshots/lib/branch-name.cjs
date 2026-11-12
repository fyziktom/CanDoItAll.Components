"use strict";

// Single source of truth for the {key}_{yyyy-MM-dd}_{HH-mm} (UTC) branch-name format, so
// git-storage.cjs's branch-listing glob, snapshot.cjs's branch creation, and report.cjs's
// "baseline branch ... (N minutes ago)" text all agree on one format. `key` (config.key,
// default "screenshots") namespaces branches within a shared storageRepoPath so multiple
// unrelated screenshot configs/runs don't treat each other's branches as their own baseline.

const DEFAULT_KEY = "screenshots";

function branchPrefix(key = DEFAULT_KEY) {
  return `${key}_`;
}

function branchPattern(key = DEFAULT_KEY) {
  const escapedKey = key.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
  return new RegExp(`^${escapedKey}_(\\d{4})-(\\d{2})-(\\d{2})_(\\d{2})-(\\d{2})$`);
}

function generateBranchName(key = DEFAULT_KEY, date = new Date()) {
  const pad = value => String(value).padStart(2, "0");
  const datePart = `${date.getUTCFullYear()}-${pad(date.getUTCMonth() + 1)}-${pad(date.getUTCDate())}`;
  const timePart = `${pad(date.getUTCHours())}-${pad(date.getUTCMinutes())}`;
  return `${branchPrefix(key)}${datePart}_${timePart}`;
}

// Returns the UTC Date encoded in a {key}_* branch name, or null if it doesn't match
// (e.g. a branch from before this naming scheme, a different key, or a manually created branch).
function parseBranchTimestamp(key, branchName) {
  const match = branchPattern(key).exec(branchName ?? "");
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

module.exports = { DEFAULT_KEY, branchPrefix, generateBranchName, parseBranchTimestamp, humanizeAgo };
