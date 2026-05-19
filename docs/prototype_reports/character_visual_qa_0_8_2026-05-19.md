# Character Visual QA 0.8

Date: 2026-05-19

Decision: Pablo's runtime Animator must use sanitized game-owned locomotion clips, not the raw full-body Unity AI clips.

## Finding

Manual screenshots after QA 0.7 still showed Pablo with a broken upper-body pose. A diagnostic pose dump showed the source prefab rest pose is acceptable enough for runtime testing: hands sit low near the sides. After the runtime controller evaluated the Unity AI `Idle` clip, the hands moved inward and upward, creating the visible crossed/raised-arm silhouette.

That means the problem is in the source animation curves, not in the mesh being static or in the camera.

## Runtime Fix

The builder now creates these runtime-owned clips:

- `PabloValera_Runtime_Idle.anim`
- `PabloValera_Runtime_Walk.anim`
- `PabloValera_Runtime_Run.anim`
- `PabloValera_Runtime_Sprint.anim`

They are copied from the Unity AI locomotion clips, then stripped of upper-body curves for spine, chest, neck, head, jaw, shoulders, arms, forearms, and hands. Lower-body/root locomotion curves remain so Pablo can stay in the scene for camera, movement, and scale testing.

## Boundary

This is still not final character animation. It is a runtime safety pass that prevents known-bad upper-body animation from shipping into playtests while keeping the Humanoid candidate useful.
