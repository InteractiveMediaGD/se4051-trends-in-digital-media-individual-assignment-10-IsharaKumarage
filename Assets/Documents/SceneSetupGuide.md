# Scene Setup Guide — Smart Relaxation Room
## SE4051 Individual Assignment — Unity 2022.3 LTS

> **Read this fully before opening Unity.** Follow every step in order.
> Estimated setup time: 30–45 minutes.

---

## Step 0 — Prerequisites

- Unity Hub installed with Unity **2022.3.x LTS** (matches `ProjectVersion.txt`)
- TextMeshPro package (auto-imported when you first open a TMP text field — Unity will prompt you)
- At least **3 audio clips** (WAV or MP3) for calm, energy, and focus moods
  - Suggestions: search **freesound.org** for "nature ambient", "upbeat lo-fi", "study focus"
  - Place them in `Assets/Audio/` once downloaded

---

## Step 1 — Open the Project

1. Open **Unity Hub**
2. Click **Add** → browse to the project folder
3. Open with **Unity 2022.3.x**
4. Wait for Unity to import and compile scripts (may take 1–2 minutes)
5. When prompted about TextMeshPro Essentials, click **Import TMP Essentials**

---

## Step 2 — Create a New Scene

1. `File → New Scene → Basic (Built-in)`
2. Save immediately: `File → Save As` → name it **`SmartRelaxationRoom`**
3. Save to `Assets/Scenes/SmartRelaxationRoom.unity`

---

## Step 3 — Run the Room Builder (Auto-Setup)

The `RoomBuilder.cs` script creates the entire scene for you.

1. In the **Project** panel, find `Assets/Scripts/Editor/RoomBuilder.cs`
2. In the Unity **menu bar**, click: `Tools → Smart Relaxation Room → Build Full Scene`
3. Check the **Console** — you should see: `=== RoomBuilder: Scene built successfully! ===`
4. The Hierarchy will now contain:
   - `Room` (floor, walls, furniture)
   - `Player` with `Main Camera` child
   - `Directional Light`
   - `EnvironmentManager`
   - `TriggerZones` (5 trigger zones)
   - `Canvas_SETUP_MANUALLY` (placeholder — replace in Step 5)

> ⚠️ **Delete** the `Canvas_SETUP_MANUALLY` object after you create the real Canvas in Step 5.

---

## Step 4 — Set the Player Tag

1. Select the **Player** object in Hierarchy
2. At the top of the Inspector, open the **Tag** dropdown
3. Select **Player** (it should already be set by RoomBuilder)
4. If "Player" is not in the list: click `Add Tag → +` → type `Player` → save

---

## Step 5 — Create the Canvas UI

### 5a — Create Canvas

1. `Right-click Hierarchy → UI → Canvas`
2. Rename it to `Canvas`
3. In the Inspector, set:
   - **Render Mode**: Screen Space — Overlay
   - **UI Scale Mode**: Scale With Screen Size
   - Reference Resolution: **1920 × 1080**

### 5b — Add a Semi-Transparent Panel (Mood Selection)

1. `Right-click Canvas → UI → Panel`
2. Rename to `MoodPanel`
3. Set **Rect Transform**: Anchor = top-right, Width = 320, Height = 380, Pos X = -170, Pos Y = -200
4. Set **Image Color** to black with alpha ~150 (semi-transparent dark)

### 5c — Add Title Text

1. `Right-click MoodPanel → UI → Text - TextMeshPro`
2. Rename to `TitleText`
3. Text: **Smart Relaxation Room**
4. Font Size: 18, Bold, Center aligned

### 5d — Add Three Mood Buttons

Repeat 3 times inside `MoodPanel`:

1. `Right-click MoodPanel → UI → Button - TextMeshPro`
2. Button names and labels:

| Object Name | Button Label |
|---|---|
| `StressedButton` | 😟  Stressed |
| `TiredButton` | 😴  Tired |
| `UnfocusedButton` | 🙁  Unfocused |

3. Set button **Width = 260, Height = 52**
4. Stack them vertically with 8px gaps
5. Color the buttons:
   - Stressed: `#3366CC` (blue-grey)
   - Tired: `#CC7700` (amber)
   - Unfocused: `#445566` (dark slate)

