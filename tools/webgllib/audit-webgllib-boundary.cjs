#!/usr/bin/env node
const { runBoundaryAudit } = require("./domain-boundary-auditor.cjs");

runBoundaryAudit({ defaultProfile: "webgllib" });
