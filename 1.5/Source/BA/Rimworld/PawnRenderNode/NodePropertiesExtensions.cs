using Verse;

namespace BA
{
    public static class NodePropertiesExtensions
    {
        public static float LayerForLayerByDirection(this INodeProperties_LayerByDirection props, PawnRenderNode node, PawnDrawParms parms, float defaultValue)
        {
            if (parms.facing == Rot4.South)
                return props.SouthLayer ?? node.Props.baseLayer;
            if (parms.facing == Rot4.North)
                return props.NorthLayer ?? node.Props.baseLayer;
            return defaultValue;
        }
    }
}