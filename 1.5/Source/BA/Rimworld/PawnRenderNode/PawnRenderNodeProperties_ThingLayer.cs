using Verse;

namespace BA
{
    public class PawnRenderNodeProperties_ThingLayer : PawnRenderNodeProperties, INodeProperties_ThingLayer
    {
        public bool isFront;
        public bool isBack;

        public bool IsFront => isFront;
        public bool IsBack => isBack;
    }
}