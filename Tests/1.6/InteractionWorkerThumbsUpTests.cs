using System.Reflection;
using RimWorld;
using Verse;
using Xunit;

namespace BionicThumbGuild.Tests
{
    // InteractionWorker_ThumbsUp.RandomSelectionWeight reads a live Pawn's
    // health.hediffSet (via HasHediff) plus the Inhumanized() extension, and
    // looks up its gating hediff through DefDatabase<HediffDef>.GetNamed —
    // none of that resolves without a booted game and loaded defs, so the
    // gating behavior itself isn't headless-testable (matches the family's
    // "no Find.*, no DefDatabase reads" line the other test suites draw).
    // What's left is the override shape: the right base class, the right
    // signature, and the right return type — a mismatch there would mean
    // RimWorld's interaction system never calls this worker at all.
    public class InteractionWorkerThumbsUpTests
    {
        [Fact]
        public void ThumbsUpWorker_DerivesFromInteractionWorker()
        {
            Assert.True(typeof(InteractionWorker).IsAssignableFrom(typeof(InteractionWorker_ThumbsUp)));
        }

        [Fact]
        public void ThumbsUpWorker_OverridesRandomSelectionWeight_WithExpectedSignature()
        {
            MethodInfo method = typeof(InteractionWorker_ThumbsUp).GetMethod(
                nameof(InteractionWorker.RandomSelectionWeight),
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(Pawn), typeof(Pawn) },
                null);

            Assert.NotNull(method);
            Assert.Equal(typeof(float), method.ReturnType);
            // Declared on the subclass itself (not just inherited), i.e. it's
            // actually overridden rather than falling through to the base's
            // default weight.
            Assert.Equal(typeof(InteractionWorker_ThumbsUp), method.DeclaringType);
        }

        [Fact]
        public void ThumbsUpWorker_HasPublicParameterlessConstructor()
        {
            // RimWorld's InteractionDef instantiates workerClass via
            // Activator.CreateInstance, which needs a public parameterless
            // constructor. The class doesn't declare one explicitly, so this
            // guards against a future edit (e.g. adding DI-style constructor
            // params) that would silently break InteractionDef loading.
            Assert.NotNull(typeof(InteractionWorker_ThumbsUp).GetConstructor(System.Type.EmptyTypes));
        }
    }
}
