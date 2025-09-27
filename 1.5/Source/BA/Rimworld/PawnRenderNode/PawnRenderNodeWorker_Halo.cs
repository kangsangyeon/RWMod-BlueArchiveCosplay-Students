using UnityEngine;
using Verse;

namespace BA
{
    public class PawnRenderNodeWorker_Halo : PawnRenderNodeWorker_FlipWhenCrawling
    {
        public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
        {
            Vector3 offset = base.OffsetFor(node, parms, out pivot);
            if (node.Props is not BA.PawnRenderNodeProperties_Halo props)
                return offset;

            if (parms.pawn.kindDef is not BA.PawnKindDef pawnKind)
                return offset;

            offset = pawnKind.haloOffset;
            return offset;
        }

        public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
        {
            return base.ScaleFor(node, parms);
        }

        public override float LayerFor(PawnRenderNode node, PawnDrawParms parms)
        {
            var value = base.LayerFor(node, parms);
            if (node.Props is not BA.INodeProperties_LayerByDirection props)
                return value;
            return props.LayerForLayerByDirection(node, parms, value);
        }
    }
}