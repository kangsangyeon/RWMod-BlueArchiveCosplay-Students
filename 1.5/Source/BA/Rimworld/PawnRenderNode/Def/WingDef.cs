using UnityEngine;
using Verse;

namespace BA
{
    public class WingDef : RimWorld.StyleItemDef
    {
        public override Graphic GraphicFor(Pawn pawn, Color color)
        {
            return noGraphic ? null : GraphicDatabase.Get<Graphic_Multi>(texPath, overrideShaderTypeDef?.Shader ?? ShaderDatabase.CutoutHair, Vector2.one, color);
        }
    }
}