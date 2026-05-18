# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

VR museum/interactive-space experience. Unity + XR Interaction Toolkit 2.5.4. Targets Oculus/OpenXR. One scene: `Assets/scene_jeu.unity`.

## Key Packages

- `com.unity.xr.interaction.toolkit` 2.5.4 — grab, hover, interactable events
- `com.unity.xr.oculus` 4.0.0 + `com.unity.xr.openxr` 1.8.2
- `com.unity.xr.arfoundation` 5.0.7
- `com.ivanmurzak.unity.mcp` — AI Game Developer MCP server (scene manipulation via tools)

## Scene Structure

Single scene `scene_jeu`. Key roots:

| Root | Role |
|---|---|
| `XR Interaction Setup/XR Origin (XR Rig)` | Player rig. Camera height via `Camera Offset` child localPosition.y |
| `objets attrapables/` | Grabbable props (Guitar, jeu) |
| `Old Pool Table/` | Pool cues (`Pool Cue @Mark Peters on Sketchfab`, `Pool Cue2 @Mark Peters on Sketchfab`) and balls |
| `scene (3)/root/GLTF_SceneRootNode/` | Imported GLTF objects (Steam Controller_17…) |
| `ascenseur` / `Gestionnaire_Ascenseur` | Elevator system |
| `Zone_Detection_Porte` | Door trigger zones |

## Script Locations

- `Assets/Scripts/` — main custom scripts
- `Assets/joel/` — `AscenseurAutomatique.cs` (elevator logic, lives here, not in Scripts/)

**No duplicate class names across folders.** Past CS0101 bug came from two files defining `AscenseurAutomatique`. Keep one definition per class.

## Architecture

### VR Interaction Pattern

All interactable objects use `XRGrabInteractable`. Scripts subscribe to `grab.selectEntered` / `grab.selectExited` in `Awake`, unsubscribe in `OnDestroy`. Never subscribe in `Start` if the listener accesses `grab.interactorsSelecting`.

### Player Reference

Scripts that need the player head use `Camera.main.transform` as fallback, with an assignable `public Transform playerHead/joueur/playerCamera` field. Assign Main Camera in inspector.

### Two-Hand Grab

`TwoHandPlayPose` — reusable component. On second-hand grab: aligns object so `leftGripPoint`/`rightGripPoint` child Transforms match each hand's position. Uses `LateUpdate` to override XRGrabInteractable. Requires `XRGrabInteractable` on same GameObject. Left/right hand detected by "left"/"right" in interactor GameObject name.

`GuitarMelody` — complementary to `TwoHandPlayPose` on Guitar. Plays continuous looping melody while both hands hold. Second hand must call `SetSecondHand()` / `ClearSecondHand()` from a secondary grab point's events.

### Doors

`PorteAutomatique` — proximity open/close, lerps two door halves by distance to player.  
`AscenseurAutomatique` — elevator: proximity detection → coroutine (close doors → wait 20s music → teleport XR Origin to `pointArrivee` → open doors). Guard `!voyageEnCours` on coroutine start; `voyageEnCours` reset to `false` after teleport so doors open on arrival.

**AscenseurAutomatique tuning notes:**
- `vitesse` default 1.5f is too fast (looks like overshoot). Use 0.4f (elevator 1) or 0.7f (elevator 2).
- `posInitialeGauche`/`posInitialeDroite` captured in `Start()` from `localPosition`. If doors are at GLTF open pose in editor, `posInitiale` = open → doors never close. Physically move door transforms to closed position in editor before Play.
- Translation axis depends on door parent rotation. Check `parent.right` / `parent.forward` in world space. Elevator 1 parent has `right=(0,1,0)` so local Y = world X (horizontal). Use `translationGauche=(0,-1.5,0)` / `translationDroite=(0,1.5,0)`.
- Two elevators: `Gestionnaire_Ascenseur` (~16.8, 5.4, -99.9) and `Gestionnaire_Ascenseur (1)` (~-43.6, 9.7, -136.8) with GLTF doors `LeftInteriorDoor_8_26` / `RightInteriorDoor_10_31`.

### Grab Utilities

`HoverFeedback` — scale pulse + aura object on hover.  
`MagnetReturn` — snaps object back to spawn if released within `snapDistance`.

### Audio

`FootstepManager` — plays footstep audio when camera moves more than `sensitivity` per frame.

### UI / Menu

`MenuOpener` — Input action toggles canvas, positions it in front of head.  
`VRMenuSettings` / `SliderStep` / `LaserToggle` — VR settings panel helpers.

## Inspector Wiring Rules

Every script that references the player expects **Main Camera** dragged into its transform field. XR Origin rig path: `XR Interaction Setup > XR Origin (XR Rig) > Camera Offset > Main Camera`.

`TwoHandPlayPose.playerHead` auto-resolves via `Camera.main` at runtime if left empty, but explicit assignment is preferred.

## Known Gotchas

- `UnityEngine.Object` null check: never use `?.` null-conditional — throws "variable not assigned". Use explicit `== null` check instead.
- `AscenseurAutomatique.ActualiserMouvementPortes()` accesses `porteGauche`/`porteDroite` without null checks — assign both door transforms.
- `MagnetReturn.TryReturn()` logic is inverted: returns only when distance **<** `snapDistance` (i.e., near origin), not when far.
- Player height controlled by `Camera Offset` `localPosition.y`, not by `XROrigin.m_CameraYOffset` (that field is disabled in Floor tracking mode).
- `gameobject-find` fails on partial names — GLTF objects have full attribution suffixes (e.g. `Pool Cue @Mark Peters on Sketchfab`). Use `script-execute` with `Resources.FindObjectsOfTypeAll<GameObject>()` to scan by name substring.
- MCP `gameobject-component-modify` cannot assign `Transform` refs via `jsonPatch` or `pathPatches` — use `script-execute` + `EditorUtility.SetDirty(go)` instead.
- `com.ivanmurzak.unity.mcp.probuilder` CS0246 errors are a package version mismatch — bénin, n'affecte pas les scripts du projet.
- VR hand mesh offset calibration: add debug cube (`CreatePrimitive(PrimitiveType.Cube)`, scale 0.03) as child of `Left Controller`/`Right Controller` at `localPosition = Vector3.zero` to see exact controller pivot in Play Mode. Remove after calibration.
