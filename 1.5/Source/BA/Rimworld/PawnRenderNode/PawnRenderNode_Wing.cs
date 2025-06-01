using UnityEngine;
using Verse;

namespace BA
{
    public class PawnRenderNode_Wing : PawnRenderNode
    {
        private readonly Pawn _pawn;

        public PawnRenderNode_Wing(
            Pawn pawn,
            PawnRenderNodeProperties props,
            PawnRenderTree tree)
            : base(pawn, props, tree)
        {
            _pawn = pawn;
        }

        // public override GraphicMeshSet MeshSetFor(Pawn pawn)
        // {
        //     // Log.Message($"{nameof(PawnRenderNode_Wing)}::{nameof(MeshSetFor)}");
        //     if (pawn.DevelopmentalStage.Baby() || pawn.DevelopmentalStage.Newborn())
        //         return null;
        //     if (pawn.kindDef is not BA.PawnKindDef kindDef)
        //         return null;
        //     if (kindDef.wingDef == null || kindDef.wingDef.noGraphic)
        //         return null;
        //     return HumanlikeMeshPoolUtility.GetHumanlikeBodySetForPawn(pawn);
        // }
        //
        // public override Graphic GraphicFor(Pawn pawn)
        // {
        //     // Log.Message($"{nameof(PawnRenderNode_Wing)}::{nameof(GraphicFor)}");
        //     if (pawn.DevelopmentalStage.Baby() || pawn.DevelopmentalStage.Newborn())
        //         return null;
        //     if (pawn.kindDef is not BA.PawnKindDef kindDef)
        //         return null;
        //     if (kindDef.wingDef == null || kindDef.wingDef.noGraphic)
        //         return null;
        //     var shader = kindDef.wingDef.overrideShaderTypeDef?.Shader ?? ShaderDatabase.CutoutHair;
        //     return GraphicDatabase.Get<Graphic_Multi>(kindDef.wingDef.texPath, shader, Vector2.one, ColorFor(pawn));
        // }

        protected override void EnsureMaterialsInitialized()
        {
            if (_pawn.DevelopmentalStage.Baby() || _pawn.DevelopmentalStage.Newborn())
                return;
            if (_pawn.kindDef is not BA.PawnKindDef kindDef)
                return;
            if (kindDef.wingDef == null || kindDef.wingDef.noGraphic)
                return;
            var shader = kindDef.wingDef.overrideShaderTypeDef?.Shader ?? ShaderDatabase.CutoutHair;
            this.graphic = GraphicDatabase.Get<Graphic_Multi>(kindDef.wingDef.texPath, shader, Vector2.one, ColorFor(_pawn));
        }
    }
}