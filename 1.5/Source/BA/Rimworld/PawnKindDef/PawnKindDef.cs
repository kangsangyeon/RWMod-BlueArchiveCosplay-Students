using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace BA
{
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
        public HaloDef haloDef;
        public Vector3 haloOffset;

        // XML 내 <traits> 태그와 매핑할 리스트
        public List<TraitDef> traits = new List<TraitDef>();

        // Biotech Gene 추가: XML <genes> 태그와 매핑
        public List<GeneDef> genes = new List<GeneDef>();

        // healthScale 추가 (체력 배수용)
        public float healthScale = 1f;
    }
}