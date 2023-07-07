using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace BlueArchiveStudents
{
    public class PawnKindTemplateDef
    {
        public static bool currentlyGenerating =>
            operator_Pawn != null;

        protected static Pawn operator_Pawn;

        [MustTranslate] public string firstName;
        [MustTranslate] public string lastName;
        [MustTranslate] public string nickname;

        /// <summary>
        /// 이 pawn의 종족입니다.
        /// </summary>
        public ThingDef race;

        /// <summary>
        /// 이 pawn의 기본 faction(파벌)입니다.
        /// </summary>
        public FactionDef defaultFactionType;

        public BackstoryDef childHood;
        public BackstoryDef adultHood;

        /// <summary>
        /// 이 타입의 pawn에 부여되는 기본 생체 나이입니다.
        /// </summary>
        public int age = 16;

        /// <summary>
        /// 이 타입의 pawn에 부여되는 실제 인게임 나이입니다.
        /// </summary>
        public int realAge = -1;

        /// <summary>
        /// 이 타입의 pawn이 남성인지 또는 여성인지 구분합니다.
        /// </summary>
        public bool isMale = false;

        /// <summary>
        /// 이 타입의 pawn이 스폰될 때 기본적으로 장착될 무기입니다.
        /// </summary>
        public ThingDef weaponDef;

        /// <summary>
        /// 이 타입의 pawn이 스폰될 때 기본적으로 장착될 의상입니다.
        /// 목록 중 하나의 의상이 입혀집니다.
        /// </summary>
        public List<ThingDef> apparels;

        /// <summary>
        /// 이 타입의 pawn의 체형 타입입니다.
        /// </summary>
        public BodyTypeDef bodyTypeDef;

        /// <summary>
        /// 이 타입의 pawn의 머리 타입입니다.
        /// </summary>
        public HeadTypeDef headTypeDef;

        /// <summary>
        /// 이 타입의 pawn의 머리털 타입입니다.
        /// </summary>
        public HairDef hair;

        /// <summary>
        /// 이 타입의 pawn의 수염 타입입니다.
        /// </summary>
        public BeardDef beard;

        public Color skinColor = Color.white;
        public Color hairColor = Color.white;
    }
}