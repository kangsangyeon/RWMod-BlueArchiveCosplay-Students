using RimWorld;
using UnityEngine;
using Verse;

namespace BA
{
    public class PawnRenderNode_HeadgearLining : PawnRenderNode
    {
        private readonly Pawn _pawn;

        public PawnRenderNode_HeadgearLining(
            Pawn pawn,
            PawnRenderNodeProperties props,
            PawnRenderTree tree)
            : base(pawn, props, tree)
        {
            _pawn = pawn;
        }
        
        protected override void EnsureMaterialsInitialized()
        {
            if (_pawn.DevelopmentalStage.Baby() || _pawn.DevelopmentalStage.Newborn())
                return;
            var apparel = _pawn.apparel.WornApparel.FirstOrDefault(x =>
                x.def.apparel.layers.Contains(ApparelLayerDefOf.Overhead) &&
                x.def is BA.ApparelDef);
            if (apparel == null)
                return;
            if (apparel.def is not BA.ApparelDef apparelDef)
                return;
            if (string.IsNullOrEmpty(apparelDef.texPathHeadgearLining))
                return;
            var shader = ShaderDatabase.Cutout;
            this.graphic = GraphicDatabase.Get<Graphic_Multi>(apparelDef.texPathHeadgearLining, shader, Vector2.one, ColorFor(_pawn));
        }
    }
}