# Phase 1 Stop Condition Handoff - 2026-05-18

## Current Decision

Stop before Phase 2.

The Phase 1 automated prototype is in place, verified, and pushed, but the production contract still requires a manual 10-minute feel playtest before action, pressure, World State, or Phase 2 work can start.

## Why This Is A Real Stop

The Phase 1 contract requires a human feel decision:

- 10 minutes of walking and driving is not tiring.
- Camera does not fight the player in the tight street.
- Player understands character, front, car, and route orientation.
- Entering and exiting the car is smooth and not disorienting.
- Car has tension on the narrow route without random physics chaos.
- Workshop interaction is readable without frustration.
- Route and patrol presence create tension without a full chase.
- Controller, camera, and driving fixes are clear after the test.

Automated tests can prove coverage, scene wiring, route checkpoints, build health, and decision parsing. They cannot prove feel.

## Latest Published Work

All listed commits are on `main` and pushed to `origin/main`.

| Commit | Milestone |
| --- | --- |
| `44495eb` | Add Phase 1 decision checker regression tests. |
| `04c10d9` | Show Phase 1 manual decision in the status dashboard. |
| `217b2a4` | Add manual decision checker for Phase 1 reports. |
| `2c4efd2` | Print manual feel checklist before the playtest. |
| `e2286f8` | Clarify content diff vs status-only dirty files. |
| `836a7de` | Add Phase 1 status checker. |
| `83312f6` | Add manual gate wrapper. |
| `bc72390` | Add manual playtest report generator. |
| `940853b` | Add one-command Phase 1 verification script. |
| `07b657a` | Add PlayMode scene coverage smoke. |
| `76ed872` | Teach playtest helper to check coverage. |
| `25295d4` | Add Phase 1 coverage gate. |
| `b79c081` | Add Phase 1 playtest helper. |
| `3dcc8f2` | Add runtime metrics. |
| `46a50f6` | Add developer build pipeline. |
| `31960b1` | Add route PlayMode smoke. |
| `b3b018a` | Add vehicle PlayMode smoke. |
| `db17ccd` | Add route checkpoints. |

## Fresh Verification

Commands run after the latest pushed work:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\show_phase1_status.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\test_phase1_manual_decision.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\check_phase1_manual_decision.ps1 -StatusOnly
git rev-list --left-right --count main...origin/main
```

Evidence:

- `show_phase1_status.ps1` exits with code `0`.
- Branch is `main`.
- `HEAD...origin/main` is `0 0`.
- Scene validator is `passed`.
- EditMode reports `total=4 passed=4 failed=0`.
- PlayMode reports `total=5 passed=5 failed=0`.
- Developer build reports `Phase 1 build result: Succeeded`.
- Latest metrics include `CoverageComplete: True`.
- Latest metrics include `CoverageStatus: Coverage complete`.
- Manual decision is `pending`.
- Decision checker reports no `docs\prototype_reports\phase1_manual_playtest_*.md` report yet.
- Decision checker regression test exits with code `0`.
- Decision checker regression test prints `Phase 1 manual decision checker tests passed.`

## Untouched Unrelated Files

These files remain dirty and were not staged as part of the Phase 1 gate/tooling commits:

```text
Assets/Scenes/Phase1_FeelPrototype.unity
Assets/Settings/DefaultVolumeProfile.asset
Assets/Settings/Mobile_RPAsset.asset
Assets/Settings/PC_RPAsset.asset
Assets/Settings/UniversalRenderPipelineGlobalSettings.asset
Assets/TutorialInfo/Icons/URP.png
ProjectSettings/GraphicsSettings.asset
ProjectSettings/ProjectSettings.asset
ProjectSettings/ShaderGraphSettings.asset
ProjectSettings/UnityConnectSettings.asset
```

The status checker currently reports `Assets/Scenes/Phase1_FeelPrototype.unity` as status-only/no `git diff` content. The other listed Unity settings/tutorial files have content diffs and should not be committed without a separate owner decision.

## Morning Commands

Run the manual gate:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\run_phase1_manual_gate.ps1
```

After filling the generated manual report, check the decision:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\check_phase1_manual_decision.ps1
```

Use the dashboard any time:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\show_phase1_status.ps1
```

## Next Allowed Moves

If the decision checker reports `accepted`:

- start Phase 2 with the smallest action/pressure microtest;
- do not start full `Pierwszy Front`;
- keep the first Phase 2 milestone narrow and verified.

If the decision checker reports `blocked`:

- do not start Phase 2;
- iterate the specific blocker: controller, camera, vehicle, or route/layout;
- rerun the manual gate after the fix.

If the decision checker reports `pending`, `conflicting`, or `unrecognized`:

- do not start Phase 2;
- fix the report decision first.

## Project Lead Note

No further autonomous coding is justified before the manual feel playtest. Additional scripts or docs would reduce clarity more than they would reduce risk. The next real evidence must come from playing the Phase 1 build and marking the manual decision.
