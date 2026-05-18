# Phase 2R Closeout - Director Audit - 2026-05-18

## Decision

Stop Phase 2R here.

The pressure loop now has enough proof for this stage: public violence raises pressure, bribe contains it, uncontained route entry triggers crackdown, scene playback changes, and route completion distinguishes contained success from pressure-failure escape.

Do not spend the next milestone on another pressure micro-polish pass. The next risk is not missing pressure logic. The next risk is that the project becomes a strongly tested greybox that still does not feel like a convincing playable slice.

## Current State

### Strong Enough To Build On

- Camera/input/movement/driving have a tested foundation lock.
- Runtime driving stays on the arcade Rigidbody baseline; WheelCollider did not win the first spike.
- Custom layers and camera collision masks are in place.
- `PrototypeWorldState` is the central source of truth.
- `PrototypeMissionSpine`, objective marker, route progress, pressure playback, and metrics now react to world events.
- Phase 1 manual feel gate is accepted.
- Phase 2R proves a small action-pressure branch with visible consequences.

### Not Strong Enough Yet

- The authored scene still reads as a prototype lab: cubes, markers, simple labels, and proof geometry dominate the experience.
- The current route is mechanically valid but not yet emotionally or spatially memorable.
- There is no real district identity beyond "narrow barrio route" and object names.
- Interaction targets are useful as test objects but not yet dressed as believable people/places.
- The HUD/debug state is honest for development, but it is not a player-facing presentation layer.

## Product Read

This is no longer blocked by the existence of systems. It is blocked by believability and playable slice identity.

The question for the next milestone should be:

> Can this 5-minute loop feel like the first playable proof of Valle de Plata instead of a systems checklist?

That means the next useful pass should improve scene readability, spatial identity, and authoring workflow without touching core gameplay truth.

## Unity AI Beta Position

Unity AI is useful here, but only as an authoring accelerator.

Use it for:

- scene dressing ideas and generated reference assets;
- placeholder signs, shutters, posters, wall treatments, grime, barrio props;
- material/texture exploration for the current greybox;
- quick iteration on mood boards or object variants;
- editor assistance for repetitive authoring work, if it stays reviewable.

Do not use it for:

- `WorldState` logic;
- mission truth;
- route consequences;
- camera/movement/vehicle physics;
- police/combat behavior;
- generated code that bypasses tests.

Current official Unity AI material says the beta provides editor-integrated assistance, generators, AI Gateway, and MCP-style integration, with project-aware assistant behavior and generated assets. It also requires package/cloud setup and credit/account management. That makes it a production accelerator, not a runtime dependency.

References:

- https://docs.unity.com/en-us/ai
- https://support.unity.com/hc/en-us/articles/48060149523476-Getting-started-with-Unity-AI-open-beta-user-guide
- https://unity.com/features/ai

## Next Milestone Recommendation

### Phase 2R Is Closed

No Phase 2R.5.

### Start: Vertical Slice Believability Pass 0.1

Goal:

Turn the current pressure/front route from a cube proof into a readable first slice of a place.

Scope:

- Keep all existing gameplay systems unchanged.
- Use the current scene builder and layers.
- Dress only the existing route: street, walls, workshop, Rios spot, roadblock, pressure patrol, safe return.
- Replace/augment the most debug-like markers with simple readable world objects.
- Use Unity AI beta only for authoring references/placeholders/assets, not core logic.
- Preserve test coverage and scene validator.

Acceptance criteria:

- A screenshot of the route communicates "hot barrio street with pressure" before reading the HUD.
- Rios, roadblock, workshop, pressure patrol, and safe return are spatially distinct.
- Both pressure branches remain green in PlayMode.
- No new combat, police AI, district expansion, mission tree, or animation system.

## What Not To Do Next

- Do not add full police AI.
- Do not add combat.
- Do not add a new district.
- Do not expand mission content before the current route has identity.
- Do not use Unity AI to generate unreviewed gameplay code.
- Do not continue adding pressure states unless a manual playtest proves a missing player-facing beat.

## Validation Target

Before accepting the next milestone:

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\verify_phase1.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\show_phase1_status.ps1
~~~

Then capture at least one current-scene screenshot from the Scene or Game view and compare it against the believability goal above.
