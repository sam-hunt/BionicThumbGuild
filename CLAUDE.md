# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Bionic Thumb Guild** is a RimWorld 1.6 mod that adds a bionic thumb prosthetic. Requires Harmony. Does not require any DLC.

**Key Features:**

- Bionic Thumb artificial body part (ThingDef) with matching hediff at 120% manipulation efficiency
- Surgical installation recipe targeting hands (uses standard surgery base)
- Harmony patches adding 2x bionic thumbs to orbital trader and Traders Guild settlement stock
- Thumbs-up social interaction: pawns with the bionic thumb can give a thumbs up, granting a +5 opinion memory and an escalating stacking mood buff (up to five thumbs)

**Key Technologies:** C# (.NET Framework 4.7.2), Harmony library, RimWorld modding API, XML definitions

## Build Commands

```bash
# Build the mod (outputs to 1.6/Assemblies/ and deploys to RimWorld Mods folder)
dotnet build BionicThumbGuild.sln -c Release

# Build only the main project
dotnet build Source/1.6/BionicThumbGuild.csproj

# Full clean rebuild
dotnet clean BionicThumbGuild.sln && dotnet build BionicThumbGuild.sln -c Release

# Run the test suite (native; vstest hosts the net472 suite via mono)
dotnet test Tests/1.6/BionicThumbGuild.Tests.csproj

# Validate translations / sidecar freshness
python3 Scripts/check-translations.py --strict
```

The build system auto-detects the RimWorld installation path on Windows/Linux/Mac (including WSL targeting a Windows install). For CI builds without RimWorld installed, it falls back to the `Krafs.Rimworld.Ref` NuGet package.

### Deployment

The repo lives in `~/dev/BionicThumbGuild`, separate from the RimWorld Mods folder. The csproj's `StageMod` target is the **single source of truth** for what files ship: its ItemGroup feeds both the post-build local deploy (`DeployToModFolder` → `StageMod`, an atomic wipe+recopy of `$RIMWORLD_PATH/Mods/BionicThumbGuild/`, so renamed/deleted files never linger) and the CI release, which invokes the same target with `-p:StageDir=...` so the release zip cannot drift from local deploys. Add/remove shipped files only in that ItemGroup.

A machine-local Claude Code Stop hook (`.claude/hooks/sync-mod.sh`, untracked) rebuilds and redeploys after any turn that touched mod files, so the deployed copy stays fresh without manual builds.

**WSL Setup:** Requires `RIMWORLD_PATH` env var in `~/.bashrc` pointing to the Windows RimWorld install (e.g., `/mnt/c/Program Files (x86)/Steam/steamapps/common/RimWorld`). The csproj auto-detects `RimWorldWin64_Data` when the Linux data folder isn't found.

## Architecture

### Entry Point

`Source/1.6/Core/ModInitializer.cs` - Static constructor with `[StaticConstructorOnStartup]` auto-patches via Harmony attribute discovery. Logs initialization message with patch count.

### Directory Structure

```
Source/1.6/
├── Core/           # ModInitializer
├── DefRefs/        # BionicThumbDefOf ([DefOf] static refs)
├── Interactions/   # InteractionWorker_ThumbsUp
├── Patches/        # Harmony patches (orbital trader + settlement stock hooks)
└── Properties/     # AssemblyInfo

1.6/Defs/           # XML definitions
├── ThingDefs/      # Bionic Thumb body part item
├── HediffDefs/     # Bionic Thumb hediff (120% efficiency)
├── RecipeDefs/     # Installation surgery recipe
├── InteractionDefs/# Thumbs-up social interaction (grammar-resolved log lines)
└── ThoughtDefs/    # Thumbs-up opinion memory + stacking mood thought

Textures/           # Root-level (LoadFolders.xml loads "/"): thumbs-up speech symbol

Tests/1.6/          # Headless xUnit (net472) suite
Scripts/            # Translation checker/refresh, sidecar
```

### .claude layout

`.gitignore` tracks only `.claude/skills/` (shared: `release`, `translate`, `rimworld-logs`); `.claude/hooks/` and `.claude/settings.local.json` (Stop-hook wiring, permissions) stay machine-local.

### Key Patterns

**Harmony Patching:** All patches use `[HarmonyPatch]` attributes for automatic discovery. Patches are organized by target class in subdirectories under `Patches/`.

**Namespace Convention:** Use `*Patches` suffix for patch namespaces to avoid RimWorld type conflicts (e.g., `TraderPatches`).

**DefOf References:** `BionicThumbDefOf.BTG_BionicThumb` is populated by RimWorld's `[DefOf]` startup scan matching field *names* against defNames — a rename silently leaves the field null (the test suite pins this).

### Feature Details

**Bionic Thumb Body Part:**
- ThingDef for the bionic thumb item (craftable/purchasable)
- HediffDef with `addedPartProps` at 120% part efficiency
- Targets the thumb body part record on human pawns

**Installation Recipe:**
- RecipeDef for surgical installation targeting hands
- Uses standard `SurgeryInstallBodyPartArtificialBase` parent
- Standard bionic surgery skill requirements

**Trader Stock Hooks:**
- Harmony Postfix on `TradeShip.GenerateThings` adds 2x bionic thumbs to orbital traders
- Harmony Postfix on `Settlement_TraderTracker.RegenerateStock` (protected — patched via string method name) does the same for Traders Guild settlements (BetterTradersGuild compatibility)

