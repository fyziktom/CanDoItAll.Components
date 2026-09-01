"use strict";

const fs = require("node:fs");
const path = require("node:path");
const os = require("node:os");
const crypto = require("node:crypto");
const { execFileSync } = require("node:child_process");
const { BRANCH_PREFIX } = require("./branch-name.cjs");

// All operations here run against `storageRepoPath` from the tool's config, which is a
// separate git repository from the one this tool lives in. Nothing in this file ever
// touches the caller's own working tree or branch.

function git(repoPath, args) {
  return execFileSync("git", ["-C", repoPath, ...args], { encoding: "utf8" }).trim();
}

function ensureRepo(repoPath) {
  const resolved = path.resolve(repoPath);
  if (!fs.existsSync(resolved)) {
    throw new Error(
      `storageRepoPath "${resolved}" does not exist. Create it once before first use, e.g.:\n  git init "${resolved}"`
    );
  }

  try {
    const isInsideWorkTree = git(resolved, ["rev-parse", "--is-inside-work-tree"]);
    if (isInsideWorkTree !== "true") {
      throw new Error("not inside a work tree");
    }
  } catch {
    throw new Error(`storageRepoPath "${resolved}" is not a git repository. Run "git init" there first.`);
  }

  return resolved;
}

function listScreenshotBranches(repoPath) {
  const resolved = ensureRepo(repoPath);
  let output;
  try {
    output = git(resolved, ["branch", "--list", `${BRANCH_PREFIX}*`, "--format=%(refname:short)"]);
  } catch {
    return [];
  }

  return output
    .split("\n")
    .map(line => line.trim())
    .filter(Boolean)
    .sort()
    .reverse();
}

function makeTempWorktreePath(prefix) {
  return path.join(fs.realpathSync(os.tmpdir()), `${prefix}-${crypto.randomUUID()}`);
}

function removeWorktree(repoPath, worktreePath) {
  try {
    git(repoPath, ["worktree", "remove", "--force", worktreePath]);
  } catch (error) {
    console.warn(`Warning: failed to remove worktree ${worktreePath}: ${error.message}`);
  } finally {
    fs.rmSync(worktreePath, { recursive: true, force: true });
  }
}

/** Commits `sourceDir`'s contents to a brand-new orphan branch, in an isolated worktree. */
function createSnapshotBranch(repoPath, branchName, sourceDir) {
  const resolved = ensureRepo(repoPath);
  const worktreePath = makeTempWorktreePath("visual-snapshot");

  git(resolved, ["worktree", "add", "--orphan", "-b", branchName, worktreePath]);
  try {
    fs.cpSync(sourceDir, worktreePath, { recursive: true });
    git(worktreePath, ["add", "-A"]);
    git(worktreePath, [
      "-c", "user.name=screenshots-bot",
      "-c", "user.email=screenshots-bot@local",
      "commit", "-m", `Screenshots ${branchName}`,
    ]);
  } finally {
    removeWorktree(resolved, worktreePath);
  }

  return branchName;
}

/** Checks out an existing branch read-only into a temp worktree. Caller must call cleanup(). */
function checkoutBranchToTemp(repoPath, branchName) {
  const resolved = ensureRepo(repoPath);
  const worktreePath = makeTempWorktreePath("visual-baseline");
  git(resolved, ["worktree", "add", "--detach", worktreePath, branchName]);
  return {
    path: worktreePath,
    cleanup: () => removeWorktree(resolved, worktreePath),
  };
}

function deleteBranch(repoPath, branchName, { remote = false } = {}) {
  const resolved = ensureRepo(repoPath);
  git(resolved, ["branch", "-D", branchName]);
  if (remote) {
    git(resolved, ["push", "origin", "--delete", branchName]);
  }
}

module.exports = {
  ensureRepo,
  listScreenshotBranches,
  createSnapshotBranch,
  checkoutBranchToTemp,
  deleteBranch,
};
