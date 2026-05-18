# Vertical Slice Readability Pass 0.3 - 2026-05-18

## Decision

Keep the current Phase 1/2R slice narrow and make the authored scene easier to read before adding more systems.

This pass does not add missions, police logic, combat, AI, or new route beats. It turns the existing greybox dressing into named, validated readable prop groups so the scene has stable authoring anchors for later prefab/material work.

## Implemented

- Added `PrototypeReadableProp` metadata for authored readability clusters.
- Grouped the main landmark dressing into five scene anchors:
  - `Barrio street identity prop`;
  - `Safe return readable prop`;
  - `Rios checkpoint readable prop`;
  - `Police roadblock readable prop`;
  - `El Respiro readable prop`.
- Parent-linked existing and new non-blocking dressing under those anchors.
- Added small readable detail props for Rios, roadblock, and El Respiro:
  - checkpoint stool and papers;
  - roadblock cones;
  - workshop shutter slats and door lamp.
- Kept the prop groups and all child dressing on `CameraIgnore`, with disabled or trigger-only colliders.
- Updated `PrototypeSceneValidator` and EditMode tests so readable prop grouping is part of the scene contract.
- Updated the capture tool output path for this pass:
  - `docs/prototype_reports/vertical_slice_readability_pass_0_3_2026-05-18.png`

## Validation

Red gate before implementation:

~~~text
EditMode results: total=36 passed=35 failed=1
Failure: PrototypeReadableProp type is missing.
~~~

Fast gate after scene rebuild:

~~~text
EditMode results: total=36 passed=36 failed=0
PlayMode results: total=29 passed=29 failed=0
~~~

Full gate is tracked in the commit closeout for this pass.

## Honest Read

This is still primitive geometry, but it now has authored structure instead of loose cube dressing. The next useful step is either a small material/prefab kit for these five anchors or a first real manual feel capture through the route with the current HUD and readable landmarks.
