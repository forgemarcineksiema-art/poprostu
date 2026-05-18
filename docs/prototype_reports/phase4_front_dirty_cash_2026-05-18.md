# Phase 4 Front and Dirty Cash Prototype - 2026-05-18

## Scope

This milestone proves one small imperium loop in the existing prototype scene: dirty cash becomes physical risk, El Respiro changes front control, the barrio visibly reacts, and Mateo can reduce the pressure cost.

## Runtime Proof

| Beat | Event | World State Change | Visible Reaction |
| --- | --- | --- | --- |
| Pick up dirty cash | `DirtyCashPickedUp` | `DirtyCash = Carried`, `StatePressure` rises | dirty cash carried marker reacts |
| Secure El Respiro | `FrontTakenUnderWatch` | `FrontControl = PabloWatched`, `DirtyCash = Hidden`, `RuleStyle = Favor` | El Respiro watched marker and barrio reaction marker react |
| Mateo helps first | `MateoProtected` before takeover | `LieutenantTrust = Trusted`, takeover pressure ends lower | same front outcome, lower pressure cost |

## What This Is Not

- Not a final economy.
- Not a full front-management system.
- Not a full mission.
- Not a new district.

## Validation

Command:

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\verify_phase1.ps1
~~~

Result:

- Scene validator passed.
- EditMode tests: total 7, passed 7, failed 0.
- PlayMode tests: total 11, passed 11, failed 0.
- Developer build passed.

## Next

After commit and push, continue Phase 4 if more acceptance criteria are missing; otherwise start the smallest Phase 5 mission-slice assembly proof using the existing Phase 1-4 systems.
