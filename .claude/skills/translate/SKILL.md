---
name: translate
description: Generate, update, or audit mod localization (DefInjected only — this mod has no Keyed strings) for a target language, grounded in vanilla RimWorld Core terminology for bionic prosthetics, surgery, social interactions, mood thoughts, and trader stock. Use when asked to add a language, update translations, or check translation freshness.
argument-hint: "[language, e.g. German | update | check]"
---

# Translate

Produce or refresh localization files for Bionic Thumb Guild. English is
the source of truth; every other language derives from it.

## Non-negotiables

- **Run the checker first and last.** `python3 Scripts/check-translations.py`
  validates key sets, placeholders, DefInjected paths, staleness, and file
  hygiene deterministically. Never hand-derive anything it reports; never
  finish with it failing.
- **Community translations are owned by their contributors.** Update
  stale/missing keys in an existing language when asked, but do not rewrite a
  contributor's phrasing wholesale without the user's explicit direction.
- **Machine-assisted output is a first pass.** PRs and commits containing
  generated translations must say so and invite native-speaker review.
- **Keep the public roster current.** `CONTRIBUTING.md`'s localization table
  (Planned / Machine-assisted / Native, plus credit) must be updated in the
  same commit whenever a language is added or a native review lands. The
  target roster lives there — consult it before proposing new languages.
  Today it lists English (Source) only, so there is nothing yet to
  reconcile, but the rule stands from the first added language onward.
- **Do not bulk-generate.** Translating every language in one pass burns a
  large amount of tokens for content nobody has asked to read yet. Generate
  one language at a time, on request, and prefer the update workflow over
  regenerating a whole file when only a few keys changed.

## File map and conventions

- **This mod ships no Keyed strings at all** — there is no settings window,
  no mod-configuration UI, and no other player-facing prose outside the Defs
  themselves. Unlike TSX (whose only translatable surface was a Keyed
  settings file), this mod's entire translatable surface is **DefInjected**,
  sourced from the Defs it ships:
  - `1.6/Defs/ThingDefs/ThingDefs_BionicThumb.xml` — `BTG_BionicThumb`
    (`label`, `description`)
  - `1.6/Defs/HediffDefs/Hediffs_BionicThumb.xml` — `BTG_BionicThumb`
    (`label`, `labelNoun`, `description`)
  - `1.6/Defs/RecipeDefs/RecipeDefs_BionicThumb.xml` — `BTG_InstallBionicThumb`
    (`label`, `description`, `jobString`)
  - `1.6/Defs/InteractionDefs/InteractionDefs_ThumbsUp.xml` — `BTG_ThumbsUp`
    (`label`, and the `logRulesInitiator/rulesStrings` battle/interaction-log
    lines — these are full grammar-resolved strings with `[INITIATOR_nameDef]`
    / `[RECIPIENT_nameDef]` symbols, not plain Keyed prose; see "Interaction
    log grammar" below)
  - `1.6/Defs/ThoughtDefs/ThoughtDefs_ThumbsUp.xml` — `BTG_ThumbsUp` and
    `BTG_ThumbsUpMood` (each `<stages><li>`'s `label` and, for the mood
    thought, `description`)
- Target layout: `1.6/Languages/<Language>/DefInjected/<DefTypeFolder>/*.xml`.
  **No `Keyed/` folder is needed for any language, including English** —
  don't create one speculatively; add it only if this mod ever grows a
  settings window or other free-standing prose.
- `<DefTypeFolder>` must be the def's resolvable type name: bare, since every
  def type this mod uses (`ThingDef`, `HediffDef`, `RecipeDef`,
  `InteractionDef`, `ThoughtDef`) is a vanilla type — this mod defines no C#
  Def subclass of its own, so no namespace-qualified folder is ever needed
  here (contrast TSX's note, preserved because it's the general rule: a
  namespace-qualified folder would only apply to a mod-defined Def
  *subclass*, which none of this mod's five def types are).
- **The type folder is load-bearing, not organizational** (decompile-verified,
  `Verse.LoadedLanguage`): RimWorld enumerates only the top-level directories
  under `DefInjected/` and resolves each directory *name* to the def type its
  files target. An `.xml` placed directly in `DefInjected/` is never loaded,
  and the checker likewise iterates only directories — a misplaced file fails
  silently on both sides, so never flatten the tree. *Inside* a type folder
  everything is free: file names are arbitrary and files are found
  recursively, so one bundled file per type vs one-def-per-file is pure
  preference. (The loader even tolerates a pluralized folder name by
  retrying with the last character stripped — `ThingDefs` → `ThingDef` — but
  the checker does not; use exact type names.)
- DefInjected keys are `DefName.field` paths, e.g. `BTG_BionicThumb.label`,
  `BTG_InstallBionicThumb.jobString`, or `BTG_ThumbsUpMood.stages.2.label`
  for a stage list element. The checker's `Scripts/expected-injections.json`
  sidecar is the authority for the exact legal set — never hand-derive it
  from reading the def XML, since inherited/C#-default fields (see next
  bullet) don't show up there.
- **Some translatable fields can exist without ever appearing in this
  repo's own XML** — inherited labels from `ParentName="..."` bases
  (`BodyPartBionicBase`, `AddedBodyPartBase`,
  `SurgeryInstallBodyPartArtificialBase`), or a C#-default string this mod's
  own `InteractionWorker_ThumbsUp` never overrides. The authority for what
  actually needs translating is never a hand-maintained list, it's
  `Scripts/expected-injections.json`, a dump of every injection point the
  live game sees, regenerated by `Scripts/refresh-translation-
  expectations.py` (launches the game with the `../L10nProbe` dev mod). The
  checker enforces the sidecar's `required` subset per language and fails on
  stale expectations, so new content of *any* shape forces a regen rather
  than a manifest edit.
- **`1.6/Defs/` is NOT the translation surface — it is a strict subset of
  it, and this mod has no English DefInjected tree at all to fall back on
  either.** Enumerate from the sidecar's `required` entries per def type,
  not from scanning `1.6/Defs/` or hand-listing defNames, and take the
  English source text for each entry from the sidecar's `english` field
  (which is also what the checker compares `<!-- EN: -->` comments
  against, so sourcing EN comments from it programmatically makes drift
  impossible). The def XML view is a useful map of *where* the mod's own
  content lives, but the sidecar is the only complete and authoritative
  list of *what* needs a translation.
- **This mod requires no DLC** (`About.xml` has no `modDependencies` beyond
  Harmony) and gates no content behind `MayRequire`, so it ships no
  `1.6/Mods/<Name>/` compat load root today (contrast the sibling mods,
  which route MayRequire-gated content there) — `REQUIRED_DLCS` in the
  checker is empty. Ground every term against **Core only**; Biotech,
  Royalty, Odyssey and Anomaly vocabulary is out of scope unless a specific
  vanilla phrase is being reused verbatim as a style reference for a
  concept Core doesn't otherwise cover (see the trader-price rows below,
  reused from Odyssey — flagged per-row where that happens).
