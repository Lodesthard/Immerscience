# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

VR museum/interactive-space experience. Unity + XR Interaction Toolkit 2.5.4. Targets Oculus/OpenXR. One scene: `Assets/scene_jeu.unity`.

## Key Packages

- `com.unity.xr.interaction.toolkit` 2.5.4 — grab, hover, interactable events
- `com.unity.xr.oculus` 4.0.0 + `com.unity.xr.openxr` 1.8.2
- `com.unity.xr.arfoundation` 5.0.7
- `com.ivanmurzak.unity.mcp` — AI Game Developer MCP server (scene manipulation via tools)

Unity Editor **2022.3.9f1** (`ProjectSettings/ProjectVersion.txt`). Render pipeline = **Built-in** (pas URP) → matériaux via shader `Standard`.

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

Sliders VR : clic piloté par l'action **"UI Press"** de `XRI Default Input Actions` (sur `ActionBasedController` du Controller), rebindée sur le **grip** (`{GripButton}`/`{Grip}`). Requiert `TrackedDeviceGraphicRaycaster` sur le canvas `Menu` + `XRUIInputModule` sur l'EventSystem (déjà en place). `SliderApplyOnRelease` (sur un slider) applique l'effet seulement au relâchement (IPointerUp/IEndDrag), pas pendant le drag.

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
- `unity_execute_code` : code wrappé en corps de méthode → **pas de `using`** ; qualifier complet (`UnityEngine.Object`, `UnityEngine.Random`, `UnityEditor.AssetDatabase`). `Object`/`Random` seuls = ambigus (CS0104).
- Plusieurs Editors peuvent tourner : ce projet = **port 7891** (`Unity/2/Immerscience`). `unity_select_instance` puis passer `port:7891` à chaque appel `unity_*`.
- Portes GLTF de l'ascenseur salle 2 : hiérarchie fortement scalée/tournée (`localPosition` 1 unité ≈ ~200 unités monde, axes pivotés). Ne pas régler `localPosition` à l'aveugle — placer les portes en pose **fermée** dans l'éditeur avant Play (posInitiale = fermé).

## Merge conflicts sur scene_jeu.unity

Résoudre avec UnityYAMLMerge (jamais à la main) :
`& "C:\Program Files\Unity\Hub\Editor\2022.3.9f1\Editor\Data\Tools\UnityYAMLMerge.exe" merge -p BASE THEIRS OURS MERGED`
- Extraire les 3 stages : `git cat-file blob <hash>` (depuis `git ls-files -u -- Assets/scene_jeu.unity` : stage 1=base, 2=ours, 3=theirs).
- Ordre des args = `base theirs ours merged`. Vérifier 0 `<<<<<<<` dans merged, puis `cp` → `git add`.
- En rebase, répéter à chaque étape (`git rebase --continue` avec `GIT_EDITOR=true`).
- Après un git op qui modifie la scène sur disque → **recharger la scène dans Unity** (sinon l'éditeur écrase avec sa version mémoire).

## graphify

This project has a knowledge graph at graphify-out/ with god nodes, community structure, and cross-file relationships.

Rules:
- For codebase questions, first run `graphify query "<question>"` when graphify-out/graph.json exists. Use `graphify path "<A>" "<B>"` for relationships and `graphify explain "<concept>"` for focused concepts. These return a scoped subgraph, usually much smaller than GRAPH_REPORT.md or raw grep output.
- If graphify-out/wiki/index.md exists, use it for broad navigation instead of raw source browsing.
- Read graphify-out/GRAPH_REPORT.md only for broad architecture review or when query/path/explain do not surface enough context.
- After modifying code, run `graphify update .` to keep the graph current (AST-only, no API cost).