### 5e — Add Reduce Effects Button

1. `Right-click MoodPanel → UI → Button - TextMeshPro`
2. Rename to `ReduceEffectsButton`, Label: **⚙ Reduce Effects**
3. Place below the 3 mood buttons
4. Color: `#2A2A35`

### 5f — Add HUD Text Fields (full screen canvas)

Create these **directly on Canvas** (not inside MoodPanel):

#### Instruction Text
- `Right-click Canvas → UI → Text - TextMeshPro`
- Name: `InstructionText`
- Anchor: bottom-left, Pos X = 20, Pos Y = 60
- Width = 560, Height = 60
- Font Size: 14, Color: White, Alpha: 200

#### Status Text
- `Right-click Canvas → UI → Text - TextMeshPro`
- Name: `StatusText`
- Anchor: top-left, Pos X = 20, Pos Y = -20
- Width = 400, Height = 40
- Font Size: 16, Bold, Color: White
- Default text: `— No Mode Active —`

#### Feedback Text
- `Right-click Canvas → UI → Text - TextMeshPro`
- Name: `FeedbackText`
- Anchor: bottom-center, Pos Y = 20
- Width = 700, Height = 50
- Font Size: 15, Center aligned, Color: White
- Default text: `Welcome! Select your mood or walk into a zone.`

#### ESC Hint Text
- `Right-click Canvas → UI → Text - TextMeshPro`
- Name: `EscHintText`
- Anchor: bottom-left, Pos X = 20, Pos Y = 20
- Width = 500, Height = 30
- Font Size: 12, Color: Light grey

---

## Step 6 — Add AudioSource & Audio Clips

1. Select **EnvironmentManager** in the Hierarchy
2. `Add Component → Audio → Audio Source`
3. Uncheck **Play On Awake**
4. Check **Loop**
5. Import your 3 audio clips into `Assets/Audio/`
6. In the EnvironmentManager Inspector, drag:
   - **Calm Audio** → your calm/nature clip
   - **Energy Audio** → your energetic clip
   - **Focus Audio** → your focus/lo-fi clip
   - **Background Audio** → the AudioSource component on EnvironmentManager

---

## Step 7 — Wire Up EnvironmentManager

Select **EnvironmentManager** in the Hierarchy, then in the Inspector:

| Field | Drag From Hierarchy |
|---|---|
| Directional Light | `Directional Light` object |
| Lamp Light | `Room → LampLight` |
| Background Audio | `EnvironmentManager` (AudioSource component) |
| Status Text | `Canvas → StatusText` |
| Feedback Text | `Canvas → FeedbackText` |
| Calm Audio | `Assets/Audio/` your calm clip |
| Energy Audio | `Assets/Audio/` your energy clip |
| Focus Audio | `Assets/Audio/` your focus clip |

---

## Step 8 — Add MoodButtonController

1. Select **Canvas** in the Hierarchy
2. `Add Component → Scripts → MoodButtonController`
3. Wire up in the Inspector:

| Field | Value |
|---|---|
| Env Manager | `EnvironmentManager` object |
| Stressed Button | `Canvas → MoodPanel → StressedButton` |
| Tired Button | `Canvas → MoodPanel → TiredButton` |
| Unfocused Button | `Canvas → MoodPanel → UnfocusedButton` |
| Reduce Button | `Canvas → MoodPanel → ReduceEffectsButton` |

---

## Step 9 — Add UIManager

1. Select **Canvas** in the Hierarchy
2. `Add Component → Scripts → UIManager`
3. Wire up:

| Field | Value |
|---|---|
| Mood Panel | `Canvas → MoodPanel` |
| Instruction Text | `Canvas → InstructionText` |
| ESC Hint Text | `Canvas → EscHintText` |

---

## Step 10 — Verify Trigger Zones

1. Select each object under `TriggerZones` in the Hierarchy
2. Confirm each has:
   - `BoxCollider` with **Is Trigger = ✓**
   - `ProximityTrigger` script with correct **Trigger Type**
   - **Env Manager** field pointing to `EnvironmentManager`

