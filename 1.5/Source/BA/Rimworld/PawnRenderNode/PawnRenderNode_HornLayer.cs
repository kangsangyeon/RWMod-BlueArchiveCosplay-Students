using UnityEngine;
using Verse;

namespace BA
{
    public class PawnRenderNode_HornLayer : PawnRenderNode
    {
        private readonly Pawn _pawn;

        public PawnRenderNode_HornLayer(
            Pawn pawn,
            PawnRenderNodeProperties props,
            PawnRenderTree tree)
            : base(pawn, props, tree)
        {
            _pawn = pawn;
        }

        // public override GraphicMeshSet MeshSetFor(Pawn pawn)
        // {
        //     // Log.Message($"{nameof(PawnRenderNode_FrontHorn)}::{nameof(MeshSetFor)}");
        //     if (pawn.DevelopmentalStage.Baby() || pawn.DevelopmentalStage.Newborn())
        //         return null;
        //     if (pawn.kindDef is not BA.PawnKindDef kindDef)
        //         return null;
        //     if (kindDef.hornDef == null || kindDef.hornDef.noGraphic)
        //         return null;
        //     return HumanlikeMeshPoolUtility.GetHumanlikeBodySetForPawn(pawn);
        // }
        //
        // public override Graphic GraphicFor(Pawn pawn)
        // {
        //     // Log.Message($"{nameof(PawnRenderNode_FrontHorn)}::{nameof(GraphicFor)}");
        //     if (pawn.DevelopmentalStage.Baby() || pawn.DevelopmentalStage.Newborn())
        //         return null;
        //     if (pawn.kindDef is not BA.PawnKindDef kindDef)
        //         return null;
        //     if (kindDef.hornDef == null || kindDef.hornDef.noGraphic)
        //         return null;
        //     var shader = kindDef.hornDef.overrideShaderTypeDef?.Shader ?? ShaderDatabase.CutoutHair;
        //     return GraphicDatabase.Get<Graphic_Multi>(kindDef.hornDef.texPathFront, shader, Vector2.one, ColorFor(pawn));
        // }

        protected override void EnsureMaterialsInitialized()
        {
            if (_pawn.DevelopmentalStage.Baby() || _pawn.DevelopmentalStage.Newborn())
                return;
            if (_pawn.kindDef is not BA.PawnKindDef kindDef)
                return;
            if (kindDef.hornDef == null || kindDef.hornDef.noGraphic)
                return;
            var shader = kindDef.hornDef.overrideShaderTypeDef?.Shader ?? ShaderDatabase.CutoutHair;
            var texPath = kindDef.hornDef.texPath;
            if (string.IsNullOrEmpty(texPath))
                return;
            this.graphic = GraphicDatabase.Get<Graphic_Multi>(texPath, shader, Vector2.one, ColorFor(_pawn));
        }
    }
}