- **EN comment convention (required):** every translated entry carries the
  current English source directly above it, e.g.
  `<!-- EN: install bionic thumb -->` — this is how the checker detects
  staleness.
- Formatting: UTF-8 without BOM, LF endings, 2-space indent, final newline,
  root element `<LanguageData>`.
- Placeholders (`{0}`, `{1}`, named args) must match English exactly per key.
  This mod's own def XML has none today (no def field uses a `{0}`-style
  arg), but the interaction log `rulesStrings` use bracketed symbols
  (`[INITIATOR_nameDef]`, `[RECIPIENT_nameDef]`) — RimWorld's grammar
  resolver, not the `{0}` argument mechanism the checker's placeholder regex
  targets. Don't conflate the two: symbols resolve through
  `GrammarResolver`/`GrammarResolverSimple` per the cross-language lessons
  below, and are not what `PLACEHOLDER_RE` in the checker is checking.

### Interaction log grammar (new relative to TSX — read this before translating `InteractionDefs_ThumbsUp.xml`)

TSX's translate skill could set aside every RulePackDef-specific lesson its
weapon-mod siblings learned, because TSX shipped no RulePackDefs and no
grammar-resolved text of its own. **This mod is different**: `BTG_ThumbsUp`'s
`logRulesInitiator/rulesStrings` are full battle/interaction-log lines routed
through RimWorld's grammar resolver with `[INITIATOR_nameDef]` and
`[RECIPIENT_nameDef]` symbols — the same resolver family the weapon mods'
combat logs use, not the simple `GrammarResolverSimple` a plain
`"key".Translate()` Keyed string reaches. Consequences:

- Everything the per-language sections below say about symbol quoting,
  gender/possessive symbols, Korean josa, and article/contraction handling
  applies directly to these seven `rulesStrings` lines, not just to some
  hypothetical future RulePackDef.
- `[INITIATOR_nameDef]` / `[RECIPIENT_nameDef]` resolve to a pawn's def-style
  name and never inflect for case; wrap them in the target language's quote
  convention exactly as the cross-language lessons describe for an injected
  def label.
- These lines are flavour text for a low-weight (`0.01`) social interaction
  gated on `BTG_BionicThumb` — translate for tone (wry, a little
  brand-obsessed, matching the English "the servo whirred" register) rather
  than literally; a native reviewer should feel free to rewrite the joke
  rather than transliterate it.
- The `ThoughtDef` stage labels/descriptions this interaction produces
  (`BTG_ThumbsUp`'s social opinion stage, `BTG_ThumbsUpMood`'s five escalating
  mood stages) are plain DefInjected strings, not grammar-resolved — normal
  rules apply there.

## Terminology grounding (do not skip)

Every game term must match the official localization, not a plausible
translation. Sources, in order:

1. Vanilla language data:
   `"$RIMWORLD_PATH"/Data/Core/Languages/<Language> (<Native>).tar`
   (read entries with `tar -xOf`). **Core only** — this mod requires no DLC,
   so there is no Biotech/Royalty/Odyssey/Anomaly tar to check for its own
   vocabulary. The one deliberate exception is the trader-price phrasing
   rows below, reused verbatim from Odyssey's `GoldInlay`/`Ugly` stat
   descriptions as a style reference for a concept Core states more
   generically — note it as DLC-sourced style guidance if grounding it
   further, since a player without Odyssey will still see this mod's own
   Core-only trader-stock feature.
2. This file's glossary below (lessons already learned — apply them).
3. If a term appears nowhere official, flag it in the PR for native review
   rather than inventing silently.

Terms that MUST be grounded before use: bionic / artificial body part
vocabulary (Core's own `Bionic arm`, `Bionic leg`, `Bionic eye` HediffDefs
and their shared `AddedBodyPartBase` phrasing), surgery/installation
recipe vocabulary (`RecipeDef.jobString`, "Install/installing X" phrasing
from Core's existing bionic recipes), manipulation/part-efficiency stat
language, social-interaction log conventions (Core's existing
`InteractionDef`s like `Chitchat`, `DeepTalk`, `Insult` — tone and grammar
patterns, not vocabulary this mod needs to reuse), opinion/mood thought
vocabulary (Core's `ThoughtDef` stage phrasing for social memories vs.
personal mood memories), quality tiers, and orbital-trader / market-value
vocabulary. **None of this repo's own glossary rows below have been
grounded yet** — no language pass has run in this repo. Treat every
glossary table below as **style/mechanics reference only** until an actual
generation pass grounds bionic/surgery/interaction/thought vocabulary
against the Core tar and records it here.

### Glossary — shared across the mod family

The style rules, worker mechanics and cross-language lessons below were
learned across the weapon-mod siblings (`../UniqueMeleeWeapons`,
`../UniqueWeaponsUnbound`, `../PersonaWeaponsUnbound`) and refined again by
`../TradersStockXenogerms` (TSX), and this repo now joins that family.
Everything about *how a language's `LanguageWorker` behaves* — quoting
conventions, punctuation, formality, dash/ellipsis rules, Korean josa
markers, German case vs. gender, French elision, Spanish/Portuguese
contraction — is mechanical fact about RimWorld's translation engine,
independent of whether a mod is about weapons, xenogerms, or bionic thumbs,
and is reproduced below unchanged. What does **not** carry over verbatim is
the glossary *tables*: TSX kept only the rows relevant to a Keyed
settings-window/trader-price mod and dropped the weapon-mod siblings' melee
vocabulary. This mod keeps TSX's surviving rows (quality tiers, trader-price
phrasing — this mod also stocks orbital traders) and adds nothing new yet,
since no generation pass has run here. It also, unusually for this family,
has a genuine use for the **interaction-log/grammar-resolution** lessons
TSX explicitly set aside (see "Interaction log grammar" above) — those
lessons are reproduced below unedited from the weapon-mod combat-log
research, because this mod's `rulesStrings` reach the same resolver. The
Cancel/Reset button rows TSX kept for its settings window are left in place
below too, on the chance this mod ever grows one (see TODO.md), but are not
currently exercised by anything this mod ships. Mirror a correction the
other direction too: if generating this mod's languages surfaces a fix to a
truly *shared* row (a button label, a punctuation rule, a resolver finding),
propagate it back into the siblings.

