# Korean — Bionic Thumb Guild glossary

No generation pass has run in this repo yet. Family-shared mechanics
(LanguageWorker behavior, style/corpus rules, vanilla-grounded common
vocabulary, including the full josa-marker mechanism) live in the `l10n/`
submodule at `l10n/languages/Korean.md` — this file holds only what is
specific to Bionic Thumb Guild, which today is limited to the
interaction-log analysis below (done ahead of any actual generation pass,
since it governs an unusual DefInjected surface).

## Interaction log symbols — `BTG_ThumbsUp`'s `rulesStrings`

Korean needs **no defensive quoting at all** around `[INITIATOR_nameDef]` /
`[RECIPIENT_nameDef]` in the seven `rulesStrings` lines — `FindLastChar`
skips ASCII `'` `"` `)` but not curly `" "` or corner `「 」`, so quoting
would actually break josa resolution rather than help it (see
`l10n/lessons.md`'s Korean-is-the-exception entry). Instead, mark every
*allomorphic* particle that follows one of these two symbols with the
appropriate token from `l10n/languages/Korean.md`'s eight-token table (e.g.
`[RECIPIENT_nameDef](을)를`, never `[RECIPIENT_nameDef]를`).

**The digit trap doesn't bite this mod's own content today, but watch for
it if that ever changes.** `BTG_ThumbsUpMood`'s five stage labels are
hardcoded English ordinal words ("two thumbs up", "three thumbs up", ...),
not a runtime-injected count, so `ReplaceJosa`'s digit-vs-word resolution
mismatch (a bare numeral always resolves to the vowel form, which is wrong
for 1/3/6/7/8/0) never triggers here. Phrase defensively anyway if a future
revision ever injects a literal count.

When a real generation pass runs, record mod-coined terms (bionic thumb
naming, the interaction's flavour phrasing) and any def-to-vanilla-template
reuse here.
