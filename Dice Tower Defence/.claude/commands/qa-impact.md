---
description: Recommend what QA should test based on a PR or git diff
argument-hint: "[PR number] | --working | (empty = current branch vs main)"
---

Launch the `regression-scout` subagent to analyze code changes and produce a
grounded QA regression-test checklist.

Pass this scope argument to the subagent verbatim: `$ARGUMENTS`

Scope resolution the subagent must follow:
- A bare number (e.g. `42`) → analyze GitHub PR #42: `git fetch origin pull/42/head`
  then `git diff main...FETCH_HEAD` (no `gh` CLI required).
- `--working` → analyze uncommitted changes (`git diff HEAD`).
- Empty → analyze the current branch against main (`git diff main...HEAD`).

Do not analyze the diff yourself. Delegate the whole job to `regression-scout`
and relay its final checklist to the user unchanged.
