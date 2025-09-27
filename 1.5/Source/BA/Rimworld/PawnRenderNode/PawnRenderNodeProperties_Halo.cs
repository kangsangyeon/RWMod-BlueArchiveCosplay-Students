using Verse;

namespace BA
{
    public class PawnRenderNodeProperties_Halo : PawnRenderNodeProperties, INodeProperties_LayerByDirection
    {
        public float? southLayer;
        public float? northLayer;

        public float? SouthLayer => southLayer;
        public float? NorthLayer => northLayer;
    }
}