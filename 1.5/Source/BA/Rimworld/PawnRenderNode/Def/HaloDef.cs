using Verse;
using RimWorld;

namespace BA
{
    public class HaloDef : Def
    {
        public bool noGraphic = false;
        public ShaderTypeDef overrideShaderTypeDef;
        public string texPath; // 기존 필드

        public StyleItemCategoryDef category; // category 필드 추가 (XML <category>와 매칭)
    }
}
