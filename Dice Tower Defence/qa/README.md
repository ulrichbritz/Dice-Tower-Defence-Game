# QA Regression Scout

An AI tool that answers one question from a PR or git diff:
**"What should QA manually test because of these code changes?"**

It doesn't just list changed files — it builds the reference graph, reads scene
and prefab wiring, and grounds every recommendation in a hand-authored map of the
game's systems. Output is a prioritized QA checklist (P0/P1/P2) with *why* and
*how to reproduce* for each item.

## Usage

In Claude Code, from the repo root:

```
/qa-impact 42          analyze GitHub PR #42
/qa-impact             analyze the current branch vs main
/qa-impact --working   analyze uncommitted changes
```

You get back a `# QA Test Recommendations` report: a 3-line TL;DR, then sections
for Gameplay / UI / Multiplayer / Edge cases / Indirect regression risks, plus an
honest "Blind spots" list of what a human still needs to check.

## How it's built (two layers)

| Layer | Files | Game-specific? |
|---|---|---|
| **Engine** | `.claude/commands/qa-impact.md`, `.claude/agents/regression-scout.md` | No |
| **Knowledge** | `qa/systems-map.md`, `qa/config.md` | Yes |

The engine is a Claude Code slash command that dispatches a subagent
(`regression-scout`). The subagent's methodology: parse changed entities → grep
the reference graph → scan `.unity`/`.prefab` YAML for wiring and UnityEvent
string bindings → match against the systems map → synthesize the checklist.

The knowledge layer is what makes output specific instead of generic. The
**systems map** describes each game system: which code/scenes/prefabs it owns,
what player-facing behavior it drives, what's fragile, and how QA actually tests
it. The **config** holds flags (e.g. `multiplayer: off`) and asset paths.

## Porting to another game

1. Copy `.claude/commands/qa-impact.md` and `.claude/agents/regression-scout.md`
   into the other repo **unchanged**.
2. Write a new `qa/config.md` — flags, base branch, asset folder layout.
3. Write a new `qa/systems-map.md` — one `## System:` block per game system,
   following the schema in this repo's copy (`Code:`, `Scenes/Prefabs:`,
   `Player-facing:`, `Fragile:`, `How QA tests:`, `Depends on:`).
4. Restart Claude Code so it picks up the project agent + command, then run
   `/qa-impact` on a recent PR and tune the systems map from the output.

The systems map is the product. Budget your time there.

## Maintenance

- When a system gets refactored, update its `Fragile:` line — that's what drives
  the P0 recommendations. A stale map quietly degrades the output.
- When a new system is added, add a `## System:` block for it.
- The engine files rarely need edits; tune the *map*, not the agent.

## Known limitations

- **Requires a session restart** after the `.claude/` files are first added —
  Claude Code loads project agents and commands at startup.
- **PR mode** uses `git fetch origin pull/N/head`; needs a GitHub remote. Works
  on public repos with no auth; private repos need git credentials configured.
- It **reads, never edits** — it's a recommender, not a fixer.
- Binary assets (FBX, textures, audio) and large generated files (`.anim`
  keyframe data) can't be evaluated — those land in "Blind spots" by design.
- It augments human QA judgment; it doesn't replace a test pass.
