using UnityEngine;
using Verse;

namespace BA
{
    public class PawnRenderNode_Eyebrow : PawnRenderNode
    {
        private readonly Pawn _pawn;

        public PawnRenderNode_Eyebrow(
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
            if (_pawn.kindDef is not BA.PawnKindDef kindDef)
                return;
            if (kindDef.eyebrowDef == null || kindDef.eyebrowDef.noGraphic)
                return;
            var shader = kindDef.eyebrowDef.overrideShaderTypeDef?.Shader ?? ShaderDatabase.CutoutHair;
            this.graphic = GraphicDatabase.Get<Graphic_Multi>(kindDef.eyebrowDef.texPath, shader, Vector2.one, ColorFor(_pawn));
        }
    }
}