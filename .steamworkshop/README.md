# .steamworkshop

Publishing metadata for the mod's Steam Workshop page. Nothing in this folder
ships with the mod (the StageMod manifest never matches it) or is loaded by
RimWorld. A `Media/` folder for Workshop images can live here later.

## Description/

One file per language, named after RimWorld's language folder names. This
mod has no `Languages/` tree today, so English is the only file; add one per
language if in-game localization is ever added. Format:

- Line 1: the Workshop title for that language
- Line 2: blank
- Rest: the BBCode description

The English title line must equal `About/About.xml`'s `<name>` (`Bionic
Thumb Guild`).

Title convention, once translations exist: just as the English title leans
on vanilla vocabulary ("Bionic") so players searching for bionics and
prosthetics find the mod, every localized title should lean on that
language's vanilla-localized bionic/prosthetic vocabulary. Titles are fully
localized with no English brand appended: Workshop search is
language-agnostic (any language's title matches regardless of UI language,
verified 2026-08-12) and the preview thumbnail already carries the English
name.

Steam has no API for per-language Workshop text, so updated files are pasted
manually into the Workshop page's edit UI (note Steam's own language names
differ: schinese, koreana, brazilian, latam, ...). The `release` skill diffs
`English.txt` against the last release tag and refreshes the translations
whenever it changed.
