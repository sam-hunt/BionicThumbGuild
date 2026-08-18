# Glossary — Bionic Thumb Guild-specific terminology

These per-language files (`Russian.md`, `Japanese.md`, `ChineseSimplified.md`,
`Korean.md`, `German.md`, `Spanish.md`, `French.md`,
`PortugueseBrazilian.md`) hold everything about a language's translation
that is specific to this mod: mod-coined terms (none yet — see below),
def-to-vanilla-template reuse, and worked phrasing decisions tied to
specific `BTG_` defs.

**No generation pass has run in this repo yet**, so unlike the sibling mods'
glossaries these files carry no grounded term tables — they hold only the
one piece of mod-specific analysis done ahead of any actual pass: how to
handle the `[INITIATOR_nameDef]` / `[RECIPIENT_nameDef]` symbols in
`BTG_ThumbsUp`'s grammar-resolved `rulesStrings` (see SKILL.md's translation
surface section) in each language, since that surface is unusual for a
DefInjected-only mod. When a real generation pass runs, record its
mod-coined terms, phrasing decisions, and def-to-vanilla-template maps here,
per language.

This mod has no localized Steam Workshop title today (CONTRIBUTING.md lists
English only) — if one is ever added, keep it here and in sync with
`.steamworkshop/Description/<Language>.txt`'s title line, per the pattern
the sibling mods use.

Family-shared, mod-independent findings — LanguageWorker mechanics, style
and corpus rules, and vanilla-grounded common vocabulary (quality tiers,
Cancel/Reset buttons, trader-price phrasing, and so on) — live upstream in
the `l10n/` submodule at `l10n/languages/<Language>.md` (canonical checkout:
`~/dev/rimworld-l10n`), since they apply to any mod in the family, not just
this one. Check there before adding a row here — most of what the old,
pre-toolkit skill carried per language (quality tiers, Cancel/Reset, the
Odyssey trader-price lines) turned out to already be exactly this kind of
shared vocabulary and now lives upstream instead of being duplicated here.

When a future translation pass coins a new mod-specific term or surfaces a
correction to shared mechanics or vocabulary, record the former here and
send the latter upstream to the l10n repo rather than duplicating it here.
