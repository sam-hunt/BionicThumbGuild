using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace BionicThumbGuild.TraderPatches
{
    /// <summary>
    /// Adds 2x bionic thumbs to Traders Guild settlement inventories after stock
    /// is regenerated. Only targets TradersGuild faction settlements; other faction
    /// settlements are unaffected. RegenerateStock is protected, so we use a string
    /// method name instead of nameof().
    ///
    /// It's really just for compatibility with BetterTradersGuild since Guild
    /// settlements don't have trade inventory comps in vanilla Odyssey iirc
    /// </summary>
    [HarmonyPatch(typeof(Settlement_TraderTracker), "RegenerateStock")]
    public static class SettlementTrader_RegenerateStock_Postfix
    {
        [HarmonyPostfix]
        public static void Postfix(Settlement_TraderTracker __instance)
        {
            // Only add to Traders Guild settlements (soft dependency — no error if faction doesn't exist)
            if (__instance.settlement?.Faction?.def?.defName != "TradersGuild")
                return;

            ThingOwner things = __instance.GetDirectlyHeldThings();
            if (things == null)
                return;

            for (int i = 0; i < 2; i++)
            {
                things.TryAdd(ThingMaker.MakeThing(BionicThumbDefOf.BTG_BionicThumb));
            }
        }
    }
}
