using Verse;

namespace BA
{
    public class PawnRenderNodeProperties_Wing : PawnRenderNodeProperties, INodeProperties_LayerByDirection
    {
        public bool isRight;
        public float? southLayer;
        public float? northLayer;

        public float? SouthLayer => southLayer;
        public float? NorthLayer => northLayer;
    }
}