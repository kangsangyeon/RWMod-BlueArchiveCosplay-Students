using Verse;

namespace BA
{
    public class PawnRenderNodeProperties_LayerByDirection : PawnRenderNodeProperties, INodeProperties_LayerByDirection
    {
        public float? northLayer;
        public float? southLayer;

        public float? NorthLayer => northLayer;
        public float? SouthLayer => southLayer;
    }
}