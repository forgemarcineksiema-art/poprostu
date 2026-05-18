# Vertical Slice Feel Gate 0.4 - 2026-05-18 18:12:41

## Purpose

Manual retest required after readability pass 0.3.

This gate answers one question: after the HUD, scene dressing, and readable prop grouping passes, does the current route actually play cleanly enough to continue past the readability gate?

It does not add combat, police AI, new missions, new districts, animation systems, or extra pressure states.

## Evidence Inputs

- Metrics path: C:\Users\Marcin\AppData\LocalLow\DefaultCompany\My project\phase1_latest_run.txt
- Coverage gate: complete
- Average FPS: 1558.1
- Worst frame: 259.8ms
- Performance status: Frame spikes
- Manual decision source: C:\Users\Marcin\Documents\hahahaha\My project\docs\prototype_reports\phase1_manual_playtest_2026-05-18_09-01-46.md
- Manual decision status before 0.4: accepted

~~~text
Phase 1 Feel Prototype Run
ElapsedSeconds: 0.7
VehicleEntries: 1
VehicleExits: 1
Interactions: 1
PressureEntries: 1
CompletedCheckpoints: 5
RouteCompleted: True
RouteOutcome: Complete
MaxSpeed: 1.9
LastInteraction: Inspect workshop shutter
LastCheckpoint: Safe return
AverageFps: 1558.1
WorstFrameMs: 259.8
PerformanceStatus: Frame spikes
CoverageComplete: True
CoverageStatus: Coverage complete
ManualFeelGate: Required
~~~

## Required Route Loop

- [ ] Start on foot and walk with camera-relative movement.
- [ ] Rotate the camera through at least one 90 degree turn, then keep moving forward.
- [ ] Enter the vehicle without losing orientation.
- [ ] Drive through the narrow route and brake before reversing.
- [ ] Trigger the pressure beat.
- [ ] Make the playable pressure choice.
- [ ] Pass the route gate consequence.
- [ ] Reach El Respiro.
- [ ] Exit the vehicle at the workshop/front area.
- [ ] Interact with the readable objective.
- [ ] Return through Safe return.

## Feel Risk Matrix

| Area | Question | Pass? | Notes |
| --- | --- | --- | --- |
| Camera/Input Feel | Camera yaw, recenter, tight-space recovery, mouse look, and gamepad look keep the player oriented. |  |  |
| On-Foot Movement | W follows camera forward, A/D do not create a spiral, acceleration/deceleration feel legible. |  |  |
| Enter/Exit Orientation | Entering and exiting the car keeps yaw and target context stable without a jump. |  |  |
| Vehicle Brake/Reverse | S brakes before reverse, steering stays predictable, handbrake does not feel like random physics. |  |  |
| Route Readability | Barrio, Rios, roadblock, El Respiro, pressure, and safe return are spatially distinct while playing. |  |  |
| HUD/Prompt Clarity | Objective, pressure state, and interaction prompt make the next action obvious without debug-reading. |  |  |

## Decision

- [ ] 0.4 accepted: continue past the readability gate.
- [ ] Needs camera/input feel fix pack.
- [ ] Needs on-foot movement fix pack.
- [ ] Needs vehicle feel fix pack.
- [ ] Needs route/readability fix pack.
- [ ] Needs HUD/interaction prompt fix pack.

## Stop Rules

- Coverage is complete, so 0.4 can focus on feel, readability, and disorientation risk.
- Do not add new mission content if any Feel Risk Matrix row is blocked.
- Do not add full police AI, combat, or new districts as a response to a feel blocker.
- If 0.4 is blocked, the next milestone must be a targeted 0.5 fix pack, not Phase 3 content.

## Top Fixes Before Continuing

1.
2.
3.

## Notes

-