**Thumbs-Up Interaction:**
- `InteractionWorker_ThumbsUp.RandomSelectionWeight` gates on the initiator having the `BTG_BionicThumb` hediff (weight 0.01, inhumanized pawns excluded)
- Recipient gets `BTG_ThumbsUp` social memory (+5 opinion) which chains to `BTG_ThumbsUpMood`, a five-stage stacking mood thought
- Interaction log lines are grammar-resolved `rulesStrings` with `[INITIATOR_nameDef]`/`[RECIPIENT_nameDef]` symbols

## Testing

`Tests/1.6/` holds an xUnit (net472) suite. This mod has no extractable pure logic, so the tests pin what breaks silently at runtime: Harmony patch targets (including the string-literal `"RegenerateStock"` name), postfix wiring, the interaction worker's override shape, and the `BionicThumbDefOf` field both patches rely on. Tests are headless — anything needing `DefDatabase`/`Current.Game`/`ThingMaker` is out of scope. Run natively with `dotnet test Tests/1.6/BionicThumbGuild.Tests.csproj` — vstest hosts the net472 suite via mono. If a run fails with `BadImageFormatException`/`TypeLoadException`, a DLL is missing from the test csproj copy target (see the Assembly-CSharp-firstpass comment there): mono resolves field types eagerly where the Windows CLR is lazy. CI builds the Tests project but does not run it.

## Localization

English lives in the Defs XML — this mod ships **no Keyed strings** (no settings UI) and no `Languages/` tree yet; its entire translatable surface is DefInjected. This mod's own facts and per-language interaction-log analysis live in the `translate` skill (`glossary/<Language>.md` beside it); the contributor-facing rules and the language roster live in `CONTRIBUTING.md`.

- **Shared l10n toolkit (`l10n/` submodule):** the family-wide translation process, per-language mechanics references, cross-language lessons, Workshop conventions, and the checker/refresh script engines live in the `rimworld-l10n` repo, consumed here as the `l10n/` git submodule (canonical working checkout: `~/dev/rimworld-l10n`). `Scripts/check-translations.py` and `Scripts/refresh-translation-expectations.py` are thin per-repo config shims over its engines. If `l10n/` is empty, run `git submodule update --init`. Never edit `l10n/` in place here: mod-independent learnings go upstream in the canonical checkout, then the pin is bumped in each consuming repo; mod-specific learnings go in this repo's skill/glossary.

- `python3 Scripts/check-translations.py [--strict]` — deterministic validator (key/placeholder parity, `<!-- EN: ... -->` staleness comments, DefInjected paths, file hygiene). A missing `Languages/` tree is a legal state (`ALLOW_NO_KEYED_SURFACE = True` in the shim); the sidecar freshness check runs regardless. Run by the `translate` and `release` skills and as a CI release gate.
- `Scripts/expected-injections.json` — checked-in sidecar of every DefInjected key the live game expects for this mod (21 keys across 5 def types); regenerated by `python3 Scripts/refresh-translation-expectations.py` — it boots RimWorld (game must be closed) with a pinned mod list via the L10nProbe dev mod (source lives at `l10n/probe/`; build/deploy it only from the canonical `~/dev/rimworld-l10n` checkout — a submodule copy refuses to deploy by design), then restores `ModsConfig.xml`. Any def added or English label/description edited without a regen fails the checker.
- CONTRIBUTING.md carries the public roster (English only so far) and must move in the same commit as any language change. **Do not bulk-generate translations** — one language at a time, on request.
- **Policy:** translation generation passes run only on explicit request (they are token-expensive). Infra/tooling changes are always fine.
- The Steam Workshop page is a separate source of truth from the in-game surface above: the Workshop title and description live in `.steamworkshop/Description/English.txt` (line 1 title, matching `About/About.xml`'s `<name>`, then a blank line, then the BBCode body), pasted manually into Steam since it has no per-language API. If in-game localization is ever added, localized Workshop titles couple to their own `.steamworkshop/Description/<Language>.txt` files per `.steamworkshop/README.md`.

## Linting

Roslynator.Analyzers runs on every build (warnings only, never fails the build; `PrivateAssets=all` so nothing ships). Severities are pinned in `.editorconfig`, which also enforces the no-XML-doc-comments convention (plain `//` only). Formatting-only sweeps are registered in `.git-blame-ignore-revs`.

## Debugging

Use the `rimworld-logs` skill — it covers Player.log locations (Windows/WSL/Linux), the `[Bionic Thumb Guild]` log prefix, and API disassembly (`monodis`/`ilspycmd` against the live install's `Assembly-CSharp.dll`, preferred over the `Krafs.Rimworld.Ref` CI fallback).

## Harmony Patch Examples

**Postfix Pattern:**

```csharp
[HarmonyPatch(typeof(TargetClass), nameof(TargetClass.MethodName))]
public static class TargetClass_MethodName_Postfix
{
    [HarmonyPostfix]
    public static void Postfix(TargetClass __instance, ref ReturnType __result)
    {
        // __instance: object method was called on
        // __result: return value (modifiable with ref)
    }
}
```

**Prefix Pattern (for skipping original):**

```csharp
[HarmonyPrefix]
public static bool Prefix(ref ReturnType __result)
{
    __result = newValue;
    return false; // Skip original method
}
```
