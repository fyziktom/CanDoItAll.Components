"use strict";

const fs = require("node:fs");
const path = require("node:path");

const REQUIRED_KEYS = ["baseUrl", "viewports", "themes", "alternatives", "storageRepoPath", "outputDir"];

function loadConfig(configPath) {
  const resolved = path.resolve(configPath);
  if (!fs.existsSync(resolved)) {
    throw new Error(`Config file not found: ${resolved}`);
  }

  const configDir = path.dirname(resolved);
  const config = JSON.parse(fs.readFileSync(resolved, "utf8"));
  validateConfig(config, resolved);

  return {
    ...config,
    configDir,
    outputDir: path.resolve(configDir, config.outputDir),
    storageRepoPath: path.resolve(configDir, config.storageRepoPath),
  };
}

function validateConfig(config, resolvedPath) {
  for (const key of REQUIRED_KEYS) {
    if (!(key in config)) {
      throw new Error(`Missing required config key "${key}" in ${resolvedPath}`);
    }
  }

  if (!config.routesEndpoint && !(Array.isArray(config.routes) && config.routes.length > 0)) {
    throw new Error(`Config must define at least one of "routesEndpoint" or a non-empty "routes" array in ${resolvedPath}`);
  }

  if (!Array.isArray(config.viewports) || config.viewports.length === 0) {
    throw new Error(`Config "viewports" must be a non-empty array in ${resolvedPath}`);
  }

  if (typeof config.themes !== "object" || config.themes === null || Array.isArray(config.themes) || Object.keys(config.themes).length === 0) {
    throw new Error(`Config "themes" must be a non-empty object mapping theme name to a query-string fragment (e.g. {"light": "", "dark": "dark=true"}) in ${resolvedPath}`);
  }

  if (typeof config.alternatives !== "object" || config.alternatives === null || Array.isArray(config.alternatives) || Object.keys(config.alternatives).length === 0) {
    throw new Error(`Config "alternatives" must be a non-empty object mapping alternative name to a query-string fragment (e.g. {"default": "", "dense": "scenario=dense-content"}) in ${resolvedPath}`);
  }

  if (config.sessionStorage !== undefined &&
      (typeof config.sessionStorage !== "object" || config.sessionStorage === null || Array.isArray(config.sessionStorage) ||
       Object.values(config.sessionStorage).some(value => typeof value !== "string"))) {
    throw new Error(`Config "sessionStorage" must be an object mapping storage keys to string values in ${resolvedPath}`);
  }
}

const LOCAL_HOSTNAMES = new Set(["localhost", "127.0.0.1", "::1"]);

// ASP.NET Core's local dev HTTPS certificate is self-signed, which makes Node's built-in
// fetch reject it ("TypeError: fetch failed", cause DEPTH_ZERO_SELF_SIGNED_CERT) even though
// Playwright is separately told to ignore it (see capture.cjs). Relax cert verification only
// for localhost/127.0.0.1/::1 requests, and only for the duration of this one call.
async function fetchAllowingLocalSelfSignedCerts(url, options) {
  if (!LOCAL_HOSTNAMES.has(new URL(url).hostname)) {
    return fetch(url, options);
  }

  const previous = process.env.NODE_TLS_REJECT_UNAUTHORIZED;
  process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";
  try {
    return await fetch(url, options);
  } finally {
    if (previous === undefined) {
      delete process.env.NODE_TLS_REJECT_UNAUTHORIZED;
    } else {
      process.env.NODE_TLS_REJECT_UNAUTHORIZED = previous;
    }
  }
}

async function resolveRoutes(config) {
  const routes = [];
  const seenPaths = new Set();

  const addRoute = route => {
    if (!route || !route.path || seenPaths.has(route.path)) {
      return;
    }
    seenPaths.add(route.path);
    routes.push({ path: route.path, title: route.title ?? route.path, group: route.group ?? "ungrouped" });
  };

  for (const route of config.routes ?? []) {
    addRoute(route);
  }

  if (config.routesEndpoint) {
    const url = new URL(config.routesEndpoint, config.baseUrl).toString();
    const response = await fetchAllowingLocalSelfSignedCerts(url, { headers: { accept: "application/json" } });
    if (!response.ok) {
      throw new Error(`Failed to fetch routesEndpoint ${url}: ${response.status} ${response.statusText}`);
    }
    const fetched = await response.json();
    for (const entry of fetched) {
      addRoute(entry);
    }
  }

  return routes;
}

// A job's screenshot lives at current/{combo}/{page}.png, e.g.
// current/desktop_light_default/actions.png. "page" identifies the route (from its title,
// which is unique per route — "group" is just a coarse category like "Home"/"Components" and
// is not necessarily unique) and "combo" identifies the viewport/theme/alternative combination.
function buildJobs(config, routes) {
  const jobs = [];
  for (const route of routes) {
    for (const viewport of config.viewports) {
      for (const [themeName, themeQuery] of Object.entries(config.themes)) {
        for (const [alternativeName, alternativeQuery] of Object.entries(config.alternatives)) {
          const combo = [viewport.name, themeName, alternativeName].map(slugify).join("_");
          const page = slugify(route.title);
          const file = `${combo}/${page}.png`;
          jobs.push({
            key: `${combo}/${page}`,
            combo,
            page,
            file,
            route,
            viewport,
            themeName,
            themeQuery,
            alternativeName,
            alternativeQuery,
            url: buildUrl(config.baseUrl, route.path, viewport, themeQuery, alternativeQuery),
          });
        }
      }
    }
  }
  return jobs;
}

function buildUrl(baseUrl, routePath, viewport, themeQuery, alternativeQuery) {
  const url = new URL(routePath, baseUrl);

  if (viewport.frameQuery && !url.searchParams.has("frame")) {
    url.searchParams.set("frame", viewport.frameQuery);
  }

  applyQueryFragment(url, themeQuery);
  applyQueryFragment(url, alternativeQuery);

  return url.toString();
}

function applyQueryFragment(url, fragment) {
  if (!fragment) {
    return;
  }

  const [key, value] = fragment.split("=");
  if (key && !url.searchParams.has(key)) {
    url.searchParams.set(key, value ?? "");
  }
}

function slugify(value) {
  const slug = String(value)
    .trim()
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "");
  return slug || "x";
}

module.exports = { loadConfig, resolveRoutes, buildJobs, buildUrl, slugify };