#### Russian (from UWU PR #6 native review)

| English | Use | Never | Why |
|---|---|---|---|
| Cancel (button) | Отменить | Отмена | vanilla `Cancel`; buttons use infinitive verbs |
| report/inspect strings | noun phrases | finite verbs | matches inspect-pane convention |

The weapon-domain rows (weapon `trait`, gun `charge`) and mod-decided
WeaponCategoryDef labels are irrelevant here, as they were for TSX. This
repo has not yet run a Russian generation pass; add bionic/surgery/
interaction rows here once one lands.

#### Japanese (from the weapon-mod siblings' 2026-07 generation)

RimWorld's language folder is `Japanese` (tar: `Japanese (日本語).tar`).

Style rules discovered from the vanilla JP data (mandatory):

- Vanilla JP uses ASCII punctuation: `,` and `.` — never `、` or `。`.
- Descriptions/tooltips: polite です/ます form ending `.`; labels/buttons take
  no period.
- Quote injected def labels and cross-referenced UI labels with 「」. Suffixes
  and parentheticals take no leading space and use ASCII parens. This
  applies directly to `[INITIATOR_nameDef]`/`[RECIPIENT_nameDef]` in
  `BTG_ThumbsUp`'s `rulesStrings` — wrap each in 「」.
- DLC names stay in Latin script (Biotech, Royalty, Odyssey), as does MOD —
  moot for this mod's own content since it requires none, but still applies
  if a translated string ever needs to name a DLC generically.

| English | Use | Never | Why |
|---|---|---|---|
| Cancel / Reset / Reset to defaults | キャンセル / リセット / デフォルトに戻す | | vanilla Keyed buttons |
| quality tiers | 壊れかけ/低品質/標準品/良品/秀品/名品/幻の一品 | | Core `QualityCategory_*` |
| Traders will pay more/less for it. | 貿易商は高値で/低い価格でこれを買い取ります. | | Odyssey `GoldInlay`/`Ugly` descs — reuse verbatim; directly relevant to this mod's own orbital-trader stock feature |

The rest of the weapon-mod Japanese glossary — weapon/tool/damage
vocabulary, the attributive-form (`の`/`な`-terminated) requirement for
`traitAdjectives`, and the `[stuff_adjective]の[noun]` name-grammar
composition — is specific to `RulePackDef` name generation, which this mod
has none of (it has grammar-resolved interaction-log text, not name
generation). See `../UniqueMeleeWeapons` if that ever changes. This repo has
not yet run a Japanese generation pass; add bionic/surgery/interaction rows
here once one lands.

#### Simplified Chinese (from the weapon-mod siblings' 2026-07 generation)

