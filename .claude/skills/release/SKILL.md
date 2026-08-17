---
name: release
description: Prepare and publish a versioned release — version bumps, changelog, build, commit, tag, push
disable-model-invocation: true
argument-hint: "[major|minor|patch]"
---

# Release

Prepare and publish a new release for Bionic Thumb Guild.

The user may pass a bump type as `$ARGUMENTS` (one of `major`, `minor`, or `patch`). If omitted, ask which bump type they want.

## Current state

!`git describe --tags --abbrev=0 2>/dev/null || echo "no tags found"`
!`git log "$(git describe --tags --abbrev=0 2>/dev/null || echo 'HEAD~10')..HEAD" --oneline --no-merges`

## Steps

Work through each step below **one at a time**, confirming with the user before moving to the next. Do not batch steps together.

### 1. Determine version

- Read the current version from `About/About.xml` (`<modVersion>`)
- Calculate the new version from the bump type (`$ARGUMENTS` or ask)
- Show the user: current version, bump type, and new version
- **Ask the user to confirm** before proceeding

### 2. Review changes for changelog

- Show the commit log since the last tag (already displayed above)
- If the repo has no tags yet this is the first release: use the full history
  (`git log --oneline --no-merges`) and summarise the mod's shipped feature set rather than a diff.
  **This does not currently apply** — the repo already carries a `v1.0.0` tag from its initial
  commit baseline (`8c2f6ab`, predating this skill), so `git describe --tags --abbrev=0` above will
  resolve to a real tag and the next release is a normal diff-since-last-tag. Keep this fallback
  in the skill anyway: it is what fires correctly if tags are ever pruned, if this skill is copied
  to a sibling mod that genuinely has none yet, or in the (unlikely) event `git describe` finds
  nothing here in the future.
- Draft changelog notes grouped by category (Fixes, Features, Polish/Other)
- Omit chore/version-bump commits from the changelog
- **Present the draft to the user and ask them to confirm or edit**

### 3. Refresh translation expectations and check freshness

Run, in order:
```bash
python3 Scripts/refresh-translation-expectations.py
python3 Scripts/check-translations.py --strict
```

- The refresh script refuses to start while RimWorld is already open (it
  needs an exclusive boot for the mod-list swap). If it reports that, **stop
  and ask the user** to close the client, and rerun only after they confirm
  it is free.
- The first command regenerates `Scripts/expected-injections.json` by
  launching the local RimWorld client with `-l10nprobe` (graphical boot,
  ~1-2 min; the L10nProbe dev mod dumps every DefInjected key the live game
  expects, then quits). This is what surfaces vanilla-inherited and
  C#-default strings a def-XML scan cannot see. Report its diff summary.
- **This mod ships no translations yet**: `Scripts/expected-injections.json` is checked in and
  the checker knows a missing `1.6/Languages/` tree is a legal state — with no languages, it
  prints two informational notes, runs the sidecar-freshness check against `Defs/`, and exits 0.
  A failure here therefore always means the sidecar is stale (a def was added or its English
  edited without a regen) — rerun `refresh-translation-expectations.py` and commit the diff, do
  not hand-edit the sidecar.
- If the diff shows **added or changed keys**, translate them in every
  language now (the `translate` skill's update pass), then rerun the checker.
- Report the per-language checker result (missing keys, stale entries,
  errors). CI's release gate runs the same script without `--strict` against
  the checked-in sidecar; the stricter local run surfaces warnings while
  there is still time to act on them.
- If the sidecar or any translations changed, commit them as their own
  `fix(l10n)` commit (show the diff and **ask the user to confirm**) before
  moving on — step 8 stages only the version-bump files.

### 4. Refresh Steam Workshop page translations

The Workshop title and description live in
`.steamworkshop/Description/English.txt`: line 1 is the title, then a
blank line, then the BBCode description (see `.steamworkshop/README.md`).
This mod has no `Languages/` tree yet, so English is the only file today.

- Diff the English source against the last release:
  ```bash
  git diff $(git describe --tags --abbrev=0) -- .steamworkshop/Description/English.txt
  ```
- If nothing changed, say so and move on.
- If in-game localization is ever added, also check `1.6/Languages/` for
  languages with no description file yet, and spawn one translation
  subagent per affected language (cheaper model, in parallel) to update or
  create its file, grounded in the `translate` skill's glossary and the
  mod's own committed `1.6/Languages/<Language>/` strings, preserving
  BBCode tags and the title-line format. Subagents never commit.
- Review any diffs, then commit them as their own `docs:` commit (show the
  diff and **ask the user to confirm**).

### 5. Update CHANGELOG.md

- Unlike TSX's changelog, this one already carries an `## [Unreleased]` heading (currently empty,
  directly below the Keep a Changelog intro paragraph) — keep it, empty, at the top. Add the new
  `## [X.Y.Z] - YYYY-MM-DD` section directly below it, using today's date.
- Use the confirmed changelog notes from step 2, formatted in Keep a Changelog style (`### Added`, `### Fixed`, etc.)
- Update the `[Unreleased]` link reference at the bottom to compare from the new tag
  (`https://github.com/sam-hunt/BionicThumbGuild/compare/vX.Y.Z...HEAD`), and add a new
  `[X.Y.Z]: https://github.com/sam-hunt/BionicThumbGuild/releases/tag/vX.Y.Z`
  link reference directly below it, above any older ones (matches the existing
  `[1.0.0]: .../releases/tag/v1.0.0` style — direct tag link, not a compare)
- Show the diff and **ask the user to confirm**

### 6. Bump versions

Update the version string in all three files:
- `About/About.xml` — `<modVersion>`
- `Source/1.6/Properties/AssemblyInfo.cs` — `AssemblyVersion` and `AssemblyFileVersion` (four-part, `X.Y.Z.0`)
- `README.md` — version badge (`Version-X.Y.Z`)

Show the diff and **ask the user to confirm** the changes look correct.

### 7. Clean build and deploy

Run:
```bash
dotnet clean BionicThumbGuild.sln
dotnet build BionicThumbGuild.sln -c Release
```

Report the build result. If the build fails, stop and help the user fix it. **Ask the user to confirm** before proceeding to commit.

### 8. Stage, commit, tag

- Stage only the release files: `About/About.xml`, `Source/1.6/Properties/AssemblyInfo.cs`, `README.md`, `CHANGELOG.md`
- If there are other modified tracked files, list them and ask the user whether to include them
- Commit with message: `chore: Bump version to X.Y.Z`
- Tag with: `vX.Y.Z`
- Show `git log --oneline -3` and `git tag -l 'v*' --sort=-v:refname | head -5`
- **Ask the user to confirm** before pushing

### 9. Push

```bash
git push && git push --tags
```

Show the final result and the changelog notes for the user to copy into Steam Workshop / GitHub release notes. If step 4 updated any Workshop description files, list the affected languages and remind the user to paste each updated title and description into the Workshop page's per-language edit UI (Steam's own language names differ: schinese, koreana, brazilian, latam, ...).
