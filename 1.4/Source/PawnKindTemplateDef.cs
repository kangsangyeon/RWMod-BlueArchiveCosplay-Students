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
        public PawnKindDef pawnKindDef = PawnKindDefOf.Colonist;

        /// <summary>
        /// 이 타입의 pawn이 스폰될 때 기본적으로 장착될 무기입니다.
        /// </summary>
        public ThingDef weaponDef;

        /// <summary>
        /// 이 타입의 pawn이 스폰될 때 기본적으로 장착될 의상 목록입니다.
        /// </summary>
        public List<ThingDef> apparels;

        public Pawn Spawn()
        {
            if (pawnKindDef == null)
            {
                // pawnKindDef 속성을 설정하지 않은 경우, 기본적으로 Colonist로 스폰합니다.
                pawnKindDef = PawnKindDefOf.Colonist;
            }

            Pawn _pawn = PawnGenerator.GeneratePawn(pawnKindDef, Faction.OfPlayer);
            _pawn.Name = (Name)new NameTriple(this.firstName, this.nickname, this.lastName);
            _pawn.needs.food.CurLevel = _pawn.needs.food.MaxLevel;
            _pawn.gender = this.isMale ? Gender.Male : Gender.Female;
            _pawn.story.Childhood = this.childHood ?? _pawn.story.Childhood;
            _pawn.story.Adulthood = this.adultHood ?? _pawn.story.Adulthood;
            _pawn.story.bodyType = this.bodyTypeDef ?? _pawn.story.bodyType;
            _pawn.story.headType = this.headTypeDef ?? _pawn.story.headType;
            _pawn.story.hairDef = this.hair ?? _pawn.story.hairDef;
            _pawn.style.beardDef = this.beard ?? _pawn.style.beardDef;

            // error 나이를 강제로 고치면 문제가 발생합니다.
            // _pawn.ageTracker.AgeBiologicalTicks = (long)this.age * 3600000L; // 1000시간당 나이 + 1, 1시간은 3600초
            // _pawn.ageTracker.AgeChronologicalTicks = (long)this.realAge * 3600000L;

            if (overrideSkinColor)
                _pawn.story.skinColorOverride = this.skinColor;

            if (overrideHairColor)
                _pawn.story.HairColor = this.hairColor;

            _pawn.Notify_DisabledWorkTypesChanged();

            if (bodyTypeDef != null)
                Verse.Log.Message($"bodyTypeDefName: {bodyTypeDef.defName}");

            if (headTypeDef != null)
                Verse.Log.Message($"headTypeDefName: {headTypeDef.defName}");

            if (hair != null)
                Verse.Log.Message($"hairDefName: {hair.defName}");

            return _pawn;
        }
    }
}