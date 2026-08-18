#!/usr/bin/env python3
# BionicThumbGuild's config shim over the shared sidecar-refresh engine
# (l10n/refresh/refresh_expectations.py — the rimworld-l10n submodule),
# which drives the L10nProbe dev mod (source at l10n/probe/; build/deploy it
# only from the canonical ~/dev/rimworld-l10n checkout). The engine holds all
# logic; this file holds only this repo's config and the rationale behind it.
# Usage is unchanged (game must be closed):
#   python3 Scripts/refresh-translation-expectations.py [--no-launch]
# If l10n/ is empty, run: git submodule update --init

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent / "l10n" / "refresh"))
import refresh_expectations as engine  # noqa: E402  (import after sys.path edit)

engine.REPO_ROOT = Path(__file__).resolve().parent.parent

engine.PACKAGE_ID = "shunter.bionicthumbguild"

# RATIONALE: This mod requires no DLC (About.xml has no DLC
# modDependencies) and gates no content behind MayRequire, so no DLC belongs
# in the pinned list. This repo is not part of a probed family (unlike the
# UniqueMeleeWeapons trio riding along in one boot for convenience), so the
# list has no siblings to add. No third-party mod is MayRequired by this
# mod's content today, so none is listed.
engine.CANONICAL_ACTIVE_MODS = [
    "brrainz.harmony",
    "ludeon.rimworld",
    "shunter.bionicthumbguild",
    "shunter.l10nprobe",
]

raise SystemExit(engine.main())
