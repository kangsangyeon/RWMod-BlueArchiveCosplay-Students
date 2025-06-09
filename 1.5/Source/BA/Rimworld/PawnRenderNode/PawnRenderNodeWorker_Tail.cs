using Verse;

namespace BA
{
    public class PawnRenderNodeWorker_Tail : PawnRenderNodeWorker_FlipWhenCrawling
    {
        public override bool CanDrawNow(
            PawnRenderNode node,
            PawnDrawParms parms)
        {
            bool canDrawNow = base.CanDrawNow(node, parms);
            if (node.Props is not BA.PawnRenderNodeProperties_Tail tailNodeProps)
                return canDrawNow;
            if (tailNodeProps.isNorthOnly)
            {
                if (parms.facing != Rot4.North)
                    return false;
            }
            else
            {
                if (parms.facing == Rot4.North)
                    return false;
            }

            return canDrawNow;
        }
    }
}