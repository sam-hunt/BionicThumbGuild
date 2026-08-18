# Spanish (Castellano) — Bionic Thumb Guild glossary

No generation pass has run in this repo yet. Family-shared mechanics
(LanguageWorker behavior, style/corpus rules, vanilla-grounded common
vocabulary) live in the `l10n/` submodule at `l10n/languages/Spanish.md` —
this file holds only what is specific to Bionic Thumb Guild, which today is
limited to the interaction-log analysis below (done ahead of any actual
generation pass, since it governs an unusual DefInjected surface).

## Interaction log symbols — `BTG_ThumbsUp`'s `rulesStrings`

`Verse.LanguageWorker_Spanish` returns names unchanged and unquoted — so
`[INITIATOR_nameDef]` / `[RECIPIENT_nameDef]` in the seven `rulesStrings`
lines stay bare, matching vanilla's own handling of injected names. These
are plain name symbols, not `_definite` article symbols, so the `de el` →
`del` / `a el` → `al` hand-contraction trap (`l10n/languages/Spanish.md`)
does not apply to them today — it would only bite if a future revision adds
a `_definite` slot next to one of these names.

When a real generation pass runs, record mod-coined terms (bionic thumb
naming, the interaction's flavour phrasing) and any def-to-vanilla-template
reuse here.
