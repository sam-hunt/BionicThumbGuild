using System.Linq;
using System.Reflection;
using BionicThumbGuild.TraderPatches;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Xunit;

namespace BionicThumbGuild.Tests
{
    // Both orbital-trader-stock patches mutate a live IThingHolder's
    // ThingOwner (TradeShip.GetDirectlyHeldThings() / Settlement_TraderTracker.
    // GetDirectlyHeldThings()) via ThingMaker.MakeThing, which needs a fully
    // spawned ThingDef and a real map/world context to run — none of that
    // exists headlessly. What's left to guard without booting the game is the
    // patch *shape*: the [HarmonyPatch] attribute's target type/method name
    // (a typo here silently no-ops the patch — Harmony logs a warning instead
    // of throwing) and that the postfix method is actually wired up with
    // [HarmonyPostfix]. These tests read that attribute metadata via
    // reflection only; they never invoke the patched methods.
    public class TraderStockPatchTests
    {
        // ---- TradeShip_GenerateThings_Postfix ----------------------------------

        [Fact]
        public void TradeShipPatch_TargetsTradeShipGenerateThings()
        {
            var attr = typeof(TradeShip_GenerateThings_Postfix).GetCustomAttribute<HarmonyPatch>();

            Assert.NotNull(attr);
            Assert.Equal(typeof(TradeShip), attr.info.declaringType);
            Assert.Equal(nameof(TradeShip.GenerateThings), attr.info.methodName);
        }

        [Fact]
        public void TradeShipPatch_PostfixMethod_IsPublicStaticAndMarkedAsPostfix()
        {
            MethodInfo postfix = typeof(TradeShip_GenerateThings_Postfix)
                .GetMethod("Postfix", BindingFlags.Public | BindingFlags.Static);

            Assert.NotNull(postfix);
            Assert.NotNull(postfix.GetCustomAttribute<HarmonyPostfix>());
        }

        [Fact]
        public void TradeShipPatch_PostfixMethod_TakesTradeShipInstance()
        {
            MethodInfo postfix = typeof(TradeShip_GenerateThings_Postfix)
                .GetMethod("Postfix", BindingFlags.Public | BindingFlags.Static);

            ParameterInfo instanceParam = postfix.GetParameters()
                .FirstOrDefault(p => p.Name == "__instance");

            Assert.NotNull(instanceParam);
            Assert.Equal(typeof(TradeShip), instanceParam.ParameterType);
        }

        // ---- SettlementTrader_RegenerateStock_Postfix --------------------------

        [Fact]
        public void SettlementTraderPatch_TargetsRegenerateStock()
        {
            // RegenerateStock is protected on Settlement_TraderTracker, so the
            // production attribute uses a string method name instead of
            // nameof() — that's the one place a rename in the base game would
            // slip past a compile-time check, which is exactly why this is
            // pinned here.
            var attr = typeof(SettlementTrader_RegenerateStock_Postfix).GetCustomAttribute<HarmonyPatch>();

            Assert.NotNull(attr);
            Assert.Equal(typeof(Settlement_TraderTracker), attr.info.declaringType);
            Assert.Equal("RegenerateStock", attr.info.methodName);
        }

        [Fact]
        public void SettlementTraderPatch_PostfixMethod_IsPublicStaticAndMarkedAsPostfix()
        {
            MethodInfo postfix = typeof(SettlementTrader_RegenerateStock_Postfix)
                .GetMethod("Postfix", BindingFlags.Public | BindingFlags.Static);

            Assert.NotNull(postfix);
            Assert.NotNull(postfix.GetCustomAttribute<HarmonyPostfix>());
        }

        [Fact]
        public void SettlementTraderPatch_PostfixMethod_TakesSettlementTraderTrackerInstance()
        {
            MethodInfo postfix = typeof(SettlementTrader_RegenerateStock_Postfix)
                .GetMethod("Postfix", BindingFlags.Public | BindingFlags.Static);

            ParameterInfo instanceParam = postfix.GetParameters()
                .FirstOrDefault(p => p.Name == "__instance");

            Assert.NotNull(instanceParam);
            Assert.Equal(typeof(Settlement_TraderTracker), instanceParam.ParameterType);
        }
    }
}
