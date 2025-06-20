using Verse;

namespace BA
{
    public class PawnRenderNodeProperties_LayerByDirection : PawnRenderNodeProperties, INodeProperties_LayerByDirection
    {
        public float? northLayer;
        public float? southLayer;

        public float? NorthLayer => northLayer;
        public float? SouthLayer => southLayer;

        public PawnRenderNodeProperties_LayerByDirection()
        {
            if (!typeof(PawnRenderNodeWorker_LayerByDirection).IsAssignableFrom(this.workerClass))
                this.workerClass = typeof(PawnRenderNodeWorker_LayerByDirection);
            if (!northLayer.HasValue)
                northLayer = baseLayer;
            if (!southLayer.HasValue)
                southLayer = baseLayer;
        }
    }
}