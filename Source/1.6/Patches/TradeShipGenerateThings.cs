using HarmonyLib;
using RimWorld;
using Verse;

namespace BionicThumbGuild.TraderPatches
{
    /// <summary>
    /// Adds 2x bionic thumbs to all orbital trader (passing ship) inventories
    /// after their stock is generated.
    /// </summary>
    [HarmonyPatch(typeof(TradeShip), nameof(TradeShip.GenerateThings))]
    public static class TradeShip_GenerateThings_Postfix
    {
        [HarmonyPostfix]
        public static void Postfix(TradeShip __instance)
        {
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
