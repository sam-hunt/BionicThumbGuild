#!/usr/bin/env python3
# Pre-release startup smoke test: boots the real game once with Bionic Thumb
# Guild on its pinned minimal list, then classifies every Player.log error/
# warning by origin and fails on anything attributed to this mod. Thin shim
# over the shared engine in l10n/smoke/startup_smoke.py (see its header for
# mechanics and the BetterTradersGuild v1.1.0 CWTL incident this exists to
# catch). This mod has no optional integrations, so with nothing to gate on
# a seam, this is simply a clean-startup-log gate.
#
# Run this before every release, with the game closed:
#   python3 Scripts/integration-smoke-test.py              # boot + scan
#   python3 Scripts/integration-smoke-test.py --no-launch  # rescan last log
#   python3 Scripts/integration-smoke-test.py --strict     # any error fails

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent / "l10n" / "smoke"))
import startup_smoke as engine  # noqa: E402

engine.REPO_ROOT = Path(__file__).resolve().parent.parent

engine.PACKAGE_ID = "shunter.bionicthumbguild"

# RATIONALE: this is this repo's l10n CANONICAL_ACTIVE_MODS list (no DLC or
# mod deps) - Bionic Thumb Guild requires only Harmony and no DLC. There are
# no optional integration mods to boot alongside it, so this is a clean-
# startup-log gate rather than an integration-seam check. Probe last
# (auto-quit).
engine.SMOKE_ACTIVE_MODS = [
    "brrainz.harmony",
    "ludeon.rimworld",
    "shunter.bionicthumbguild",
    "shunter.l10nprobe",
]

engine.OWN_PATTERNS = ["BionicThumbGuild", "BionicThumb"]

# No optional integrations - any error attributed to this mod gates the test.
engine.INTEGRATION_PATTERNS = {}

raise SystemExit(engine.main())