| Zone Object | Trigger Type |
|---|---|
| RelaxTrigger | Calm |
| FocusTrigger | Focus |
| EnergyTrigger | Energy |
| LampTrigger | Lamp |
| PlantTrigger | Plant |

---

## Step 11 — Build Scene Settings

1. `File → Build Settings`
2. Click **Add Open Scenes** → `SmartRelaxationRoom` should appear in the list
3. Platform: **PC, Mac & Linux Standalone**
4. Click **Switch Platform** if needed
5. Close Build Settings

---

## Step 12 — Final Test (Play Mode)

Click **Play** (▶) and verify the checklist below:

### ✅ Testing Checklist

#### Movement
- [ ] WASD moves the player correctly
- [ ] Mouse look rotates view smoothly
- [ ] Player cannot walk through walls
- [ ] Pressing ESC releases the cursor

#### Mood Buttons (press ESC first to see cursor)
- [ ] **Stressed** → light turns blue, calm audio plays, status says "Calm Mode Active"
- [ ] **Tired** → light turns warm orange, energy audio plays, status says "Energy Mode Active"
- [ ] **Unfocused** → light turns bright white, focus audio plays, status says "Focus Mode Active"
- [ ] Button highlights change color when clicked

#### Trigger Zones (walk into each coloured floor pad)
- [ ] Walking into **blue pad (left)** → Calm Mode activates automatically
- [ ] Walking into **white pad (centre)** → Focus Mode activates
- [ ] Walking into **orange pad (right)** → Energy Mode activates
- [ ] Walking near **Lamp** → Lamp light turns on, message appears
- [ ] Walking near **Plant** → "Take a deep breath" message appears
- [ ] Leaving Lamp zone → Lamp light turns off

#### Accessibility
- [ ] **Reduce Effects** button lowers audio volume and dims light
- [ ] Clicking again restores full effects

#### Console
- [ ] No red error messages in Console window
- [ ] Only `[EnvironmentManager]` and `[ProximityTrigger]` debug logs visible

---

## Step 13 — Prepare Submission Folder

Only submit these folders (zip them together):

```
SmartRelaxationRoom/
├── Assets/
│   ├── Audio/           ← your .wav/.mp3 files
│   ├── Documents/       ← ConceptProposal.md, SelfStudyExplanation.md
│   ├── Scenes/          ← SmartRelaxationRoom.unity
│   └── Scripts/         ← all .cs files
├── Packages/
│   └── manifest.json
└── ProjectSettings/
    └── (all .asset files)
```

**Do NOT include:** `Library/`, `Temp/`, `Logs/`, `obj/`, `Builds/`, `UserSettings/`

---

## Hierarchy Overview (Final)

```
SmartRelaxationRoom [Scene]
├── Player                          ← Tag: Player, PlayerMovement.cs
│   └── Main Camera                 ← Tag: MainCamera, Camera, AudioListener
├── EnvironmentManager              ← EnvironmentManager.cs, AudioSource
├── Directional Light               ← Light (Directional)
├── Room
│   ├── Floor / Ceiling / WallX4
│   ├── Window
│   ├── TableTop + TableLegX4
│   ├── ChairSeat + ChairBack + ChairLegX4
│   ├── LampPole / LampShade / LampLight (Point Light, disabled)
│   ├── PlantPot / PlantBush
│   ├── MoodSelectionArea
│   ├── RelaxZonePad
│   ├── FocusZonePad
│   └── EnergyZonePad
├── TriggerZones
│   ├── RelaxTrigger                ← ProximityTrigger (Calm)
│   ├── FocusTrigger                ← ProximityTrigger (Focus)
│   ├── EnergyTrigger               ← ProximityTrigger (Energy)
│   ├── LampTrigger                 ← ProximityTrigger (Lamp)
│   └── PlantTrigger                ← ProximityTrigger (Plant)
└── Canvas                          ← MoodButtonController.cs, UIManager.cs
    ├── MoodPanel
    │   ├── TitleText
    │   ├── StressedButton
    │   ├── TiredButton
    │   ├── UnfocusedButton
    │   └── ReduceEffectsButton
    ├── InstructionText
    ├── StatusText
    ├── FeedbackText
    └── EscHintText
```
