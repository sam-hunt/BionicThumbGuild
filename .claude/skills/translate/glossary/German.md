# German — Bionic Thumb Guild glossary

No generation pass has run in this repo yet. Family-shared mechanics
(LanguageWorker behavior, style/corpus rules, vanilla-grounded common
vocabulary) live in the `l10n/` submodule at `l10n/languages/German.md` —
this file holds only what is specific to Bionic Thumb Guild, which today is
limited to the interaction-log analysis below (done ahead of any actual
generation pass, since it governs an unusual DefInjected surface).

## Interaction log symbols — `BTG_ThumbsUp`'s `rulesStrings`

`Verse.LanguageWorker_German`'s `PostProcessed` rewrites a trailing English
`'s` to `s` (or a bare `'` after s/ß/z/x/ce), and vanilla de never quotes
pawn names in cited-name slots (only cited def/UI labels take ASCII single
quotes). So leave `[INITIATOR_nameDef]` / `[RECIPIENT_nameDef]` bare and
unquoted in the seven `rulesStrings` lines — never write `'[RECIPIENT_
nameDef]'s`, which the worker would mangle. Case, not gender, is the
landmine for any oblique slot built around these symbols (see
`l10n/languages/German.md`'s case-vs-gender finding and
`l10n/lessons.md`'s possessive-symbol entry) — restructure a sentence that
would need a dative/accusative/genitive form of a pawn name rather than
guessing an article, since neither `GrammarResolverSimple` nor the full
`GrammarResolver` these lines reach implements a `lookup` function.

When a real generation pass runs, record mod-coined terms (bionic thumb
naming, the interaction's flavour phrasing) and any def-to-vanilla-template
reuse here.
