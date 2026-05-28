# Graph Report - Assets/Scripts+joel  (2026-05-28)

## Corpus Check
- Corpus is ~2,368 words - fits in a single context window. You may not need a graph.

## Summary
- 209 nodes · 230 edges · 23 communities (19 shown, 4 thin omitted)
- Extraction: 94% EXTRACTED · 6% INFERRED · 0% AMBIGUOUS · INFERRED: 14 edges (avg confidence: 0.72)
- Token cost: 50,000 input · 6,902 output

## Community Hubs (Navigation)
- [[_COMMUNITY_Audio System Core|Audio System Core]]
- [[_COMMUNITY_Two-Hand Grab Pose|Two-Hand Grab Pose]]
- [[_COMMUNITY_Guitar Melody Playback|Guitar Melody Playback]]
- [[_COMMUNITY_Interactive Environment Effects|Interactive Environment Effects]]
- [[_COMMUNITY_Elevator Wall-Fit (Editor)|Elevator Wall-Fit (Editor)]]
- [[_COMMUNITY_Elevator Travel Sequence|Elevator Travel Sequence]]
- [[_COMMUNITY_Hover Feedback|Hover Feedback]]
- [[_COMMUNITY_Magnet Return|Magnet Return]]
- [[_COMMUNITY_Gaze Teleport Detection|Gaze Teleport Detection]]
- [[_COMMUNITY_VR Menu Opener|VR Menu Opener]]
- [[_COMMUNITY_Painting Stare Effect|Painting Stare Effect]]
- [[_COMMUNITY_Footstep Audio|Footstep Audio]]
- [[_COMMUNITY_Laser Pointer Toggle|Laser Pointer Toggle]]
- [[_COMMUNITY_VR Settings Sliders|VR Settings Sliders]]
- [[_COMMUNITY_Audio Triggers & Singletons|Audio Triggers & Singletons]]
- [[_COMMUNITY_Automatic Doors|Automatic Doors]]
- [[_COMMUNITY_Debug Trigger Zone|Debug Trigger Zone]]
- [[_COMMUNITY_Room Wall Flip|Room Wall Flip]]
- [[_COMMUNITY_Second-Hand Grab Coordination|Second-Hand Grab Coordination]]
- [[_COMMUNITY_VR Settings UI Group|VR Settings UI Group]]
- [[_COMMUNITY_Grab Utilities|Grab Utilities]]
- [[_COMMUNITY_Menu Opener|Menu Opener]]
- [[_COMMUNITY_Wall Mesh Flip|Wall Mesh Flip]]

## God Nodes (most connected - your core abstractions)
1. `TwoHandPlayPose` - 18 edges
2. `GuitarMelody` - 15 edges
3. `AscenseurAutomatique` - 11 edges
4. `AudioManager` - 10 edges
5. `HoverFeedback` - 10 edges
6. `MagnetReturn` - 10 edges
7. `PaintingStareEffect` - 9 edges
8. `ElevatorWallFit` - 8 edges
9. `FootstepManager` - 8 edges
10. `MenuOpener` - 8 edges

## Surprising Connections (you probably didn't know these)
- `CreepyZoneManager (singleton)` --conceptually_related_to--> `PaintingStareEffect`  [INFERRED]
  Assets/Scripts/CreepyZoneManager.cs → Assets/Scripts/PaintingStareEffect.cs
- `PorteAutomatique (proximity door)` --semantically_similar_to--> `FootstepManager`  [INFERRED] [semantically similar]
  Assets/Scripts/PorteAutomatique.cs → Assets/Scripts/FootstepManager.cs
- `AscenseurAutomatique (elevator)` --semantically_similar_to--> `PorteAutomatique (proximity door)`  [INFERRED] [semantically similar]
  Assets/joel/AscenseurAutomatique.cs → Assets/Scripts/PorteAutomatique.cs
- `StareDetector.TriggerSuddenTeleport` --semantically_similar_to--> `AscenseurAutomatique.SequenceVoyage (close-wait-teleport-open)`  [INFERRED] [semantically similar]
  Assets/Scripts/StareDetector.cs → Assets/joel/AscenseurAutomatique.cs
- `CreepyZoneManager (singleton)` --conceptually_related_to--> `AudioManager.StartCreepyDistortion`  [INFERRED]
  Assets/Scripts/CreepyZoneManager.cs → Assets/Scripts/AudioManager.cs

## Hyperedges (group relationships)
- **VR grab-interactable scripts (XRGrabInteractable select events)** — guitarmelody_GuitarMelody, twohandplaypose_TwoHandPlayPose, magnetreturn_MagnetReturn, hoverfeedback_HoverFeedback [INFERRED 0.85]
- **Player proximity / trigger detection scripts** — porteautomatique_PorteAutomatique, ascenseurautomatique_AscenseurAutomatique, creepyzonemanager_CreepyZoneManager, etage2musictrigger_Etage2MusicTrigger, debugtrigger_DebugTrigger [INFERRED 0.85]
- **Elevator close-wait-teleport-open flow** — ascenseurautomatique_AscenseurAutomatique, ascenseurautomatique_SequenceVoyage, ascenseurautomatique_symmetric_door_rationale [EXTRACTED 1.00]

