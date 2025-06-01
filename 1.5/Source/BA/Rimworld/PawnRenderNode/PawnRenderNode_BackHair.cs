using UnityEngine;
using Verse;

namespace BA
{
    public class PawnRenderNode_BackHair : PawnRenderNode
    {
        private readonly Pawn _pawn;

        public PawnRenderNode_BackHair(
            Pawn pawn,
            PawnRenderNodeProperties props,
            PawnRenderTree tree)
            : base(pawn, props, tree)
        {
            _pawn = pawn;
        }

        // public override GraphicMeshSet MeshSetFor(Pawn pawn)
        // {
        //     // Log.Message($"{nameof(PawnRenderNode_BackHair)}::{nameof(MeshSetFor)}");
        //     if (pawn.story?.hairDef == null || pawn.story.hairDef.noGraphic)
        //         return null;
        //     if (pawn.story?.hairDef is not HairDef hairDef)
        //         return null;
        //     if (string.IsNullOrEmpty(hairDef.texPathBackHair))
        //         return null;
        //     return HumanlikeMeshPoolUtility.GetHumanlikeHairSetForPawn(pawn);
        // }
        //
        // public override Graphic GraphicFor(Pawn pawn)
        // {
        //     // Log.Message($"{nameof(PawnRenderNode_BackHair)}::{nameof(GraphicFor)}");
        //     if (pawn.DevelopmentalStage.Baby() || pawn.DevelopmentalStage.Newborn())
        //         return null;
        //     if (pawn.story?.hairDef == null || pawn.story.hairDef.noGraphic)
        //         return null;
        //     if (pawn.story?.hairDef is not BA.HairDef hairDef)
        //         return null;
        //     if (string.IsNullOrEmpty(hairDef.texPathBackHair))
        //         return null;
        //     var shader = hairDef.overrideShaderTypeDef?.Shader ?? ShaderDatabase.CutoutHair;
        //     return GraphicDatabase.Get<Graphic_Multi>(hairDef.texPathBackHair, shader, Vector2.one, ColorFor(pawn));
        // }

        protected override void EnsureMaterialsInitialized()
        {
            if (_pawn.DevelopmentalStage.Baby() || _pawn.DevelopmentalStage.Newborn())
                return;
            if (_pawn.story?.hairDef == null || _pawn.story.hairDef.noGraphic)
                return;
            if (_pawn.story?.hairDef is not BA.HairDef hairDef)
                return;
            if (string.IsNullOrEmpty(hairDef.texPathBackHair))
                return;
            var shader = hairDef.overrideShaderTypeDef?.Shader ?? ShaderDatabase.CutoutHair;
            this.graphic = GraphicDatabase.Get<Graphic_Multi>(hairDef.texPathBackHair, shader, Vector2.one, ColorFor(_pawn));
        }
    }
}