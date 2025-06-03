using UnityEngine;
using Verse;

namespace BA
{
    public class PawnRenderNode_Face : PawnRenderNode
    {
        private readonly Pawn _pawn;

        public PawnRenderNode_Face(
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
            if (kindDef.faceDef == null || kindDef.faceDef.noGraphic)
                return;
            var shader = kindDef.faceDef.overrideShaderTypeDef?.Shader ?? ShaderDatabase.CutoutHair;
            this.graphic = GraphicDatabase.Get<Graphic_Multi>(kindDef.faceDef.texPath, shader, Vector2.one, ColorFor(_pawn));
        }
    }
}