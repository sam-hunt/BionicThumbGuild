# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Bionic Thumb Guild** is a RimWorld 1.6 mod that adds a bionic thumb prosthetic. Requires Harmony. Does not require any DLC.

**Key Features:**

- Bionic Thumb artificial body part (ThingDef) with matching hediff at 120% manipulation efficiency
- Surgical installation recipe targeting hands (uses standard surgery base)
- Harmony patch hooking orbital trader stock generation to add 2x bionic thumbs to all orbital trader inventories

**Key Technologies:** C# (.NET Framework 4.7.2), Harmony library, RimWorld modding API, XML definitions

## Build Commands

```bash
# Build the mod (outputs to 1.6/Assemblies/ and deploys to RimWorld Mods folder)
dotnet build BionicThumbGuild.sln -c Release

# Build only the main project
dotnet build Source/1.6/BionicThumbGuild.csproj

# Clean build artifacts
dotnet clean BionicThumbGuild.sln

# Clean deployed mod folder (use when Defs/Patches are renamed or deleted)
dotnet build Source/1.6/BionicThumbGuild.csproj -t:CleanModFolder
```

The build system auto-detects the RimWorld installation path on Windows/Linux/Mac (including WSL targeting a Windows install). For CI builds without RimWorld installed, it falls back to the `Krafs.Rimworld.Ref` NuGet package.

### Deployment

The repo lives in `~/dev/BionicThumbGuild`, separate from the RimWorld Mods folder. A post-build MSBuild target (`DeployToModFolder`) automatically copies only runtime files (About, Assemblies, Defs, Patches, LoadFolders.xml) to `$RIMWORLD_PATH/Mods/BionicThumbGuild/`. It uses `SkipUnchangedFiles` for fast incremental builds.

**Important:** The deploy copies files but does not delete stale files. If you rename or delete a Def/Patch XML, run `dotnet build Source/1.6/BionicThumbGuild.csproj -t:CleanModFolder` to wipe the deployed folder, then rebuild to redeploy cleanly.

**WSL Setup:** Requires `RIMWORLD_PATH` env var in `~/.bashrc` pointing to the Windows RimWorld install (e.g., `/mnt/c/Program Files (x86)/Steam/steamapps/common/RimWorld`). The csproj auto-detects `RimWorldWin64_Data` when the Linux data folder isn't found.

## Architecture

### Entry Point

`Source/1.6/Core/ModInitializer.cs` - Static constructor with `[StaticConstructorOnStartup]` auto-patches via Harmony attribute discovery. Logs initialization message with patch count.

### Directory Structure

```
Source/1.6/
├── Core/           # ModInitializer
├── Patches/        # Harmony patches (orbital trader stock generation hook)
└── Properties/     # AssemblyInfo

1.6/Defs/           # XML definitions
├── ThingDefs/      # Bionic Thumb body part item
├── HediffDefs/     # Bionic Thumb hediff (120% efficiency)
└── RecipeDefs/     # Installation surgery recipe

1.6/Patches/        # XML patches (if needed)
```

### Key Patterns

**Harmony Patching:** All patches use `[HarmonyPatch]` attributes for automatic discovery. Patches are organized by target class in subdirectories under `Patches/`.

**Namespace Convention:** Use `*Patches` suffix for patch namespaces to avoid RimWorld type conflicts (e.g., `TraderPatches`).

### Feature Details

**Bionic Thumb Body Part:**
- ThingDef for the bionic thumb item (craftable/purchasable)
- HediffDef with `addedPartProps` at 120% part efficiency
- Targets the thumb body part record on human pawns

**Installation Recipe:**
- RecipeDef for surgical installation targeting hands
- Uses standard `SurgeryInstallBodyPartArtificialBase` parent
- Standard bionic surgery skill requirements

**Orbital Trader Stock Hook:**
- Harmony Postfix on trader stock generation methods
- Adds 2x BionicThumb ThingDef to all orbital trader inventories when stock is generated
- Targets `TraderStock` or equivalent stock generation to inject items

## Debugging

1. **Enable RimWorld Dev Mode:** Settings → Dev Mode → Logging
2. **Log locations:**
   - **Windows:** `%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log`
   - **WSL:** `/mnt/c/Users/*/AppData/LocalLow/Ludeon Studios/RimWorld by Ludeon Studios/Player.log`
3. **Logging:** Use `Log.Message("[Bionic Thumb Guild] ...")` for mod-specific logs
4. **Inspect RimWorld API:** `monodis "/mnt/c/.../RimWorldWin64_Data/Managed/Assembly-CSharp.dll"`

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
