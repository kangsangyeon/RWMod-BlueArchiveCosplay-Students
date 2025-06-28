using Verse;

namespace BA
{
    public class PawnRenderNodeWorker_LayerByDirection : PawnRenderNodeWorker_FlipWhenCrawling
    {
        public override float LayerFor(PawnRenderNode node, PawnDrawParms parms)
        {
            var value = base.LayerFor(node, parms);
            if (node.Props is not BA.INodeProperties_LayerByDirection props)
                return value;
            return props.LayerForLayerByDirection(node, parms, value);
        }
    }
}