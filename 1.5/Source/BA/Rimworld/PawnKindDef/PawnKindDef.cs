using Verse;
using RimWorld;
using BA;  // 반드시 BA 네임스페이스 포함
using System.Collections.Generic;

namespace BA
{
    public class PawnKindDef : Verse.PawnKindDef
    {
        public int studentId;
        public RimWorld.BodyTypeDef forcedBodyType;
        public RimWorld.BodyTypeDef forcedBodyTypeFemale;
        public Verse.HeadTypeDef forcedHeadType;
        public Verse.HeadTypeDef forcedHeadTypeFemale;
        public FaceDef faceDef;
        public EyebrowDef eyebrowDef;
        public WingDef wingDef;
        public TailDef tailDef;
        public EarDef earDef;
        public HornDef hornDef;

        // 주의: HaloDef 클래스를 반드시 별도로 정의해야 하며, 네임스페이스가 같아야 합니다.
        public HaloDef haloDef;

        // TraitDef 리스트 필드 추가 (XML <traits> 태그 매핑용)
        public List<TraitDef> traits = new List<TraitDef>();
    }
}
