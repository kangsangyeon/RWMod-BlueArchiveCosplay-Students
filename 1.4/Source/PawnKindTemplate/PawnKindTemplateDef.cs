using System.Collections.Generic;
using System.Reflection;
using AlienRace;
using RimWorld;
using UnityEngine;
using Verse;

namespace BlueArchiveStudents
{
    public class PawnKindTemplateDef : Def
    {
        /*[MustTranslate]*/
        public string firstName;

        /*[MustTranslate]*/
        public string lastName;

        /*[MustTranslate]*/
        public string nickname;

        public BackstoryDef childHood;

        public BackstoryDef adultHood;

        // /// <summary>
        // /// 이 타입의 pawn에 부여되는 기본 생체 나이입니다.
        // /// </summary>
        // public int age = 16;
        //
        // /// <summary>
        // /// 이 타입의 pawn에 부여되는 실제 인게임 나이입니다.
        // /// </summary>
        // public int realAge = -1;

        /// <summary>
        /// 이 타입의 pawn이 남성인지 또는 여성인지 구분합니다.
        /// </summary>
        public bool isMale = false;

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

        public bool overrideSkinColor;
        public bool overrideHairColor;

        public Color skinColor = Color.white;
        public Color hairColor = Color.white;

        /// <summary>
        /// 이 타입의 pawnkind입니다.
        /// </summary>
        public PawnKindDef pawnKindDef;

        /// <summary>
        /// 이 타입의 pawn이 스폰될 때 기본적으로 장착될 무기입니다.
        /// </summary>
        public ThingDef weaponDef;

        /// <summary>
        /// 이 타입의 pawn이 스폰될 때 기본적으로 장착될 의상 목록입니다.
        /// </summary>
        public List<ThingDef> apparels;
    }
}