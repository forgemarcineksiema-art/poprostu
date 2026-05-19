# Character/Avatar Pass 0.5

Date: 2026-05-19

Decision: KeepVisualRequestHumanoidSource

## Current Status

Current status: Pablo V2 is a Generic GLB visual candidate. The prefab has a skinned mesh, a skeleton, an Animator component, and the placeholder controller created in the previous pass, but the Animator has no Avatar assigned. That means the current asset is not Humanoid-retarget ready and should not receive production locomotion clips yet.

## Why This Decision

The existing Pablo V2 asset is useful as a visual candidate because it is skinned, scaled for gameplay, mounted through `PrototypeAvatarDefinition`, and isolated from gameplay colliders/rigidbodies. It is not a final animation foundation because Unity currently sees it as Generic rather than a valid Humanoid Avatar.

The correct next step is not to fake motion on the Generic rig. The next asset step should request a Humanoid-native source or an external conversion path that gives Unity a valid Humanoid Avatar first.

## Unity AI Assistant Boundary

Unity AI Assistant can be used for the next asset pass, but only as an asset worker. It should create or export a Humanoid-native Pablo Valera source candidate and report whether the result is Humanoid-compatible.

Hard scope: Do not edit gameplay scripts, scenes, packages, project settings, input, camera, motor, vehicle, mission, route, HUD, or Unity AI packages.

## Recommended Unity AI Prompt

Create or export a Humanoid-native Pablo Valera character source asset for this Unity 6.4 URP project.

Pablo Valera is a fictional character. Do not make him resemble any real person, celebrity, politician, or historical criminal. Do not use real logos, real brands, cartel symbols, weapons, tattoos with text, or copyrighted designs.

Use the current Pablo V2 only as visual direction. The current status is Generic GLB and it has no valid Unity Humanoid Avatar. The goal is a clean Humanoid-native source, preferably FBX or a Unity-importable asset that exposes standard Rig settings and can be configured as Humanoid.

Create assets only under `Assets/Models/Characters/PabloValera_HumanoidCandidate/`.

Deliver:
- `PabloValera_HumanoidCandidate.prefab`
- source mesh/model files, materials, and textures inside that folder
- if possible, a valid Unity Humanoid Avatar
- if possible, safe idle/walk/run/sprint preview clips
- a short report stating whether the rig is Humanoid-compatible, whether the Avatar is valid, whether the clips are real or placeholder, approximate height, known issues, and which meshes/materials could become customization slots

Do not edit gameplay scripts, scenes, packages, project settings, input, camera, motor, vehicle, mission, route, HUD, or Unity AI packages.

## Acceptance For Next Pass

The next pass can integrate real locomotion only if Unity reports a valid Humanoid Avatar or an equivalent verified animation-ready rig. If the new candidate is still Generic-only, it remains a visual candidate and does not replace the animation foundation.
