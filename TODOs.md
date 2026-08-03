# TODOs

## Features

- Test thumbs up interaction/moodlet
  `claude --resume "rimworld-mood-progression-thumbs"`

- Backport improved build/ci architecture from AAH
- Add thumbs down interaction/moodlet
- Thumb relic/ideo/meme/desire
- MIIB guaranteed thumb
- Archotech Thumb
- Thumb render node?
- Thumb artwork/tale focus?
- Add unique melee verb/combat log when pawns strike or are struck with body part

- thumb link/grip unique weapon trait that adds +5.0 shooting accuracy when used by a pawn with a Bionic Thumb (tm) installed
- capitalize the T for thumb in thing/hediff labels
- Evaluate whether `Scripts/test-windows.sh` is still necessary or the suite can
  run natively with `dotnet test Tests/1.6/BionicThumbGuild.Tests.csproj` — the idiomatic
  pattern BetterTradersGuild uses (its CLAUDE.md warns the Windows-interop script
  corrupts shared `obj/` incremental state; ArchotechAndroidHardware verified
  native runs work and dropped the script, AAH 9bc240f). `DeployToModFolder` is
  already Release-gated here, so Debug `dotnet test` builds won't redeploy.
