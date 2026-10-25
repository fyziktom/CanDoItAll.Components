# Node selection and drag intent

Pointer-down selects a node and prepares a possible drag. It must not change collision
weights, resolved positions, snap targets or persisted positions. A crowded node must
remain under the pointer throughout a click or double-click, including a stationary hold.

A node drag activates after six CSS screen pixels of pointer displacement. Using screen
coordinates keeps the gesture consistent at every zoom. This movement threshold gives
clicks room for hand jitter without a timer that could activate a stationary press or
make deliberate dragging wait. Snapping runs only after activation.

Activation captures the currently displayed layout before setting the existing `moved`
flag. That flag now identifies an intentional drag for the collision solver as well as
release handling. The first move uses the captured positions, preserves other nodes and
initializes the drag render context once. Later moves use the existing frame-coalesced
patch path. Release retains collision resolution and one `NodesMoved` publication.
If the surface changes while pressed, activation rebases on the current displayed
positions and excludes targets that disappeared, became read-only or are no longer visible.

Ordinary, selection-group, frame and dependency drags share this admission path. Read-only
nodes remain excluded. A dependency click below the threshold still requests the existing
relationship action. Activating a drag clears click history so it cannot accidentally open
a node. The native double-click path remains available for the browser's slower double-click
timing; it must not duplicate the manual quick-double-activation path.

CanvasLib owns gestures, hit testing, rendering and overlap resolution. The application
continues to own node opening, concurrency admission and committed movement through the
existing typed callbacks. No project identity, domain rule, new server endpoint, per-move
interop or application timer belongs in this repair.

Validate crowded layout stability before activation, jitter at different zooms, preserved
neighbours during movement, editable group membership, one release publication, dependency
clicks and both canvas/legacy double-click paths. Native browser proof must reproduce the
reported node, hold and jitter without moving it, open it by double-click, and retain a
usable deliberate drag. Asset verification and the consumer build confirm the shipped
script revision; Node tests alone do not prove browser behavior.
