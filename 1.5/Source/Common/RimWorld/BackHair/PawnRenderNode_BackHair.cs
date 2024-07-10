using UnityEngine;
using Verse;

namespace BlueArchiveStudents;

public class PawnRenderNode_BackHair : PawnRenderNode
{
    public PawnRenderNode_BackHair(Pawn _pawn, PawnRenderNodeProperties _props, PawnRenderTree _tree)
        : base(_pawn, _props, _tree)
    {
    }

    public override GraphicMeshSet MeshSetFor(Pawn _pawn)
    {
        if (_pawn.story?.hairDef == null || _pawn.story.hairDef.noGraphic)
            return null;
        if (_pawn.story?.hairDef is not BA_HairDef)
            return null;
        return HumanlikeMeshPoolUtility.GetHumanlikeHairSetForPawn(_pawn);
    }

    public override Graphic GraphicFor(Pawn _pawn)
    {
        if (_pawn.story?.hairDef == null || _pawn.story.hairDef.noGraphic || _pawn.DevelopmentalStage.Baby() || _pawn.DevelopmentalStage.Newborn())
            return null;
        var _baHairDef = _pawn.story?.hairDef as BA_HairDef;
        if (_baHairDef == null)
            return null;
        var _shader = _baHairDef.overrideShaderTypeDef?.Shader ?? ShaderDatabase.CutoutHair;
        return GraphicDatabase.Get<Graphic_Multi>(_baHairDef.texPathBackHair, _shader, Vector2.one, ColorFor(_pawn));
    }
}