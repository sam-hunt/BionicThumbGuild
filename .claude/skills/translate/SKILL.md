---
name: translate
description: Generate, update, or audit mod localization (DefInjected only — this mod has no Keyed strings) for a target language, grounded in vanilla RimWorld Core terminology for bionic prosthetics, surgery, social interactions, mood thoughts, and trader stock. Use when asked to add a language, update translations, or check translation freshness.
argument-hint: "[language, e.g. German | update | check]"
---

# Translate

Produce or refresh localization files for Bionic Thumb Guild. English is
the source of truth; every other language derives from it.

**The family-wide process lives in the `l10n/` submodule — load these first,
and only these** (progressive disclosure; if `l10n/` is empty, run
`git submodule update --init`):

- `l10n/process.md` — non-negotiables, file/format conventions, terminology
  grounding method, and the generation / update / audit workflows. This is
  the workflow authority; follow it step by step.
- `l10n/languages/<Language>.md` — the target language's engine mechanics,
  style rules, and vanilla-grounded common vocabulary. Read ONLY the target
  language's file.
- `glossary/<Language>.md` (beside this file) — this mod's own coined-term
  table for the target language. Read it in the same pass.
- `l10n/lessons.md` — cross-language lessons; read when generating a new
  language, skim otherwise.
- `l10n/workshop.md` — Steam Workshop description/title conventions. This
  mod has no localized Workshop title today (see glossary/README.md).

**Where learnings land:** mod-independent findings (engine mechanics, a
language's grammar rule, corpus style facts) go in the `l10n/` submodule —
edit the canonical checkout at `~/dev/rimworld-l10n`, commit there, then bump
the pin here. Mod-specific findings (coined terms, phrasing decisions) go in
`glossary/<Language>.md`.

## This mod's translation surface

- **No Keyed surface at all** — there is no settings window, no
  mod-configuration UI, and no other player-facing prose outside the Defs
  themselves. The checker's `ALLOW_NO_KEYED_SURFACE = True` config reflects
  this as a legal state, not a config error: don't create a `Keyed/` folder
  speculatively, for English or any other language, unless this mod grows a
  settings window or other free-standing prose.
- The entire translatable surface is **DefInjected**, sourced from the Defs
  this mod ships:
  - `1.6/Defs/ThingDefs/ThingDefs_BionicThumb.xml` — `BTG_BionicThumb`
    (`label`, `description`)
  - `1.6/Defs/HediffDefs/Hediffs_BionicThumb.xml` — `BTG_BionicThumb`
    (`label`, `labelNoun`, `description`)
  - `1.6/Defs/RecipeDefs/RecipeDefs_BionicThumb.xml` — `BTG_InstallBionicThumb`
    (`label`, `description`, `jobString`)
  - `1.6/Defs/InteractionDefs/InteractionDefs_ThumbsUp.xml` — `BTG_ThumbsUp`
    (`label`, and the `logRulesInitiator/rulesStrings` interaction-log lines)
  - `1.6/Defs/ThoughtDefs/ThoughtDefs_ThumbsUp.xml` — `BTG_ThumbsUp` and
    `BTG_ThumbsUpMood` (each `<stages><li>`'s `label` and, for the mood
    thought, `description`) — note `BTG_ThumbsUp` names both an
    InteractionDef and a ThoughtDef; don't conflate their two DefInjected
    folders.
  - All five def types (`ThingDef`, `HediffDef`, `RecipeDef`,
    `InteractionDef`, `ThoughtDef`) are vanilla types — this mod defines no
    C# Def subclass of its own, so every DefInjected folder resolves bare.
  - As always, `Scripts/expected-injections.json` is the authority for the
    exact legal key set (inherited `ParentName` base fields and C#-default
    strings don't show up in this repo's own XML) — see `l10n/process.md`.
- **`BTG_ThumbsUp`'s `logRulesInitiator/rulesStrings` are grammar-resolved,
  not plain Keyed strings** — they route through RimWorld's full rulepack
  `GrammarResolver` (the same family a weapon mod's combat log reaches), not
  the `GrammarResolverSimple` a plain `"key".Translate()` string reaches (see
  `l10n/lessons.md`'s "know which resolver" entry for what that distinction
  buys and costs). They carry `[INITIATOR_nameDef]` / `[RECIPIENT_nameDef]`
  symbols — bare pawn-name tokens, not `_definite`/`_possessive` article or
  pronoun symbols. Translate these seven lines for tone (wry, a little
  brand-obsessed) rather than literally; per-language quoting/marking
  decisions for the two symbols are in `glossary/<Language>.md`. The
  `ThoughtDef` stage labels/descriptions this interaction produces are plain
  DefInjected strings, not grammar-resolved — normal rules apply there.
- **No compat load roots** — this mod requires no DLC (`About.xml` has no
  DLC `modDependencies` beyond Harmony) and gates no content behind
  `MayRequire`, so it ships no `1.6/Mods/<Name>/` root and `REQUIRED_DLCS`
  in the checker is `set()`.

## This mod's grounding domain

Domain DLC: **Core only**. This mod requires no DLC, so there is no
Biotech/Royalty/Odyssey/Anomaly tar to ground its own vocabulary against.
Terms that MUST be grounded before use: bionic/artificial body part
vocabulary (Core's `Bionic arm`/`Bionic leg`/`Bionic eye` HediffDefs and
their shared `AddedBodyPartBase` phrasing), surgery/installation recipe
vocabulary (`RecipeDef.jobString`, "Install/installing X" phrasing from
Core's existing bionic recipes), manipulation/part-efficiency stat language,
social-interaction log conventions (Core's `Chitchat`/`DeepTalk`/`Insult`
InteractionDefs — tone and grammar patterns, not vocabulary to reuse), and
opinion/mood thought vocabulary (Core's ThoughtDef stage phrasing for social
vs. personal-mood memories). The one deliberate exception: this mod's
trader-price phrasing reuses Odyssey's `GoldInlay`/`Ugly` stat descriptions
verbatim as a style reference, flagged as DLC-sourced in the glossary rows
that use it — a player without Odyssey still sees this mod's own Core-only
feature.

**No language pass has run in this repo yet.** Every `glossary/<Language>.md`
file is a placeholder today (interaction-log quoting decisions aside, which
were analyzed structurally without a full generation pass) — treat glossary
content as style/mechanics reference only until an actual generation pass
grounds bionic/surgery/interaction/thought vocabulary against the Core tar
and records it there.

## Workflows

Follow `l10n/process.md`'s Initial generation / Update pass / Audit-only
workflows verbatim. This mod's specifics on top:

- The checker: `python3 Scripts/check-translations.py` (`--strict` for new
  languages). It runs sidecar-freshness checks even with no Languages/ tree
  at all — see the note it prints. Sidecar regen: `python3
  Scripts/refresh-translation-expectations.py` (game must be closed; drives
  the deployed L10nProbe).
- Enumerate the target key set from the sidecar's `required` entries per def
  type, not from `1.6/Defs/` — inherited/C#-default fields don't show up
  there (see `l10n/process.md`).
- The public roster (and credits) is CONTRIBUTING.md's localization
  table — update it in the same commit as any language addition or native
  review. Today it lists English (Source) only.
