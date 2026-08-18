# PR Notes

## Sandbox App

- reorganized sandbox groups to match folders in [CanDoItAll.Components.Sandbox/Components](./samples/CanDoItAll.Components.Sandbox/Components)
- reorganized sandbox group content around components
- modernized the Sandbox look [from](https://github.com/darosh/CanDoItAll.Components.Screenshots/blob/screenshots_2026-08-13_10-49/current/desktop_light_default/README.md) ... to [see latest branch here](https://github.com/darosh/CanDoItAll.Components.Screenshots)
- added toolbar
- viewport and theme selection moved to toolbar
- viewport and theme selection is persistent across navigation
- removed scenario selection, examples are demoing diferents states directly
- removed poetic LLM lingo artifacts "calm laboratory" and so on
- added search as you type filter to side nav
- removed coverage page
- coverage notes aggregate from two sources, now avaliable under "Coverage" toolbar button
- added watch script to package.json
- added logo [./assets/logo](./assets/logo)
- added /api/pages.json endpoint for screenshot automation
- added missing components to sandbox (around 110+ ones)
- replaced home page / with TOC and direct links to component examples
- added wasm version of the app
- added GitHub pages deploy script
- added Usage filter

## Sanbox App Examples

- added *Example*, **Code**, **Api** tabs
- added **Copy code** button
- added link to source on GitHub

## Components

- added `/// <summary>` tags (shown in API tab in the sanbox app) to 30% of components 
- added RoboAvatar and HomoAvatar experimantal components
- migrated to material symbols
- added Noto Sans and Noto Mono fonts
- changed MaterialIconCatalogGallery to expose the icon dictionary and added complete preview to sandbox app
- consolidated "Bare elements", "class" and "TextBlock" styles
- added ExpandTransition

## Tailwind

- migrated sanbox app from css to tailwind
- added tailwind migration script `apply` to [Tailwind/package.json](./Tailwind/package.json)
- renamed to `input-base.css` and `input-sandbox.css`
- moved output.css to css folder
- added cuncurently watch script for both output targets
- added stylelint/pretier based `format` script
- consolidated intend across css files, added to editorconfig css section
- added css reset
- ignore output.css

## Tools

- added screenshoting diff, visual, regression check tool
- added `componets-usage` tool generating [./USAGE.md](./USAGE.md) report

## Repo

- added .idea to gitignore
