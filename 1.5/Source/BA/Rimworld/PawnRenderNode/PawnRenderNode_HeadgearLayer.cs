using RimWorld;
using UnityEngine;
using Verse;

namespace BA
{
    public class PawnRenderNode_HeadgearLayer : Verse.PawnRenderNode
    {
        private readonly Pawn _pawn;
        private readonly BA.INodeProperties_ThingLayer _layerProps;

        public PawnRenderNode_HeadgearLayer(
            Pawn pawn,
            PawnRenderNodeProperties props,
            PawnRenderTree tree)
            : base(pawn, props, tree)
        {
            _pawn = pawn;
            _layerProps = props as BA.INodeProperties_ThingLayer;
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
            var shader = ShaderDatabase.Cutout;
            var texPath = string.Empty;
            if (_layerProps.IsFront)
                texPath = apparelDef.texPathFrontLayer;
            else if (_layerProps.IsBack)
                texPath = apparelDef.texPathBackLayer;
            if (string.IsNullOrEmpty(texPath))
                return;
            this.graphic = GraphicDatabase.Get<Graphic_Multi>(texPath, shader, Vector2.one, ColorFor(_pawn));
        }
    }
}