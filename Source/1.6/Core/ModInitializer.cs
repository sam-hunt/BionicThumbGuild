using HarmonyLib;
using Verse;

namespace BionicThumbGuild
{
    [StaticConstructorOnStartup]
    public static class BionicThumbGuildMod
    {
        static BionicThumbGuildMod()
        {
            var harmony = new Harmony("samhunt.bionicthumbguild");
            harmony.PatchAll();

            Log.Message($"[Bionic Thumb Guild] Initialized with {harmony.GetPatchedMethods().EnumerableCount()} patches.");
        }
    }
}