RimWorld's language folder is `ChineseSimplified` (tar: `ChineseSimplified
(简体中文).tar`) — the mod's folder must match it exactly, whatever the
public roster calls the language.

Style rules discovered from the vanilla zh data (mandatory):

- Full-width punctuation in prose (，。、；：（）……); descriptions end with 。;
  labels and buttons carry no trailing period. Placeholders, digits and units
  stay ASCII. Vanilla labels use full-width parens: 锻造台（燃料）.
- Quote cited names in prose with full-width curly quotes — vanilla writes
  任务"{0}". Terse stat templates take no quotes ({0}伤害). Quote
  `[INITIATOR_nameDef]`/`[RECIPIENT_nameDef]` in `BTG_ThumbsUp`'s
  `rulesStrings` the same way.
- Vanilla zh files can contain untranslated English values — vanilla
  incompleteness is not style guidance. Some vanilla zh files carry a BOM;
  ours never do.

| English | Use | Never | Why |
|---|---|---|---|
| quality tiers | 极差/较差/一般/良好/极佳/大师级/传奇级 | | Core `QualityCategory_*` |

The rest of the weapon-mod Simplified Chinese glossary — weapon/tool/damage
vocabulary and the name-grammar composition rules (的/之 linking, material
compounding) — is specific to `RulePackDef` name generation, which this mod
has none of. See `../UniqueMeleeWeapons` if that ever changes. This repo
has not yet run a Simplified Chinese generation pass; add bionic/surgery/
interaction rows here once one lands.

#### Korean (from the weapon-mod siblings' 2026-07 generation)

Language folder is `Korean` (tar: `Korean (한국어).tar`). Decompile-verified
why the paren-stripped name works: `LoadedLanguage` derives
`legacyFolderName` by cutting at `(`, and mod language dirs match on
*either* `folderName` or `legacyFolderName` — the same mechanism behind
`Japanese`.

**Josa (particle) markers are the one hard mechanical rule Korean adds, and
nothing else in this skill has an equivalent — and it applies to any Keyed
or grammar-resolved string, not just combat/rulepack text.** Korean
particles are allomorphic: the correct form depends on whether the previous
syllable ends in a consonant, which is unknowable when the preceding text is
an injected value. For this mod, that means every occurrence of
`[INITIATOR_nameDef]` or `[RECIPIENT_nameDef]` in `BTG_ThumbsUp`'s
`rulesStrings` followed by an allomorphic particle needs a marker — this is
not a hypothetical, it's the actual translation surface this mod ships.
`Verse.LanguageWorker_Korean.ReplaceJosa` (decompile-verified) resolves
exactly eight tokens, and no others:

```
(이)가   (와)과   (을)를   (은)는   (아)야   (이)어   (으)로   (이)
```

- Every *allomorphic* particle following `[INITIATOR_nameDef]`,
  `[RECIPIENT_nameDef]`, `{0}` or `[TOKEN_x]` MUST use a marker.
  `[RECIPIENT_nameDef](을)를` is correct; `[RECIPIENT_nameDef]를` breaks on
  consonant-final names. Only five distinctions inflect (은/는, 이/가, 을/를,
  와/과, 으로/로); **`에`, `에서` and `의` are invariant** — write those bare
  after a placeholder.
- Never hand-roll `{0}을(를)` — the worker does not recognize it.
- **Spelling is exact, and `(와)과` is asymmetric.** For every token the paren
  holds the post-*consonant* form — except `(와)과`, where `JosaPatternPaired`
  maps to `("과","와")`, so the paren holds the post-*vowel* form.
- **A marker resolving off a digit is always wrong.** `HasJong()` falls back to
  `AlphabetEndPattern` = `{b,c,k,l,m,n,p,q,t}` for non-Korean chars, which has no
  digits, so a number always yields the vowel form — right for 2/4/5/9
  (이·사·오·구), wrong for 1(일) 3(삼) 6(육) 7(칠) 8(팔) 0(영). This mod's own
  `BTG_ThumbsUpMood` stages are hardcoded English ordinal words ("two thumbs
  up", "three thumbs up", ...), not runtime-injected numbers, so this
  specific digit trap doesn't bite there — but phrase around it defensively
  anyway if a future feature ever injects a count.
- **Quoting interacts with resolution.** `FindLastChar` skips a preceding `"`,
  `'` or `)` to reach the real final character, so `"[X]"(을)를` resolves
  correctly. Curly `" "` and corner `「 」` are **not** skipped, so the token
  is returned unresolved and the raw `(은)는` shows on screen. Korean
  therefore needs no defensive quoting at all around
  `[INITIATOR_nameDef]`/`[RECIPIENT_nameDef]` — josa does the job quoting
  does in ja/ru/zh.
- The one safe unmarked case: a symbol that always resolves the same way (a
  fixed pronoun). Pawn names and any mod-coined term are never safe.
- A lint for this lives outside the repo checker (which is language-agnostic).

Other style rules discovered from the vanilla ko data (mandatory):

- ASCII punctuation (`.` `,`), never `。`. Descriptions/tooltips take polite
  formal `-습니다.`/`-입니다.`; labels, buttons and stat fragments take no
  trailing period.
- Korean **uses spaces**, unlike JP/zh.
- Units attach with no space: `{0}시간`, `{0}일`, `{0}칸`. Some vanilla ko
  files carry a BOM; ours never do.

| English | Use | Never | Why |
|---|---|---|---|
| Cancel / Reset / Reset all | 취소 / 초기화 / 모두 초기화 | | Core Keyed |
| quality tiers | 끔찍/빈약/평범/상급/완벽/걸작/전설적 | | Core `QualityCategory_*` |
| Traders will pay more/less for it. | 상인들이 더 높은 값을 쳐줍니다. / 상인들은 더 적은 돈을 쳐줍니다. | | Odyssey `GoldInlay`/`Ugly` — reuse verbatim; directly relevant to this mod's own trader-stock feature |

**Cross-checked against PWU's own ko pass, landed the same day, independently
grounded** — worth keeping as a caution even though the specific rows are
weapon-domain: two rows genuinely diverged between sibling mods on the same
term (`mechanite`, `armor penetration`) because each was grounded against a
different tar subset. **Ground this mod's own bionic/surgery terms
independently against the Core tar rather than assuming a sibling mod's
word for an adjacent concept transfers** — a weapon mod's word for
"prosthetic" or "installed" in a combat context may not match Core's own
bionic-recipe phrasing.

The rest of the weapon-mod Korean glossary — weapon/tool/damage vocabulary
and the extensive mod-decided trait-adjective list — is specific to melee
combat text, which this mod has none of. See `../UniqueMeleeWeapons` if that
ever changes. This repo has not yet run a Korean generation pass; add
bionic/surgery/interaction rows here once one lands.

#### German (preseeded from PersonaWeaponsUnbound's 2026-07-28 generation,
extended across the weapon-mod siblings 2026-07-28)

Language folder is `German` (tar: `German (Deutsch).tar`).

Style rules from the vanilla de data (mandatory, applies to any Keyed or
grammar-resolved string regardless of mod domain):

- **ASCII single quotes** for cited def labels and UI labels — vanilla writes
  `Forschungsprojekt '{0}'`. Core+Royalty Keyed ship 140 single-quoted
  placeholders and **zero** German `„…"`. Never use `„ "`, `» «`, or curly
  quotes. Pawn names are not quoted — this directly governs
  `[INITIATOR_nameDef]`/`[RECIPIENT_nameDef]` in `BTG_ThumbsUp`'s
  `rulesStrings`: leave them bare.
- **En dash `–`, never em dash `—`** (20 vs 0). English source uses `—`, so
  every dash needs converting; `<!-- EN: -->` comments keep the English form
  verbatim.
- Ellipsis is ASCII `...` (74 in Core Keyed, `…` zero).
- Descriptions end with `.`; labels and buttons take none. Player-facing
  prose is informal **du** with imperatives, never Sie.

