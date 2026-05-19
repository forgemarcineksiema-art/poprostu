# Character/Avatar Humanoid Candidate Audit

Date: 2026-05-19

Decision: accept as Humanoid source candidate, not active runtime Pablo yet.

## Result

The second Unity AI pass corrected the core technical blocker found in the first candidate. Unity API validation now resolves the prefab as:

- `SkinnedMeshRenderer`: 1
- `MeshRenderer`: 0
- `MeshFilter`: 0
- `Animator.avatar.isValid`: true
- `Animator.avatar.isHuman`: true
- `Rigidbody`: 0
- gameplay `Collider`: 0
- locomotion source clips: `Idle`, `Walk`, `Run`, `Sprint`

The source clips contain real animation curves. They are accepted as source clips, not yet tuned game locomotion.

## Current Boundary

`PabloValera_HumanoidCandidate` is now useful as a technical source asset. It does not automatically replace the current scene avatar because runtime integration still needs controller wiring, scale/framing validation, material/visual review, and play-mode movement checks.

The current Pablo V2 visual remains active until a dedicated integration pass swaps the visual and proves camera, movement, vehicle visibility, and animation parameters together.

## Known Limitations

- The candidate still appears to use one combined material/customization slot.
- The prefab has no runtime Animator Controller assigned, which is intentional for a source candidate.
- The generated clips need visual inspection before being treated as final movement feel.
- Unity AI also touched `ProjectSettings/URPProjectSettings.asset`; that change is outside the character asset scope and should not be committed as part of the candidate.
