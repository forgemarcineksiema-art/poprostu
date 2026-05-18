# Phase 2 Action and Pressure Microtests - 2026-05-18

## Scope

Phase 2 is implemented as three short consequence microtests inside the current prototype scene. This is not a full combat, police, bribery, or companion system.

## Implemented Proofs

| Microtest | Event | World State Change | Visible World Reaction |
| --- | --- | --- | --- |
| Public violence | `PublicViolenceCommitted` | `Fear = High`, `PeopleLove = Low`, `StatePressure` rises, `RuleStyle = ShowOfForce` | civilian panic marker, shop shutter marker, police pressure marker react |
| Bribe | `BribeAccepted` | `StatePressure` lowers, `DirtyCash = Hidden`, `RuleStyle = Bribe` | roadblock opens marker, Rios leverage marker, risk cargo hidden marker react |
| Mateo relation | `MateoProtected` / `MateoHumiliated` | `LieutenantTrust = Trusted` or `Humiliated` | early warning marker or late warning marker reacts |

## Runtime Pieces

- `PrototypeWorldState` is the single runtime state source for the Phase 2 proof.
- `PrototypeInteractable` can emit a `PrototypeWorldEvent`.
- `PrototypeWorldReactionMarker` listens to world state changes and changes visible scene objects.
- The debug HUD now prints world state and the latest visible world reaction.

## Validation

Command:

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\verify_phase1.ps1 -RebuildScene -SkipBuild
~~~

Result:

- Scene validator passed.
- EditMode tests: total 5, passed 5, failed 0.
- PlayMode tests: total 8, passed 8, failed 0.

## Stop Rule

Phase 2 should not expand into full combat yet. The next useful proof is Phase 3: save/load and scene reconstruction from `PrototypeWorldState`, because Phase 2 now has event-driven consequences worth preserving.
