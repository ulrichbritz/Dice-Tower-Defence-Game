# QA Config — Dice Tower Defence

Flags and paths the `regression-scout` agent reads. Keep this short; it's the
per-project dial board. When porting the tool to another game, rewrite this file
and `systems-map.md` — the `.claude/` engine files stay untouched.

## Flags
- multiplayer: off          # single-player; no Netcode/Mirror/Photon in repo. Skip the Multiplayer section.
- base_branch: main         # default diff base for `/qa-impact` with no argument
- engine: Unity 6 (URP)

## Key asset locations
- scripts: Assets/Scripts/**
- scenes: Assets/Scenes/**          # World_Scene_01.unity (gameplay), TitleScene_01.unity (menu), WorldScene_01_Old.unity (DEAD — flag edits here as suspicious)
- gameplay_prefabs: Assets/Data/Prefabs/**
- art_prefabs: Assets/Art/**         # models only, no game scripts — low regression risk
- scriptable_objects: Assets/Data/** (*.asset)

## Noise to down-rank
- Assets/Art/Placeholders/** and Assets/Data/Prefabs/EditorOnly/** — placeholder/editor-only, not shipped gameplay.
- Assets/Scripts/Utilities/unity-async-routines-master/** — vendored library; a change here is high-blast-radius (everything async) but rarely intentional — flag loudly if touched.

## Conventions
- World managers are singletons (`WorldManager<T>`), accessed via `.Instance`. A change to any `*Manager` Awake/Start order is execution-order sensitive.
- Async flow runs through `WorldRoutineManager.Instance.Run(...)` and the `Routine` type.
- UnityEvents/animation events bind by method-name string in scene/prefab YAML — renames break them silently.
