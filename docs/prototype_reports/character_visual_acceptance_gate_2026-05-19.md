# Character Visual Acceptance Gate - Pablo Valera

Date: 2026-05-19

Current verdict: Rejected for playable Pablo identity.

Runtime status: Technical pipeline only. The current `PabloValera_HumanoidCandidate` can remain useful for validating the Unity Humanoid import path, Animator bridge, runtime visual mounting, camera framing, and movement tests. It is not accepted as the protagonist visual and must not be treated as final Pablo.

## Why The Current Candidate Fails

The screenshots from the gameplay camera show an asset-level failure, not a single Animator bug:

- Silhouette: the back view reads as a wide block instead of a human third-person protagonist.
- Head and neck: the head is too small/low and appears sunk into the jacket and shoulders.
- Shoulders: shoulders are too high and too wide, creating a hunched turtle-shell shape.
- Arms: arms are not clearly readable; they appear hidden, glued to the torso, or pulled into the body.
- Legs and feet: legs and feet are not readable in third-person camera as a confident locomotion base.
- Pose: the idle stance looks permanently hunched rather than tired, controlled, and grounded.
- Camera read: from the actual TPP camera distance the model loses the body landmarks a player needs to read direction, speed, and intent.

## Acceptance Shots Required

Every future Pablo candidate must be reviewed with front / side / back / gameplay camera screenshots before it can be accepted as a playable visual.

Required views:

- Neutral front view at roughly character height.
- Neutral side view.
- Neutral back view.
- Gameplay camera behind the player on the route.
- Gameplay camera side orbit.
- Idle, walk, run, and sprint sampled in PlayMode if animations exist.

## Hard Visual Bar

A future candidate must pass all of these before it can become `GameplayCandidate`:

- Human silhouette reads immediately from TPP distance.
- head not sunk into the jacket, chest, or shoulders.
- Neck visible enough to avoid a collapsed torso/head shape.
- Shoulders natural, not raised to ear level.
- arms visible and naturally separated from torso in neutral pose.
- Hands readable and not melted, oversized, hidden, or glued into clothing.
- Torso has believable width and posture, without a permanent hunch unless intentionally animated.
- legs and feet readable in third-person camera.
- Feet contact the ground cleanly at runtime scale.
- Outfit supports grounded crime-drama tone without logos, text, weapons, police/military gear, or celebrity resemblance.
- Face is readable and serious from inspection distance.
- The asset still looks acceptable after the runtime Animator is assigned.

## Technical Bar

A future candidate still needs to keep the already-proven technical requirements:

- `SkinnedMeshRenderer`, not static `MeshRenderer`.
- Valid Unity Humanoid `Avatar`.
- Animator on the prefab root or a stable child that `PrototypeAvatarView` can find.
- No gameplay colliders or rigidbodies on the visual prefab.
- Runtime scale close to 1.75m-1.85m.
- Idle, Walk, Run, Sprint clips must not destroy the neutral silhouette.
- If source animation is poor, runtime clips may be quarantined, but the base rest pose must still look human.

## Pipeline Decision

Keep the current candidate as a technical Humanoid source only. The next model pass should create or import a new candidate rather than keep trying to solve the current model with code. Runtime systems should continue to distinguish:

- Technical readiness: can Unity import, skin, animate, and mount the candidate?
- Visual acceptance: does the candidate actually look like a believable playable Pablo in the real camera?

The current candidate passes enough technical checks to be useful for pipeline work, but fails visual acceptance.
