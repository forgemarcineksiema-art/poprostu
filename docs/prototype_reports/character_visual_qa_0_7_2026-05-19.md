# Character Visual QA 0.7

Date: 2026-05-19

Decision: keep `PabloValera_HumanoidCandidate` as the active runtime visual, but quarantine AI-generated upper-body animation until it passes visual review.

## Screenshot Finding

Manual screenshots after runtime integration showed a technically valid Humanoid setup but unacceptable pose quality:

- shoulders and arms collapse into a tense, crossed/raised pose,
- side view reads like broken upper-body retargeting rather than intentional locomotion,
- the model itself is usable enough for scale/camera checks, but the source locomotion clips are not approved as full-body gameplay animation.

## Runtime Fix

The runtime Animator still exposes the bridge parameters:

- `Speed`
- `IsSprinting`
- `Grounded`

The controller now uses a lower-body AvatarMask so the AI source clips can drive legs/root motion intent while head, arms, and fingers stay out of the generated upper-body motion. This keeps Pablo playable for camera/movement testing without accepting broken full-body animation as final.

## Boundary

This is not final locomotion polish. It is a safety pass so the character can stay in-game while we decide whether to source better Humanoid animations, ask Unity AI for corrected clips, or use a known external animation pack.
