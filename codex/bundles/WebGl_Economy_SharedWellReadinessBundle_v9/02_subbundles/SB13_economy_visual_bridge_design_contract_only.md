# SB13 - Cross-repo future bridge design only

## Scope
Design only. Do not add a reference between Economy and Components.

## Tasks
- Add a design document describing future bridge:
  `EconomyVisualAction` -> `WebGlRunAction`.
- Define mapping table:
  - move-to-target -> move-to-object
  - return-to-home -> return-to-anchor(home)
  - change-pose -> set-pose
  - show-symbol -> show-symbol
  - transfer-resource -> resource-transfer-visual
  - pulse-relationship -> pulse-link
- Define required object id conventions for bridge consumers.
