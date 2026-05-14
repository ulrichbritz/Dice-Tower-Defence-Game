# Systems Map — Dice Tower Defence

Hand-authored game knowledge for the `regression-scout` QA tool. Each system maps
code → player-facing behavior → how QA actually tests it. Keep `Fragile:` current
— that's what drives P0 recommendations.

Game shape: single-player wave-survival. Player auto-targets and shoots enemies;
each shot rolls a die for a damage multiplier; between waves a shop opens. Player
movement is currently disabled (input commented out).

---

## System: Round / Game Flow
Code: Assets/Scripts/WorldManagers/GameManager.cs
Scenes/Prefabs: World_Scene_01.unity; Assets/Data/Prefabs/World Managers/** ; InGameShop UI
Player-facing: Starting a run, wave 1 begins, waves advance, "Next Wave" button, shop opening/closing between waves, run progression.
Fragile: Recently refactored ("round start code handled by the game manager"). `CurrentWaveNumber` starts at 0 so `StartNextWave()` makes it 1 — off-by-one prone. `DelayedShopOpen()` has a hardcoded 1s wait. `EndWave()` early-returns if `!WaveCurrentlyInProgress` — double-call silently no-ops. Wave-complete detection depends on `WorldAIManager.SpawnedCharacters` being emptied.
How QA tests: Start a run; confirm wave 1 spawns. Kill a full wave; confirm shop opens (~1s later) and wave counter reads correctly. Click Next Wave; confirm shop closes and next wave spawns. Try clicking Next Wave twice fast. Play 3+ waves to check counter never desyncs.
Depends on: AI / Enemies, UI / HUD / Shop, Scene Management, Player Character

## System: Combat
Code: Assets/Scripts/Character/Character/CharacterCombatManager.cs, Assets/Scripts/Character/Player/PlayerCombatManager.cs, Assets/Scripts/Character/AICharacter/AICombatManager.cs
Scenes/Prefabs: Player.prefab; Weapon_Handgun_01.prefab
Player-facing: Player auto-targets nearest enemy in range, faces it, plays the shoot animation, deals damage. Attack cooldown gates fire rate.
Fragile: Most recently worked-on area ("combat controller", "shoot animation"). Die roll is `await`ed mid-attack — async timing between roll, animation, and damage application. Damage = `weapon.CurrentPhysicalDamage * dieResult`. Damage applied by directly mutating enemy health (bypasses Effects system). Only fires while `GameManager.WaveCurrentlyInProgress`. `Debug.Log` and an always-on `OnGUI()` debug panel left in the attack path. `AICombatManager` is an empty stub — enemies cannot attack yet. Attack animation forces `applyRootMotion:false`, `canRotate:false`.
How QA tests: Start a wave; confirm player auto-targets the closest live enemy and rotates to face it. Watch a kill: die roll → shoot anim → enemy health drops, in that order. Confirm damage scales with die result. Confirm player does NOT fire between waves. Stand multiple enemies at equal range — confirm sane target pick. Confirm cooldown spacing matches weapon attack speed.
Depends on: Dice, Equipment / Items, Animation, AI / Enemies, Health / Stats, Round / Game Flow

## System: Dice
Code: Assets/Scripts/Dice/**
Scenes/Prefabs: D6_Standard_01.prefab; Assets/Art/Models/Dice/**.prefab
Player-facing: The 3D die spins/bounces and lands on a face; that face number is the combat damage multiplier. Die faces can be upgraded (face numbers changed).
Fragile: `DieController.GetCorrectedRotationForFace()` HARDCODES face rotations (1-6) and ignores `DieFace.Rotation` data — editing DieFace rotations has no effect; out-of-1-6 faces silently return `Vector3.zero`. Bounce animation (sine-based, `bounceCount`/`bounceHeight`) is tuning-sensitive. `RollDieAndGetResult()` is async via `WorldRoutineManager` — combat awaits it. `DieTestController.cs` is an in-game keyboard test harness (R/1-6/C/S/Space) — NOT shippable; flag any reliance on it. `Debug.Log` in roll path. `ChangeFaceNumber`/`SetAllFaceNumbers` mutate the live die.
How QA tests: Roll the die repeatedly — confirm it lands flat on a real face and the reported number matches the visible top face. Confirm the awaited result feeds combat damage. If face numbers were changed/upgraded, confirm renderer pips and returned value agree. Watch for the die never settling or returning 0.
Depends on: Equipment / Items (die model loaded into head slot)

## System: Player Character
Code: Assets/Scripts/Character/Player/PlayerManager.cs, Assets/Scripts/Character/Character/CharacterManager.cs
Scenes/Prefabs: Player.prefab
Player-facing: The player entity — exists across scenes, holds all sub-managers (locomotion, combat, equipment, inventory, stats, animator), drives the per-frame action/move/rotate gates.
Fragile: `DontDestroyOnLoad` — re-entering the world scene could double the player. Awake→base.Awake→component-getter→Start chain is execution-order sensitive; `Start` builds health UI with hardcoded values (TODO: "move when we add saving/loading"). `PlayerAnimatorManager` is an empty stub. Player is instantiated by `GameManager.StartRun()` from `WorldSaveGameManager.PlayerPrefab` (null unless set in inspector).
How QA tests: Start a run — confirm exactly one player spawns and all sub-systems work (shoots, has health bar, equips die+weapon). Transition scenes (title→world, replay) — confirm no duplicate player and references survive.
Depends on: Combat, Equipment / Items, Health / Stats, Input, Animation, UI / HUD / Shop

## System: AI / Enemies
Code: Assets/Scripts/WorldManagers/WorldAIManager.cs, Assets/Scripts/Character/AICharacter/**, Assets/Scripts/States/**
Scenes/Prefabs: Zombie_01.prefab; WorldAIManager.prefab
Player-facing: Waves of zombies spawn at spawn points and pathfind toward the player via NavMesh. Idle→PursueTarget state machine. Enemies despawn on death; emptying the list ends the wave.
Fragile: `SpawnCharacters()` had spawn-camera code removed — large commented-out block still present (lines ~40-68). No null check before `aiCharacter.AssignTarget()` after spawn. NavMesh agent re-placement has fallback logic (history of placement bugs). State machine ticks in `FixedUpdate` — execution-order sensitive; state objects are cloned per-instance from SOs. `AICombatManager` empty — enemies don't attack. `AssignTarget()` unguarded if `PlayerManager.Instance` is null.
How QA tests: Start a wave — confirm all enemies spawn at valid NavMesh positions and walk toward the player (no T-posing, no stuck-at-spawn, no falling through floor). Kill enemies one by one — confirm each despawns and the LAST kill ends the wave. Spawn-kill-respawn across multiple waves — watch for NavMesh errors or ghost entries in `SpawnedCharacters`.
Depends on: Round / Game Flow, Player Character, Health / Stats

## System: Health / Stats / Death
Code: Assets/Scripts/Character/Character/CharacterStatsManager.cs, Assets/Scripts/Character/Character/CharacterStats.cs, Assets/Scripts/Character/Player/PlayerStatsManager.cs, Assets/Scripts/Character/AICharacter/AIStatsManager.cs
Scenes/Prefabs: Player.prefab, Zombie_01.prefab; CharacterStats *.asset
Player-facing: Health bars (screen HUD for player, floating world-space for all). Taking damage updates the bar; health ≤ 0 triggers a death animation and despawn.
Fragile: `CurrentCharacterStats = Instantiate(BaseCharacterStats)` clones the SO at runtime (TODO re: save/load). Health-changed event fires even when the value didn't change; clamping happens inside the callback. `IsDead` guard prevents re-trigger but has no reset path (no respawn). Death routine hardcodes "Death_F_1H" anim + 5s wait via `WorldRoutineManager`. Combat writes health directly — no validation layer.
How QA tests: Damage an enemy — confirm floating bar updates and matches `weapon damage × die roll`. Kill it — confirm death anim plays then it despawns (~5s). Confirm player HUD bar tracks player health. Check overkill (huge die roll) doesn't go negative or break the bar.
Depends on: Combat, UI / HUD / Shop, Animation

## System: Equipment / Items
Code: Assets/Scripts/Character/Character/CharacterEquipmentManager.cs, Assets/Scripts/Character/Player/PlayerEquipmentManager.cs, Assets/Scripts/Character/Player/DieModelInstantiationSlot.cs, Assets/Scripts/Character/Character/WeaponModelInstantiationSlot.cs, Assets/Scripts/Items/**, Assets/Scripts/Dice/DieItem.cs
Scenes/Prefabs: Player.prefab; Weapon_Handgun_01.prefab; D6_Standard_01.prefab; WeaponItem/DieItem *.asset
Player-facing: Weapon model loads into the hand slot, die model loads into the head slot. Weapon stats (damage, attack speed, range, crit) feed combat.
Fragile: `LoadDieHead()` / weapon loads have NO null checks — null `CurrentDieHead`/weapon Instantiates nothing silently, breaking combat downstream. `WeaponItem` has `BaseXxx` + `CurrentXxx` stat pairs with no visible sync mechanism. `RangedWeaponItem` is an empty stub. Slot components found via `GetComponentsInChildren` in `Start` — order-sensitive. `DieController` reference is grabbed off the loaded die model.
How QA tests: Start a run — confirm the weapon model appears in-hand and the die model on the head, both correctly positioned. Confirm combat reads real weapon stats (damage/range/attack speed). If a weapon/die asset changed, confirm the right model and stats load.
Depends on: Dice, Combat, Player Character

## System: UI / HUD / Shop
Code: Assets/Scripts/WorldManagers/PlayerUIManager.cs, Assets/Scripts/UI/**
Scenes/Prefabs: Player UI Manager.prefab; World Space HUD.prefab; Player World Space HUD Variant.prefab
Player-facing: Player health HUD, floating world-space health bars, the between-waves shop menu and its "Next Wave" button.
Fragile: `PlayerUIManager.Awake()` finds children via `GetComponentsInChildren` with no null checks. `InGameShopUIManager` is tightly coupled — its button directly calls `GameManager.Instance.StartNextWave()` (no event indirection); listener added in `Start`, removed in `OnDestroy`. `UI_StatBar` drives the bar visuals. NOTE: UnityEvent/button bindings in the prefab YAML reference method names as strings — renaming a handler breaks the button with no compile error.
How QA tests: Confirm player HUD bar shows and tracks health. Confirm floating bars appear above enemies and the player. After a wave, confirm the shop opens and the Next Wave button actually starts the next wave. Confirm shop closes when the wave starts.
Depends on: Round / Game Flow, Health / Stats

## System: Camera
Code: Assets/Scripts/Cameras/PlayerCameraManager.cs
Scenes/Prefabs: Player Camera.prefab (CinemachineCamera)
Player-facing: Game camera; zoom/focus animations for spawns and scene transitions (focus code largely not wired into the current flow).
Fragile: Much of the API (`MoveToPosition`, `FocusSpawn`, `ZoomOutForSpawn`) is currently NOT called — the WorldAIManager callsites were commented out. `isAnimating` flag blocks overlapping animations (can drop/queue a request). Hardcoded `focusOffset (0,8,-5)`. Ortho-size animation duration is derived from size delta / speed — tiny deltas animate near-instantly.
How QA tests: Confirm the gameplay camera frames the play area correctly and follows as expected. If camera focus/zoom code was re-enabled, confirm spawn/transition zooms play once and return to normal without sticking.
Depends on: Scene Management, AI / Enemies

## System: Scene Management
Code: Assets/Scripts/WorldManagers/WorldSceneManager.cs
Scenes/Prefabs: TitleScene_01.unity, World_Scene_01.unity
Player-facing: Loading/transitioning between title and gameplay scenes, fade transitions, additive loads.
Fragile: `WorldSceneIndex = 1` hardcoded — assumes build order Title(0)/World(1); reordering Build Settings breaks it. `minimumLoadTime = 1.0s` hardcoded. Additive-load-already-loaded returns early with a warning (soft failure). Fade uses `UITweens.FadeImageColor`.
How QA tests: Title → start run → world scene transition with fade, no hang. Confirm the correct scene loads (not WorldScene_01_Old). Return to title and back if that path exists. Check Build Settings scene order if this system changed.
Depends on: Round / Game Flow

## System: Input
Code: Assets/Scripts/WorldManagers/PlayerInputManager.cs
Scenes/Prefabs: Player Input Manager.prefab; PlayerControls input actions asset
Player-facing: Captures player input (new Input System). Movement input is currently COMMENTED OUT — player does not move.
Fragile: `HandleMovementInput()` call is commented out (~line 88) — movement intentionally disabled; if a diff re-enables it, that's a major behavior change. `PlayerControls` instantiated in `OnEnable` — possible leak on repeated enable/disable. Input enabled/disabled by active scene + `OnApplicationFocus`. `Player` reference can be null.
How QA tests: Confirm input only responds in the world scene. If movement was re-enabled, regression-test the whole movement + combat-while-moving + camera interaction surface (currently untested territory). Alt-tab and back — confirm input re-enables.
Depends on: Player Character, Scene Management

## System: Save / Load (STUB)
Code: Assets/Scripts/WorldManagers/WorldSaveGameManager.cs
Scenes/Prefabs: World Save Game Manager.prefab
Player-facing: None yet — only holds the `PlayerPrefab` reference used by `GameManager.StartRun()`.
Fragile: Entirely a stub — no serialization, no file I/O. `PlayerPrefab` is null unless wired in the inspector. Many other systems carry "TODO when we add saving/loading" comments — treat any real save/load code here as brand-new, untested.
How QA tests: Today, only: confirm `PlayerPrefab` is assigned so `StartRun()` can spawn the player. If real save/load lands, it's all-new and needs full coverage.
Depends on: Player Character

## System: Effects (NOT INTEGRATED)
Code: Assets/Scripts/Effects/**, Assets/Scripts/Character/Character/CharacterEffectsManager.cs
Scenes/Prefabs: InstantCharacterEffect / TakeHealthDamageInstantEffect *.asset
Player-facing: None yet — intended damage/effect pipeline, but combat bypasses it and writes health directly.
Fragile: Framework exists but is unused. If a diff routes combat damage THROUGH this system, that's a core-loop rewrite — regression-test all damage/death behavior.
How QA tests: No direct test today. If combat is migrated onto the Effects system, re-run the entire Combat + Health/Death test set.
Depends on: Combat, Health / Stats
