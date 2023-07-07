using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace BlueArchiveStudents
{
    public class PawnKindTemplateDef
    {
        [MustTranslate] public string firstName;
        [MustTranslate] public string lastName;
        [MustTranslate] public string nickname;

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

        /// <summary>
        /// 이 pawn의 종족입니다.
        /// </summary>
        public ThingDef race;

        /// <summary>
        /// 이 타입의 pawn이 스폰될 때 기본적으로 장착될 무기입니다.
        /// </summary>
        public ThingDef weaponDef;

        /// <summary>
        /// 이 타입의 pawn이 스폰될 때 기본적으로 장착될 의상입니다.
        /// 목록 중 하나의 의상이 입혀집니다.
        /// </summary>
        public List<ThingDef> apparels;

        public void Fill(Pawn _pawn)
        {
            _pawn.needs.food.CurLevel = _pawn.needs.food.MaxLevel;

            _pawn.Name = (Name)new NameTriple(this.firstName, this.nickname, this.lastName);
            _pawn.story.Childhood = this.childHood;
            _pawn.story.Adulthood = this.age < 20 ? (BackstoryDef)null : this.adultHood;
            _pawn.ageTracker.AgeBiologicalTicks = (long)this.age * 3600000L;
            _pawn.ageTracker.AgeChronologicalTicks = (long)this.realAge * 3600000L;
            _pawn.gender = this.isMale ? Gender.Male : Gender.Female;
            _pawn.story.bodyType = this.bodyTypeDef;
            _pawn.story.headType = this.headTypeDef ?? DefDatabase<HeadTypeDef>.GetNamed("Female_NarrowPointy");
            _pawn.story.hairDef = this.hair == null ? HairDefOf.Bald : this.hair;
            _pawn.style.beardDef = this.beard == null ? BeardDefOf.NoBeard : this.beard;
            _pawn.story.skinColorOverride = this.skinColor;
            _pawn.story.HairColor = this.hairColor;
            _pawn.Notify_DisabledWorkTypesChanged();
        }
    }
}