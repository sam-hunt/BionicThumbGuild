# Japanese — Bionic Thumb Guild glossary

No generation pass has run in this repo yet. Family-shared mechanics
(LanguageWorker behavior, style/corpus rules, vanilla-grounded common
vocabulary) live in the `l10n/` submodule at `l10n/languages/Japanese.md` —
this file holds only what is specific to Bionic Thumb Guild, which today is
limited to the interaction-log analysis below (done ahead of any actual
generation pass, since it governs an unusual DefInjected surface).

## Interaction log symbols — `BTG_ThumbsUp`'s `rulesStrings`

Vanilla ja quotes injected def labels and cross-referenced UI labels with
「」, with no leading space before a following suffix or parenthetical. This
applies directly to `[INITIATOR_nameDef]` / `[RECIPIENT_nameDef]` in the
seven `rulesStrings` lines — wrap each occurrence in 「」. Check
`l10n/lessons.md`'s note on quote-mark-is-per-slot-not-per-language before
assuming this 「」 convention ports to any other bracketed content in this
mod (a cited note or inscription, say) without checking the nearer vanilla
analog first.

When a real generation pass runs, record mod-coined terms (bionic thumb
naming, the interaction's flavour phrasing) and any def-to-vanilla-template
reuse here.
