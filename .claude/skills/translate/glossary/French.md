# French — Bionic Thumb Guild glossary

No generation pass has run in this repo yet. Family-shared mechanics
(LanguageWorker behavior, style/corpus rules, vanilla-grounded common
vocabulary) live in the `l10n/` submodule at `l10n/languages/French.md` —
this file holds only what is specific to Bionic Thumb Guild.

## Interaction log symbols — `BTG_ThumbsUp`'s `rulesStrings`

`LanguageWorker_French`'s elision/contraction rewrites
(`l10n/languages/French.md`) apply to these seven grammar-resolved lines
same as to any other French string, but `[INITIATOR_nameDef]` /
`[RECIPIENT_nameDef]` are bare name symbols, not `_definite`/`_indefinite`
article slots, so the `de le`/`à le` contraction traps don't apply to them
directly. **No specific quoting decision for these two symbols was reached
before this repo adopted the shared toolkit** (unlike German/Spanish above,
where "leave bare" was explicitly verified) — when a French pass actually
runs, check the nearest vanilla analog for how an injected bare pawn name is
punctuated in French prose and record the decision here.

When a real generation pass runs, record mod-coined terms (bionic thumb
naming, the interaction's flavour phrasing) and any def-to-vanilla-template
reuse here.
