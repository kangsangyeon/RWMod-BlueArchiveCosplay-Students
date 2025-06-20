using Verse;

namespace BA
{
    public class PawnRenderNodeWorker_LayerByDirection : PawnRenderNodeWorker_FlipWhenCrawling
    {
        public override float LayerFor(PawnRenderNode node, PawnDrawParms parms)
        {
            if (parms.facing.IsHorizontal)
                return base.LayerFor(node, parms);
            if (node.Props is not BA.PawnRenderNodeProperties_LayerByDirection props)
                return base.LayerFor(node, parms);
            if (parms.facing == Rot4.South)
                return props.SouthLayer.Value;
            if (parms.facing == Rot4.North)
                return props.NorthLayer.Value;
            return base.LayerFor(node, parms);
        }
    }
}