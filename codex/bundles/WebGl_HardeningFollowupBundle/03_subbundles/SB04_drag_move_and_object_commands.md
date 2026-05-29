# SB04 - Drag/move and object commands

## Goal

Complete existing generic drag/move contracts.

## Background

The branch already has:
- `AllowDragOnGroundPlane`
- `WebGlObjectMovedEventArgs`
- `WebGlSceneView.ObjectsMoved`

But the JS interaction runtime currently only handles hover, click selection, and double-click focus.

## Tasks

1. Implement ground-plane drag for draggable scene objects.
2. Add drag threshold and pointer capture.
3. Prevent camera orbit while dragging an object.
4. Emit `OnObjectsMoved`.
5. Update scene model positions on commit.
6. Add JS public command:
   - `setObjectTransform(host, objectId, transform)`
   - `moveObject(host, objectId, position)`
7. Add Blazor wrapper methods.

## Acceptance criteria

- A draggable object can be moved in `/tycoon-village`.
- `ObjectsMoved` event fires with stable object id and final position.
- Proof snapshot includes updated object position.
- Non-draggable objects cannot be dragged.
