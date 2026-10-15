using RimWorld;
using Verse;

namespace BionicThumbGuild
{
    public class InteractionWorker_ThumbsUp : InteractionWorker
    {
        private static HediffDef _bionicThumb;
        private static HediffDef BionicThumb => _bionicThumb ??= DefDatabase<HediffDef>.GetNamed("BTG_BionicThumb");

        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            if (initiator.Inhumanized())
                return 0f;

            if (!initiator.health.hediffSet.HasHediff(BionicThumb))
                return 0f;

            return 0.01f;
        }
    }
}