**Case is the German landmine, not gender** (decompile-verified:
`Verse.GrammarResolverSimple`, `LanguageWorker_German`, `LanguageWordInfo`).
`"key".Translate(args)` reaches `GrammarResolverSimple`; the full
`GrammarResolver` that `BTG_ThumbsUp`'s `rulesStrings` reach supports more —
but neither implements a `lookup` function, so `{lookup: {0}; decline; N}`
— the only route to the 2457-row `decline.txt` case forms — is unavailable
either way, and de's article helpers are nominative-only. Gender is
solvable, case is not: restructure any oblique slot (a sentence needing a
dative/accusative/genitive form of `[INITIATOR_nameDef]` or
`[RECIPIENT_nameDef]`) rather than guessing an article. A gender lookup that
misses **defaults to masculine** (`ResolveGender`'s `defaultGender`) — safe
only for vanilla nouns in nominative slots, never for a pawn name or any
mod-coined label absent from the Gender tables.

`PostProcessed` also rewrites a trailing English `'s` to `s` (or a bare `'`
after s/ß/z/x/ce) — a closing ASCII single quote immediately followed by
lowercase `s` is silently mangled, so never write `'{0}'s` (or
`'[RECIPIENT_nameDef]'s`) in German prose.

| English | Use | Never | Why |
|---|---|---|---|
| Cancel / Reset / Confirm / Randomize | Abbrechen / Zurücksetzen / Bestätigen / Zufällig | | Core buttons |
| Reset to defaults / default | Auf Standard zurücksetzen / Standard | | Core `ResetBinding`, `Default` |
| None | Nichts | Keine | Core `None` |
| quality / tiers | Qualität / übel·schlecht·normal·gut·exzellent·meisterlich·legendär | | Core `Quality`, `QualityCategory_*` |
| "{0} quality or better" | `Qualität {0} oder besser` | | reshaped from Core `NormalQualityOrBetter` (pre-inflected, untemplatable) |

The rest of the weapon-mod German glossary — weapon/tool/damage vocabulary,
the `namerLabels`/`traitAdjectives` `|M|`/`|F|`/`|N|` gender-marker scheme
for `RulePackDef`s, and the relic-name truncation rule — is specific to name
generation, which this mod has none of (its own grammar-resolved text is
interaction-log flavour, not name generation). The "never *print* a
`[X_definite]'s` genitive" battle-log lesson, however, generalizes directly
to this mod's `rulesStrings`. See `../UniqueMeleeWeapons` or
`../PersonaWeaponsUnbound` for the RulePackDef-specific material if this mod
ever gains one. This repo has not yet run a German generation pass; add
bionic/surgery/interaction rows here once one lands.

#### Spanish (Castellano) (from the weapon-mod siblings' 2026-07-29 generation)

RimWorld ships **two** Spanish languages: `Spanish (Español(Castellano)).tar` and
`SpanishLatin (Español(Latinoamérica)).tar`. The roster's "Spanish" means the
Castilian one, so the mod folder is `Spanish` (the parenthetical is stripped by
`legacyFolderName`, same mechanism as `Japanese`/`Korean`). A LatAm pass would be a
separate `SpanishLatin` folder, not an edit to this one.

`Verse.LanguageWorker_Spanish` is decompiled and **imposes no hidden
authoring requirements** — no `PostProcessed` override (unlike German), no
particle system (unlike Korean). It prepends `el/la/los/las` and
`un/una/unos/unas` from the word's gender, returns names unchanged, has
full `Pluralize` rules plus a `plural.txt` lookup, and renders ordinals
`N.º`. Notably it does **not** contract `de el`/`a el` — that is the
author's job (see below).

Style rules from the vanilla es data (mandatory):

- **ASCII straight double quotes** for cited def labels: vanilla writes
  `La misión se llama "{0}".` — 7689 ASCII `"` against **7** curly `“` and
  **zero** guillemets `«»`. Names (including `[INITIATOR_nameDef]` /
  `[RECIPIENT_nameDef]` in `BTG_ThumbsUp`'s `rulesStrings`) return unchanged
  and unquoted, per the worker note above.
- **Inverted opening marks are required**: `¿…?`, `¡…!` (168 / 433 in Core).
- **Zero dashes.** Core+DLC contain **no** em dashes and **no** en dashes, so
  an English `—` must be **reflowed**, not converted. This is the opposite
  of German, which mandates `–`.
- Ellipsis is ASCII `...`. Descriptions end `.`; labels, buttons and stat
  fragments take none, and labels are lowercase noun phrases.
- **Informal tú with imperatives**, decisively: Explora 12 / Explore 0,
  Asegúrate 41 / Asegúrese 0, `tu colonia` 61 / `su colonia` 3.

**`de el` → `del` and `a el` → `al` must be contracted by hand** whenever a
sentence places `de`/`a` directly before an injected `[X_definite]` symbol
(available even in a plain `.Translate()` call — see the German note above
on `GrammarResolverSimple`). Core es fixes this 89 times with the colour
code baked into the search pattern:

```
{replace: de [RECIPIENT_definite]; "de &lt;color=#D09B61FF>el "-"&lt;color=#D09B61FF>del "}
{replace: a [RECIPIENT_definite]; "a &lt;color=#D09B61FF>el "-"&lt;color=#D09B61FF>al "}
```

Feminine (`de la pirata`) and named entities simply don't match and pass
through untouched, which is correct — this is the case for
`[RECIPIENT_nameDef]` in `BTG_ThumbsUp`, a bare name symbol, not a
`_definite` article symbol, so this specific contraction trap doesn't apply
to this mod's own rulesStrings unless a future revision adds a `_definite`
slot. **Core es also ships a shorter, buggy variant**
(`{replace: de [X]; ">el "-">del "}`, 20 uses in `RulePacks_CombatRanged`)
that leaves the literal `de ` outside the match and renders "de del
proyectil" — copy the full form only, or restructure so no `de`/`a`
precedes a `_definite` symbol.

**`[RECIPIENT_possessive]` resolves to `su` and has NO plural form** — Core
`Keyed/Grammar.xml` sets `Prohis`/`Proher`/`Proits` all to `su`. Since
Spanish `su` agrees in number with the *possessed* noun, the symbol is only
safe before a **singular** noun. Use the definite article for plurals
instead.

| English | Use | Never | Why |
|---|---|---|---|
| Cancel / Reset / Confirm / Default / None | `Cancelar` / `Restablecer` / `Confirmar` / `Por defecto` / `Ninguno` | | Core buttons |
| quality tiers | `horrible·mediocre·normal·bueno·excelente·obra maestra·legendaria` | | Core `QualityCategory_*` |
| Traders will pay more/less for it. | `Los comerciantes pagarán más por ella.` / `… menos por ella.` | | Odyssey `GoldInlay`/`Ugly` — reuse verbatim; directly relevant to this mod's own trader-stock feature |

The rest of the weapon-mod Spanish glossary — weapon/tool/damage
vocabulary, the `badass_concept`/`conceptF` parallel-symbol-family
technique for `RulePackDef` gender, and quest-site vocabulary — is specific
to name generation, which this mod has none of. See `../UniqueMeleeWeapons`
if that ever changes. This repo has not yet run a Spanish generation pass;
add bionic/surgery/interaction rows here once one lands.

#### French (from the weapon-mod siblings' 2026-07-29 generation)

Language folder is `French` (tar: `French (Français).tar`).

**`LanguageWorker_French` rewrites every string, and this is the finding
that shapes everything else** (decompile-verified) — including plain
`.Translate()` Keyed strings and grammar-resolved text alike, so it applies
directly to `BTG_ThumbsUp`'s `rulesStrings`. Its `PostProcessed` runs five
regexes in order:

```
ElisionE   \b(ce|de|je|le|me|ne|se|te|que|quoique|lorsque) + vowel   → c' d' j' l' m' n' s' t' qu' ...
ElisionLa  \bla + vowel                                             → l'
ElisionSi  \bsi il(s)                                               → s'il(s)
DeLe       \bde le(s)                                               → de / des
ALe        \bà le(s)                                                → au / aux
```

**So French is the inverse of Spanish: never hand-contract.** Write `de` /
`le` / `la` plainly and the worker fixes it. Two traps in it:

- **`de le` becomes `de`, not `du`.** Group 2 captures only `e`/`es`, so
  `de les X` correctly yields "des X" but `de le X` yields "de X" — a
  vanilla bug, not guidance to imitate; restructure so the entity is a
  subject, or use an agent phrase — **`par [X_definite]` never contracts**
  and is the clean escape.
- **`IsVowel` includes `h`**, so the worker cannot tell *h muet* from
  *h aspiré* and elides both. Never place an elidable word directly before
  an h-initial noun without checking which kind it is.

`WithDefiniteArticle`/`WithIndefiniteArticle` are **overridden**, handling
`l'` before a vowel and `le`/`la` by gender directly — so `[X_definite]` is
reliable in French even in a plain Keyed string. `Pluralize` knows
`-al`→`-aux`, `-au`/`-eu`→`+x`, and leaves `s`/`x`/`z` alone.

Style rules from the vanilla fr data (mandatory):

- **Formality is `vous`, decisively** — 564 `vous` against **zero**
  `tu`/`Tu` in Core+DLC Keyed. This is the opposite of German and Spanish,
  both informal. Imperatives are the vous form (`Explorez`, `Faites
  attention`).
- **ASCII straight double quotes** for cited def labels — 356 ASCII `"`
  against 14 guillemets `«»` (inconsistently spaced) and **zero** curly `“`.
- **ASCII apostrophe `'`**, not `’` (1991 vs 65) — load-bearing, not
  cosmetic: the elision worker emits ASCII `'`, so a curly one would not
  match.
- **A space before `:` `;` `!` `?`**, per French typography — a **plain
  ASCII space**, not a no-break or narrow space.
- **Zero dashes.** An English `—` must be **reflowed**, as in Spanish and
  unlike German, which mandates `–`. Ellipsis is ASCII `...`.
- Descriptions end `.`; labels, buttons and stat fragments take none, and
  labels are lowercase noun phrases.

**`[X_possessive]` is structurally wrong in French.** Core
`Keyed/Grammar.xml` sets `Prohis`=`son`, `Proher`=`sa`, `Proits`=`son/sa` —
resolved from the **possessor's** gender — but French `son`/`sa` must agree
with the **possessed** noun. The symbol therefore keys off the wrong entity
no matter what; write the possessive literally instead (Core's own
`[RECIPIENT_possessive]de son travail` renders the broken "sonde son
travail", which is vanilla's own evidence not to use it).

| English | Use | Never | Why |
|---|---|---|---|
| quest / mod UI: Cancel / Reset / Reset to defaults / Default / None | `Annuler` / `Réinitialiser` / `Réinitialiser les valeurs par défaut` / `Par défaut` / `Aucune` | | Core buttons |
| quality tiers | `horrible·médiocre·normal·bon·excellent·merveille·légendaire` | | Core `QualityCategory_*` |
| Traders will pay more/less for it. | `Les commerçants en paieront un prix plus élevé.` / `Les commerçants en paieront moins cher.` | | Odyssey `GoldInlay`/`Ugly` — reuse verbatim; directly relevant to this mod's own trader-stock feature |

The rest of the weapon-mod French glossary — weapon/tool/damage vocabulary,
the rule-level gender constraint technique for `RulePackDef`s
(`staggered(SUBJECT_gender==Female)->…`) and `traitAdjectives`/`namerLabels`
shape rules — is specific to name generation, which this mod has none of.
See `../UniqueMeleeWeapons` if that ever changes. This repo has not yet run
a French generation pass; add bionic/surgery/interaction rows here once one
lands.

#### Brazilian Portuguese (from the weapon-mod siblings' 2026-07-29 generation)

Language folder is **`PortugueseBrazilian`** (tar: `PortugueseBrazilian
(Português Brasileiro).tar`). RimWorld ships European `Portuguese` as a
*separate* language; a pt-PT pass would be its own folder.
`LanguageInfo.xml` declares `languageWorkerClass`
**`LanguageWorker_Portuguese`** — the two languages share one worker.

**The worker does almost nothing, and that is the finding that shapes
everything else** (decompile-verified). It overrides **only**
`WithIndefiniteArticle` and `WithDefiniteArticle` (prepending `o `/`a `/`os
`/`as `, `um `/`uma `/`uns `/`umas ` by gender). It has **no
`PostProcessed` override**, so the base `LanguageWorker.PostProcessed`
runs — and that only calls `MergeMultipleSpaces()`. No elision, no
contraction, no `'s` rewriting, no particles.

**So Portuguese is the hard case: its contractions are orthographically
mandatory and nothing supplies them.** `de`+`o`=`do`, `de`+`a`=`da`,
`em`+`o`=`no`, `em`+`a`=`na`, `a`+`o`=`ao`, `a`+`a`=`à`, `por`+`o`=`pelo`
(plus every plural). Consequences, relevant to any translated prose that
injects a definite-article'd label, not only rulepacks:

- **Never write `de` / `em` / `a` / `por` directly before a `[X_definite]`
  symbol.** `_definite` prepends a bare `o `, nothing fuses it, and the
  literal **"de o pirata"** ships — and **vanilla pt-BR ships exactly this
  bug** in its own combat packs. Frequency is not correctness. This mod's
  own `[INITIATOR_nameDef]`/`[RECIPIENT_nameDef]` are bare name symbols, not
  `_definite` articles, so this specific trap doesn't bite `BTG_ThumbsUp`
  today unless a future revision adds a `_definite` slot.
- **The clean escapes are `com`, `para`, `contra`, `sem`, `sobre`,
  `entre`** — none contract with the article. Otherwise restructure so the
  entity is a subject.
- **The idiomatic vanilla technique is to use the bare `[X_label]` and
  write the contracted article yourself, hedged**: Core's ranged pack
  writes `do(a) [INITIATOR_label]`.
- There are **zero `{replace:}` blocks** anywhere in pt-BR's rulepacks —
  don't invent one; restructure instead.

Style rules from the vanilla pt-BR data (mandatory):

- **ASCII straight double quotes**, **zero em/en dashes** (reflow an
  English `—`, as in es and fr — the opposite of de), ASCII ellipsis `...`
  and apostrophe `'`.
- **No space before `:` `;` `!` `?`** — the exact opposite of French, and
  the two languages are otherwise close enough that this is an easy
  cross-contamination.
- No `¿`/`¡` — that is Spanish only.
- **Formality is `você`, decisively** — imperatives take the você form
  (`Clique`, `Selecione`, `Escolha`, `Certifique-se`, `Faça`).
- Descriptions end `.`; labels, buttons and stat fragments take none, and
  labels are lowercase.

**Gender hedging is a distinct technique from every other language here,
and pt-BR applies it to the surface text itself**, pervasively — articles,
participles, contractions and possessives alike get a literal **`(a)`**:
`O(a)`, `um(a)`, `do(a)`, `pelo(a)`. A `.Translate()` / templated string
instead takes the inline resolver split (`{PAWN_gender ? o : a}`); which
shape applies depends on whether the string is plain prose (literal `(a)`)
or a resolver-fed template (inline split) — check the field, not a blanket
rule. `BTG_ThumbsUp`'s `rulesStrings` are grammar-resolved templates, so a
gender-dependent word there takes the inline split, not the literal `(a)`
hedge.

**`[X_possessive]` is unusable here too, for a different reason than
French.** Core `Keyed/Grammar.xml` sets `Prohis`=`o`, `Proher`=`a`,
`Proits`=`o(a)` — a bare **definite article**, not a possessive pronoun,
keyed off the **possessor's** gender while Portuguese must agree with the
**possessed** noun. Write the possessive literally, as French does, though
for a distinct underlying reason — check `Keyed/Grammar.xml`'s actual
values rather than assuming the symbol inflects.

| English | Use | Never | Why |
|---|---|---|---|
| Cancel / Reset / Reset to defaults / Default / None / Confirm | `Cancelar` / `Redefinir` / `Restaurar padrão` / `Padrão` / `Nenhum` / `Aceitar` | `Confirmar` | Core buttons. `Confirm`=`Aceitar`, `ResetBinding`=`Restaurar padrão` |
| quality tiers | `horrível·pobre·normal·bom·excelente·obra-prima·lendário` | `ruim` for poor | Core `QualityCategory_*` |
| Traders will pay more/less for it. | `Comerciantes pagarão mais por ela.` / `Comerciantes pagarão menos por ela.` | | Odyssey `GoldInlay`/`Ugly` — reuse verbatim; directly relevant to this mod's own trader-stock feature |

The rest of the weapon-mod pt-BR glossary — weapon/tool/damage vocabulary,
the preposed-namer constraint that forces gender-invariant
`traitAdjectives`, and the curated `Strings/Words/Nouns/Weapons.txt` corpus
— is specific to name generation, which this mod has none of. See
`../UniqueMeleeWeapons` if that ever changes. This repo has not yet run a
Brazilian Portuguese generation pass; add bionic/surgery/interaction rows
here once one lands.

### Cross-language lessons

- Wrap injected `[INITIATOR_nameDef]`/`[RECIPIENT_nameDef]` (this mod's own
  symbols) or any `{0}`-style def label in the language's quote marks (JP
  「{0}」, RU «{0}», zh-Hans "{0}") — injected labels never inflect, and
  quoting sidesteps case and agreement problems. **Korean is the exception,
  and porting the ja form actively breaks it**: ko solves the same problem
  mechanically with josa markers, and `FindLastChar` looks through only
  ASCII `'` `"` `)` to find the syllable that decides the particle. Curly
  `" "` and corner `「 」` are not skipped, so `「[RECIPIENT_nameDef]」(을)를`
  silently ships an unresolved `(을)를`. Inject bare and mark the particle
  instead.
- **Check whether the worker contracts before writing any contraction
  scaffolding — the answer inverts between languages.** Spanish must fuse
  `de`+`el` by hand; French must do the **opposite and write nothing**,
  because `LanguageWorker_French.PostProcessed` elides and fuses
  automatically, so hand-contracting would double-apply; Portuguese is the
  worst case, where contractions are mandatory and nothing supplies them at
  all — see the German/Spanish/French/pt-BR sections above for the
  specifics. Verify a vanilla pattern actually works before copying it;
  frequency is not correctness (both es and fr ship a demonstrably broken
  contraction in their own combat packs).
- **A "no hidden mechanics" worker is itself a finding, not a reason to
  skip the check.** Spanish's and Portuguese's workers impose few or no
  authoring requirements, but Portuguese's *absence* of a `PostProcessed`
  override is precisely what makes every contraction the author's problem.
  Read what the worker does **not** do as carefully as what it does, and
  note that languages can share one worker class (`PortugueseBrazilian` and
  `Portuguese` both use `LanguageWorker_Portuguese`).
- **The possessive symbol (`[X_possessive]`/`Prohis`/`Proher`/`Proits`) has
  a different correct answer per language, so never generalize one.**
  Korean drops it, German keeps and inflects it inline, Spanish keeps it
  only before a singular noun, French and Portuguese both must write the
  possessive literally, for two different underlying reasons. Check
  `Keyed/Grammar.xml`'s actual values for the target language rather than
  assuming the symbol inflects. This mod's own `rulesStrings` don't use a
  possessive symbol today, but this generalizes if one is ever added.
- **A def field's official label can differ across the def *types* that
  share its name or concept**, and translating from the wrong one is an
  easy, invisible error (es Core's DamageDef `Stab`=`apuñalamiento` vs
  HediffDef `Stab`=`puñalada`, for instance — see the weapon-mod skills for
  the full pattern). This mod patches five different def *types*
  (`ThingDef`, `HediffDef`, `RecipeDef`, `InteractionDef`, `ThoughtDef`) —
  confirm which def *type*'s official label you're grounding against for
  any shared-sounding word ("install" as a recipe `jobString` verb vs. as
  general prose, for instance), not just the term.
- **When two vanilla files disagree, prefer the nearer analog, not the
  more central one.** For this mod that means: if Core's own bionic-recipe
  Keyed data and Core's generic item/trader vocabulary ever disagree on a
  term, the bionic-recipe analog wins — it's the nearer one.
- **Don't spend a vanilla word on the wrong slot.** Map any concept this
  mod needs against vanilla's existing usage of that word *first* (e.g.
  don't reuse Core's word for "installed" in a different bionic-recipe
  sense than this mod's own surgery), and coin only for what's genuinely
  left over.
- **Distinguish comment occurrences from value occurrences when mining the
  tar.** Grepping a symbol across a language's files counts English
  `<!-- EN: -->` text too, which can invert the conclusion about whether a
  symbol is actually used in translated values. Strip comments before
  counting.
- **Check for a `LanguageWorker_<Language>` before generating.** It
  post-processes every string, so it can impose authoring requirements no
  amount of reading the vanilla data will reveal as *mandatory* — Korean's
  josa markers are invisible until you find `ReplaceJosa`. Decompile it:
  `ilspycmd "$RIMWORLD_PATH/RimWorldWin64_Data/Managed/Assembly-CSharp.dll" -t
  "Verse.LanguageWorker_<Language>"`. Languages with heavy inflection
  (Russian, Polish, Turkish, Czech, German) are the ones to check first. **A
  worker can also do work *for* you**, which is just as important to
  know — French's elides and contracts automatically, so the correct
  authoring there is to write the uncontracted form and leave it alone.
- **Simulate the worker rather than reasoning about it.** Its regexes are
  short enough to reimplement in a few lines of Python, and running your
  actual strings — including `BTG_ThumbsUp`'s `rulesStrings` — through them
  catches what eyeballing does not.
- **Know which resolver your strings actually reach** (decompile-verified).
  A plain `"key".Translate(args)` — this mod's `ThingDef`/`HediffDef`/
  `RecipeDef`/`ThoughtDef` labels and descriptions — goes to
  `Verse.GrammarResolverSimple`, which gives you `{N_gender ? … : … : …}`,
  `{N_definite}`, `{N_indefinite}`, `{N_plural}` and the pronoun family, but
  implements **no `lookup` function at all**, so case forms are
  unavailable. `BTG_ThumbsUp`'s `logRulesInitiator/rulesStrings`, by
  contrast, are the full rulepack-style `GrammarResolver` (like the
  weapon-mod combat logs) — check which one a given string actually reaches
  before assuming a capability is or isn't available.
- **The checker compares argument placeholders, not grammar constructs,
  and that distinction is deliberate.** `{0}`/`{PAWN_labelShort}`-style
  placeholders are supplied by the C# call site and must match English
  exactly; `{PAWN_gender ? o : a}` is inflection the target language needs
  and uninflected English never has. `Scripts/check-translations.py`
  excludes any `{...}` containing `?` before comparing (see the comment on
  `GRAMMAR_CONSTRUCT_RE`). This mod's own def XML has no `{0}`-style
  argument today; its only symbols are the bracketed
  `[INITIATOR_nameDef]`/`[RECIPIENT_nameDef]` grammar tokens, which the
  checker's placeholder regex does not touch at all (see the note in "File
  map and conventions" above) — don't expect the checker to validate those.
- When an English string is reworded, refresh the EN comments in every
  language **in the same commit** — the checker reports the mismatch as
  STALE either way, but batching avoids churn.
- Coined vanilla terms may be a portmanteau in one language and a plain
  word in another — always check, never extrapolate between languages.
- Mod-coined terms recur across Def prose that restates them (this mod says
  "the Guild," "the thumb is the star," "bionic thumb" repeatedly across
  its ThingDef/HediffDef/RecipeDef descriptions and its InteractionDef/
  ThoughtDef flavour text). When generation is chunked across files or
  subagents, reconcile those terms across the whole language before
  committing.

The RulePackDef name-generation lessons the weapon-mod siblings carry — which
part of speech a `traitAdjectives`/`namerLabels`-style field needs per
language, the several techniques for solving name-grammar gender, and
material-neutral trait-adjective phrasing — do not apply here, since this
mod generates no names. The *grammar-resolution* lessons those same siblings
carry for combat-log text, however, **do** apply, via `BTG_ThumbsUp`'s
`rulesStrings` (see "Interaction log grammar" above) — this mod sits closer
to the weapon-mods' combat-log domain than to TSX's plain-Keyed domain on
that one axis, even though it has no combat and no RulePackDefs.

## Workflows

### Initial generation (`/translate <Language>`)

1. Run the checker; confirm English itself is clean (sidecar freshness —
   this mod has no English Keyed or DefInjected files to check directly,
   see "File map and conventions" above).
2. Enumerate the target key set from `Scripts/expected-injections.json`'s
   `required` entries per def type, taking the English source text from
   each entry's `english` field — NOT from `1.6/Defs/`, which is only a
   partial view (inherited `ParentName` base fields and C#-default strings
   don't show up there — see above). Today that surface happens to be
   `BTG_BionicThumb` (ThingDef and HediffDef), `BTG_InstallBionicThumb`,
   `BTG_ThumbsUp` (InteractionDef and ThoughtDef both use this defName —
   don't conflate the two DefInjected folders), and `BTG_ThumbsUpMood`, but
   the sidecar — not this list — is what to enumerate from; it is what
   catches the day this surface grows.
3. If translating a language with grounded Core vocabulary needs, extract
   the vanilla Core tar for the target language into the scratchpad; build
   a term list for bionic/surgery/interaction/thought vocabulary (see
   "Terminology grounding" above).
4. Translate via subagent(s) carrying: the glossary, the vanilla term list,
   the EN-comment requirement, placeholder rules, formatting rules, and the
   "Interaction log grammar" section for `BTG_ThumbsUp`'s `rulesStrings`
   specifically.
5. Run the checker (`--strict` for new languages); fix everything.
6. Review the diff yourself before committing. Commit message and PR text
   must state machine-assisted origin and invite native review.

### Update pass (`/translate update`)

1. Run the checker; it lists missing keys and stale entries per language.
2. Translate only that delta, refreshing each entry's EN comment.
3. Leave correct existing entries untouched. Re-run the checker.

### Audit only (`/translate check`)

Run the checker and report; change nothing.

## Optional in-game verification

RimWorld Dev Mode offers "Save translation report" and "clean up translation
files" (Verse.LanguageReportGenerator / TranslationFilesCleaner). These need a
running game with the mod loaded — useful as a final QA pass, not a
substitute for the checker.
