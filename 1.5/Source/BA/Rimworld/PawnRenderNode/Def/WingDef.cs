using UnityEngine;
using Verse;

namespace BA
{
    public class WingDef : RimWorld.StyleItemDef
    {
        public string texPathRight; // 오른쪽 날개의 텍스쳐 경로임. null 또는 빈 문자열이면 기본 텍스쳐를 왼쪽 날개로 미러링함.

        public override Graphic GraphicFor(Pawn pawn, Color color)
        {
            return noGraphic ? null : GraphicDatabase.Get<Graphic_Multi>(texPath, overrideShaderTypeDef?.Shader ?? ShaderDatabase.CutoutHair, Vector2.one, color);
        }
    }
}