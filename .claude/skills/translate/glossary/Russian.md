# Russian — Bionic Thumb Guild glossary

No generation pass has run in this repo yet. Family-shared mechanics
(LanguageWorker behavior, style/corpus rules, vanilla-grounded common
vocabulary) live in the `l10n/` submodule at `l10n/languages/Russian.md` —
this file holds only what is specific to Bionic Thumb Guild.

## Interaction log symbols — `BTG_ThumbsUp`'s `rulesStrings`

`l10n/languages/Russian.md` documents guillemets `«…»` for cited names and
UI commands generally, but that rule is verified there against *quest/def*
names cited in a sentence, not against a bare personal-pawn-name symbol like
`[INITIATOR_nameDef]` / `[RECIPIENT_nameDef]` — German and Spanish both
found those two categories take different treatment (a cited def label is
quoted, an injected pawn name is left bare). **No specific decision for
these two symbols has been reached for Russian** — when a pass actually
runs, check the nearest vanilla analog (how Core's own combat/interaction
logs render an injected pawn name, not just how they cite a quest or def)
before assuming the guillemet rule transfers, and record the decision here.

When a real generation pass runs, record mod-coined terms (bionic thumb
naming, the interaction's flavour phrasing) and any def-to-vanilla-template
reuse here.
