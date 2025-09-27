using UnityEngine;
using Verse;

namespace BA
{
    public class PawnRenderNode_Halo : PawnRenderNode
    {
        private readonly Pawn _pawn;

        public PawnRenderNode_Halo(
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
            if (kindDef.haloDef == null || kindDef.haloDef.noGraphic)
                return;
            if (props is not BA.PawnRenderNodeProperties_Halo nodeProps)
                return;
            var shader = kindDef.haloDef.overrideShaderTypeDef?.Shader ?? ShaderDatabase.CutoutHair;
            this.graphic = GraphicDatabase.Get<Graphic_Multi>(kindDef.haloDef.texPath, shader, Vector2.one, ColorFor(_pawn));
        }
    }
}