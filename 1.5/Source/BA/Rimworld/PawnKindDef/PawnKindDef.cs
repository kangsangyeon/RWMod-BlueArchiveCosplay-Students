using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BA
{
    // Verse.PawnKindDef를 상속하여 확장된 커스텀 PawnKindDef (통합형)
    public class PawnKindDef : Verse.PawnKindDef
    {
        public int studentId;
        public BodyTypeDef forcedBodyType;
        public BodyTypeDef forcedBodyTypeFemale;
        public HeadTypeDef forcedHeadType;
        public HeadTypeDef forcedHeadTypeFemale;
        public FaceDef faceDef;
        public EyebrowDef eyebrowDef;
        public WingDef wingDef;
        public TailDef tailDef;
        public EarDef earDef;
        public HornDef hornDef;
        // HaloDef는 별도 정의 필요, 같은 네임스페이스 여야 함
        public HaloDef haloDef;
        // XML 내 <traits> 태그와 매핑할 리스트
        public List<TraitDef> traits = new List<TraitDef>();
        // healthScale 추가 (체력 배수용)
        public float healthScale = 1f;
    }
}
