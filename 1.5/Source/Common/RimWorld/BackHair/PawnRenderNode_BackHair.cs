using UnityEngine;
using Verse;

namespace BA
{
    public class PawnRenderNode_BackHair : PawnRenderNode
    {
        public PawnRenderNode_BackHair(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
            : base(pawn, props, tree)
        {
        }

        public override GraphicMeshSet MeshSetFor(Pawn pawn)
        {
            if (pawn.story?.hairDef == null || pawn.story.hairDef.noGraphic)
                return null;
            if (pawn.story?.hairDef is not HairDef)
                return null;
            return HumanlikeMeshPoolUtility.GetHumanlikeHairSetForPawn(pawn);
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            if (pawn.story?.hairDef == null || pawn.story.hairDef.noGraphic || pawn.DevelopmentalStage.Baby() || pawn.DevelopmentalStage.Newborn())
                return null;
            var baHairDef = pawn.story?.hairDef as HairDef;
            if (baHairDef == null)
                return null;
            var shader = baHairDef.overrideShaderTypeDef?.Shader ?? ShaderDatabase.CutoutHair;
            return GraphicDatabase.Get<Graphic_Multi>(baHairDef.texPathBackHair, shader, Vector2.one, ColorFor(pawn));
        }
    }
}