## Communities (23 total, 4 thin omitted)

### Community 0 - "Audio System Core"
Cohesion: 0.09
Nodes (10): AudioMixerSnapshot, MonoBehaviour, AudioManager, AudioSource, CreepyZoneManager, bool, Collider, Etage2MusicTrigger (+2 more)

### Community 1 - "Two-Hand Grab Pose"
Cohesion: 0.10
Nodes (13): AudioClip, IXRSelectInteractor, Rigidbody, AudioSource, bool, float, Quaternion, SelectEnterEventArgs (+5 more)

### Community 2 - "Guitar Melody Playback"
Cohesion: 0.18
Nodes (7): AudioSource, bool, SelectEnterEventArgs, SelectExitEventArgs, XRGrabInteractable, GuitarMelody, XRBaseInteractor

### Community 3 - "Interactive Environment Effects"
Cohesion: 0.19
Nodes (13): AscenseurAutomatique (elevator), AscenseurAutomatique.SequenceVoyage (close-wait-teleport-open), Symmetric door easing via SmoothStep+MoveTowards, ElevatorWallFit, ElevatorWallFit.FitToWall (raycast push), Editor-time wall fitting via renderer bounds raycast, FootstepManager, PaintingStareEffect.OnLookAway (+5 more)

### Community 4 - "Elevator Wall-Fit (Editor)"
Cohesion: 0.18
Nodes (9): Editor, FitAxis, LayerMask, ContextMenu, float, Transform, Vector3, ElevatorWallFit (+1 more)

### Community 5 - "Elevator Travel Sequence"
Cohesion: 0.20
Nodes (7): AscenseurAutomatique, AudioSource, bool, float, IEnumerator, Transform, Vector3

### Community 6 - "Hover Feedback"
Cohesion: 0.22
Nodes (6): bool, float, GameObject, Vector3, HoverFeedback, XRBaseInteractable

### Community 7 - "Magnet Return"
Cohesion: 0.22
Nodes (6): bool, float, Quaternion, Vector3, XRGrabInteractable, MagnetReturn

### Community 8 - "Gaze Teleport Detection"
Cohesion: 0.27
Nodes (5): PaintingStareEffect, IEnumerator, Transform, Vector3, StareDetector

### Community 9 - "VR Menu Opener"
Cohesion: 0.22
Nodes (5): CallbackContext, GameObject, InputActionReference, Transform, MenuOpener

### Community 10 - "Painting Stare Effect"
Cohesion: 0.22
Nodes (5): Material, Renderer, bool, float, PaintingStareEffect

### Community 11 - "Footstep Audio"
Cohesion: 0.25
Nodes (5): AudioSource, float, Transform, Vector3, FootstepManager

### Community 12 - "Laser Pointer Toggle"
Cohesion: 0.25
Nodes (4): CallbackContext, InputActionReference, LaserToggle, XRRayInteractor

### Community 13 - "VR Settings Sliders"
Cohesion: 0.25
Nodes (4): ContinuousMoveProviderBase, Slider, Transform, VRMenuSettings

### Community 14 - "Audio Triggers & Singletons"
Cohesion: 0.33
Nodes (7): AudioManager (singleton), AudioManager.ReturnNormalMusic, AudioManager.StartCreepyDistortion, AudioManager.StartMusic, CreepyZoneManager (singleton), DebugTrigger, Etage2MusicTrigger

### Community 15 - "Automatic Doors"
Cohesion: 0.29
Nodes (4): float, Transform, Vector3, PorteAutomatique

### Community 17 - "Room Wall Flip"
Cohesion: 0.40
Nodes (3): bool, ContextMenu, SalleMurFlipExterieur

### Community 18 - "Second-Hand Grab Coordination"
Cohesion: 0.40
Nodes (5): GuitarMelody.ClearSecondHand, GuitarMelody, GuitarMelody.SetSecondHand, TwoHandPlayPose, Left/right hand detection by interactor GameObject name substring

### Community 19 - "VR Settings UI Group"
Cohesion: 0.67
Nodes (3): LaserToggle, SliderStep, VRMenuSettings

## Knowledge Gaps
- **82 isolated node(s):** `AudioSource`, `AudioMixerSnapshot`, `bool`, `FitAxis`, `LayerMask` (+77 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **4 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `TwoHandPlayPose` connect `Two-Hand Grab Pose` to `Audio System Core`?**
  _High betweenness centrality (0.153) - this node is a cross-community bridge._
- **Why does `GuitarMelody` connect `Guitar Melody Playback` to `Audio System Core`?**
  _High betweenness centrality (0.124) - this node is a cross-community bridge._
- **Why does `ElevatorWallFit` connect `Elevator Wall-Fit (Editor)` to `Audio System Core`?**
  _High betweenness centrality (0.093) - this node is a cross-community bridge._
- **What connects `AudioSource`, `AudioMixerSnapshot`, `bool` to the rest of the system?**
  _85 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Audio System Core` be split into smaller, more focused modules?**
  _Cohesion score 0.09486166007905138 - nodes in this community are weakly interconnected._
- **Should `Two-Hand Grab Pose` be split into smaller, more focused modules?**
  _Cohesion score 0.10476190476190476 - nodes in this community are weakly interconnected._