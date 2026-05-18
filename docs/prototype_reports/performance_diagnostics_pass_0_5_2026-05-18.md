# Performance Diagnostics Pass 0.5 - 2026-05-18

## Decision

Continue game work by making the reported lag measurable before changing rendering, physics, or scene content.

Editor Play Mode can feel worse than standalone because it carries Unity Editor overhead. The game now records enough frame data to compare Editor and build runs instead of relying only on memory.

## Implemented

- Added `PrototypePerformanceSampler`.
- Added `PrototypePerformanceProbe` scene component.
- `PrototypeRunMetrics` now records:
  - `AverageFps`;
  - `WorstFrameMs`;
  - `PerformanceStatus`.
- Player HUD status line now includes the current performance line during prototype playtests.
- `vertical_slice_feel_gate_0_4` reports now include parsed performance metrics when the current run file contains them.
- Scene builder and validator require the performance probe.

## Status Labels

- `OK`: average FPS is at least 50 and worst frame is below 45 ms.
- `Low FPS`: average FPS is below 50 without a large single-frame spike.
- `Frame spikes`: worst frame is 45 ms or higher.
- `No samples`: the current metrics file was produced before the probe sampled frames.

## Validation Target

Use the 0.4 runner after this pass:

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\run_vertical_slice_feel_gate.ps1
~~~

The generated report should include `Average FPS`, `Worst frame`, and `Performance status`.

## Honest Read

This does not optimize the game yet. It makes the lag report actionable. If standalone also reports `Low FPS` or `Frame spikes`, the next pass should be a focused performance fix. If only Editor Play Mode feels bad, the manual feel decision should use standalone as the source of truth.
