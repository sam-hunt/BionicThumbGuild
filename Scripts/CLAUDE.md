# Scripts

`expected-injections.json` is generated output: never hand-edit it, not even
to delete the keys of a removed def. After adding, removing, or editing the
English label/description of any def, regenerate it with
`python3 Scripts/refresh-translation-expectations.py` (boots RimWorld; the
game must be closed). If the game can't be booted, leave the sidecar stale and
say so rather than patching it; the checker will flag it until a regen lands.
