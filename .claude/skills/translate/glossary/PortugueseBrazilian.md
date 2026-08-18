# Brazilian Portuguese — Bionic Thumb Guild glossary

No generation pass has run in this repo yet. Family-shared mechanics
(LanguageWorker behavior, style/corpus rules, vanilla-grounded common
vocabulary) live in the `l10n/` submodule at
`l10n/languages/PortugueseBrazilian.md` — this file holds only what is
specific to Bionic Thumb Guild.

## Interaction log symbols — `BTG_ThumbsUp`'s `rulesStrings`

`LanguageWorker_Portuguese` supplies no contraction of its own
(`l10n/languages/PortugueseBrazilian.md`), so the `de`/`em`/`a`/`por` +
`_definite` article traps documented there are the author's problem — but
`[INITIATOR_nameDef]` / `[RECIPIENT_nameDef]` in the seven `rulesStrings`
lines are bare name symbols, not `_definite` article slots, so that specific
trap doesn't apply to them directly. **No specific quoting or gender-hedging
decision for these two symbols was reached before this repo adopted the
shared toolkit** — when a pt-BR pass actually runs, check whether these
lines need the literal `(a)` gender hedge or the inline resolver split (the
answer depends on whether a given `rulesStrings` line needs a
gender-dependent word at all, per `l10n/languages/PortugueseBrazilian.md`'s
note on the two techniques) and record the decision here.

When a real generation pass runs, record mod-coined terms (bionic thumb
naming, the interaction's flavour phrasing) and any def-to-vanilla-template
reuse here.
