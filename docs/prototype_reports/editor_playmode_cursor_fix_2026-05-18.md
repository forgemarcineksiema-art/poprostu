# Editor Play Mode Cursor Fix - 2026-05-18

## Problem

Manual 0.4 retest in Unity Editor exposed two feel blockers:

- mouse camera movement was limited by the cursor reaching the edge of the screen;
- Play Mode felt laggy enough that standalone build performance must remain the real feel reference.

## Root Cause

`PrototypeCameraRig` reads raw mouse delta from `PrototypeInput.LookMouseDelta`, but the runtime did not lock the cursor during Play Mode. In Editor/Game view, this means the camera can only rotate while the OS cursor still has screen space.

The lag report is separate: Editor Play Mode includes Unity Editor overhead, asset/database work, Scene/Game view UI, and editor focus handling. It is useful for quick iteration, but it is not a clean performance gate.

## Implemented

- Added `PrototypeCursorController`.
- Added scene object `Prototype Cursor Controller`.
- On Play Mode / runtime focus, the cursor locks and hides.
- `Esc` unlocks the cursor.
- Clicking the Game view relocks after an escape unlock.
- Added EditMode tests for the lock/unlock/relock decision and scene presence.
- Updated the scene builder and validator so future rebuilds keep the cursor controller.

## Validation

Fast gate after scene rebuild:

~~~text
EditMode results: total=38 passed=38 failed=0
PlayMode results: total=29 passed=29 failed=0
~~~

## Manual Retest Guidance

For quick checks through Unity Hub, use Game view and click into the view when Play starts. Press `Esc` to release the cursor.

For judging lag and final feel, use a standalone dev build through the 0.4 runner. Editor Play Mode lag should be reported, but it should not replace standalone feel evidence.
