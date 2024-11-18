using System.Collections.Generic;
using System.Text.RegularExpressions;
using RimWorld;
using UnityEngine;
using Verse;

namespace BA
{
    public class Comp_BAPawn : ThingComp
    {
        public static int Tick => GameResource.Const.PawnCompSettings.Tick;
        public static int GainExpPerTick => GameResource.Const.PawnCompSettings.GainExpPerTick;
        public static float GainExpBonusThreshold => GameResource.Const.PawnCompSettings.GainExpBonusThreshold;
        public static float GainExpBonusMultiplier => GameResource.Const.PawnCompSettings.GainExpBonusMultiplier;
        public static float GainExpDayLimitCount => GameResource.Const.PawnCompSettings.GainExpDayLimitCount;
        public static float GainExpDayLimitMultiplier => GameResource.Const.PawnCompSettings.GainExpDayLimitMultiplier;

        public int StudentId;
        public int Level = -1;
        public int RequiredExp = int.MaxValue;

        private int _exp;
        private Dictionary<SkillDef, int> _gainExpDayCount = new();
        private int _lastDayLimitResetTimestamp = -1;

        public int Exp
        {
            get => _exp;
            set
            {
                if (_exp == value)
                    return;
                _exp = value;
                TryLevelUpTick();
            }
        }

        public StudentData StudentData => GameResource.StudentTable[StudentId];
        public Pawn Owner => parent as Pawn;
        public SkillRecord ShootingSkillRecord => Owner.skills.GetSkill(SkillDefOf.Shooting);
        public SkillRecord MeleeSkillRecord => Owner.skills.GetSkill(SkillDefOf.Melee);
        public SkillRecord ConstructionSkillRecord => Owner.skills.GetSkill(SkillDefOf.Construction);
        public SkillRecord MiningSkillRecord => Owner.skills.GetSkill(SkillDefOf.Mining);
        public SkillRecord CookingSkillRecord => Owner.skills.GetSkill(SkillDefOf.Cooking);
        public SkillRecord PlantsSkillRecord => Owner.skills.GetSkill(SkillDefOf.Plants);
        public SkillRecord AnimalsSkillRecord => Owner.skills.GetSkill(SkillDefOf.Animals);
        public SkillRecord CraftingSkillRecord => Owner.skills.GetSkill(SkillDefOf.Crafting);
        public SkillRecord ArtisticSkillRecord => Owner.skills.GetSkill(SkillDefOf.Artistic);
        public SkillRecord MedicalSkillRecord => Owner.skills.GetSkill(SkillDefOf.Medicine);
        public SkillRecord SocialSkillRecord => Owner.skills.GetSkill(SkillDefOf.Social);
        public SkillRecord IntellectualSkillRecord => Owner.skills.GetSkill(SkillDefOf.Intellectual);

        public void OnSkillTrackerLearn(SkillDef sDef, float xp)
        {
            if (!_gainExpDayCount.TryGetValue(sDef, out var dayCount))
                _gainExpDayCount[sDef] = 0;
            _gainExpDayCount[sDef] = ++dayCount;

            Log.Message($"{GainExpBonusThreshold}, {GainExpBonusMultiplier}, {GainExpDayLimitCount}, {GainExpDayLimitMultiplier}");

            // 예: threshold가 0.5로 설정된 상태에서 mood가 0.75이면 delta는 0.5가 됨.
            // 예2: threshold가 0.4로 설정된 상태에서 mood가 0.4이면 delta는 0이 됨.
            float bonusDelta =
                (Owner.needs.mood.CurLevel - GainExpBonusThreshold) / (1 - GainExpBonusThreshold);
            bonusDelta = Mathf.Clamp01(bonusDelta);
            float bonusMultiplier =
                Mathf.Lerp(1f, GainExpBonusMultiplier, bonusDelta);
            float dayLimitMultiplier = dayCount > GainExpDayLimitCount ? GainExpDayLimitMultiplier : 1f; // 하루 경험치 제한 횟수보다 카운팅이 높아지면 곱 적용
            float exp = xp * bonusMultiplier * dayLimitMultiplier;
            Exp += Mathf.RoundToInt(exp);
            Log.Message($"{bonusDelta}, {dayCount}, {xp}*{bonusMultiplier}*{dayLimitMultiplier}={exp}");
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref StudentId, $"BA_{nameof(StudentId)}", forceSave: true);
            Scribe_Values.Look(ref Level, $"BA_{nameof(Level)}", forceSave: true);
            Scribe_Values.Look(ref _exp, $"BA_{nameof(_exp)}", forceSave: true);
            Scribe_Values.Look(ref RequiredExp, $"BA_{nameof(RequiredExp)}", forceSave: true);
            Scribe_Values.Look(ref _lastDayLimitResetTimestamp, $"BA_{nameof(_lastDayLimitResetTimestamp)}");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (respawningAfterLoad)
            {
            }
            else
            {
                Match match = Regex.Match(Owner.kindDef.label, @"\((\d+)\)$");
                StudentId = int.Parse(match.Groups[1].Value);
                Level = 1;
                Exp = 0;
                UpdateRequiredExp();

                // 스킬 레벨 초기화
                ShootingSkillRecord.Level = StudentData.DefaultShooting;
                MeleeSkillRecord.Level = StudentData.DefaultMelee;
                ConstructionSkillRecord.Level = StudentData.DefaultConstruction;
                MiningSkillRecord.Level = StudentData.DefaultMining;
                CookingSkillRecord.Level = StudentData.DefaultCooking;
                PlantsSkillRecord.Level = StudentData.DefaultPlants;
                AnimalsSkillRecord.Level = StudentData.DefaultAnimals;
                CraftingSkillRecord.Level = StudentData.DefaultCrafting;
                ArtisticSkillRecord.Level = StudentData.DefaultArtistic;
                MedicalSkillRecord.Level = StudentData.DefaultMedical;
                SocialSkillRecord.Level = StudentData.DefaultSocial;
                IntellectualSkillRecord.Level = StudentData.DefaultIntellectual;

                // 스킬 경험치 배율(=passion) 축소
                ShootingSkillRecord.passion = Passion.None;
                MeleeSkillRecord.passion = Passion.None;
                ConstructionSkillRecord.passion = Passion.None;
                MiningSkillRecord.passion = Passion.None;
                CookingSkillRecord.passion = Passion.None;
                PlantsSkillRecord.passion = Passion.None;
                AnimalsSkillRecord.passion = Passion.None;
                CraftingSkillRecord.passion = Passion.None;
                ArtisticSkillRecord.passion = Passion.None;
                MedicalSkillRecord.passion = Passion.None;
                SocialSkillRecord.passion = Passion.None;
                IntellectualSkillRecord.passion = Passion.None;
            }
        }

        public override void CompTick()
        {
            if (!parent.Spawned)
                return;
            TryClearDayLimitTick();
            if (!parent.IsHashIntervalTick(GameResource.Const.PawnCompSettings.Tick)) // 1초마다 tick 실행
                return;
            // Log.Message(
            //     $"BAPawnComp::CompTick() : id: {StudentId}, ageTracker.CurLifeStage.factor: {Owner.ageTracker.CurLifeStage.meleeDamageFactor}, GetStatValue({Owner.GetStatValue(StatDefOf.MeleeDamageFactor)})");
            GainExpTick();
            TryLevelUpTick();
        }

        private void TryClearDayLimitTick()
        {
            if (GenLocalDate.HourInteger(Owner) == 0 &&
                (_lastDayLimitResetTimestamp < 0 || Find.TickManager.TicksGame - _lastDayLimitResetTimestamp >= 30000)) // 30000은 1일동안의 총 tick 수
            {
                // 날이 지나면 하루 경험치 제한 카운팅 값을 초기화
                foreach (var k in _gainExpDayCount.Keys)
                    _gainExpDayCount[k] = 0;
                _lastDayLimitResetTimestamp = Find.TickManager.TicksGame;
            }
        }

        private void TryLevelUpTick()
        {
            UpdateRequiredExp();

            while (Exp > RequiredExp && Level < Const.PawnMaxLevel)
            {
                Exp -= RequiredExp;
                ++Level;
                TryLevelUpSkills();
                UpdateRequiredExp();
            }

            if (Level >= Const.PawnMaxLevel)
            {
                Exp = 0;
            }
        }

        private void GainExpTick()
        {
            Exp += GameResource.Const.PawnCompSettings.GainExpPerTick;
        }

        private void UpdateRequiredExp()
        {
            if (Level < 0 || Level >= Const.PawnMaxLevel)
                RequiredExp = 0;
            else
                RequiredExp = GameResource.StudentLevelRequiredExpTable[Level + 1].Value;
        }

        private void TryLevelUpSkills()
        {
            var attributeLevelData = GameResource.StudentAttributeLevelTable[(StudentData.Attribute, Level)];
            ShootingSkillRecord.Level += attributeLevelData.Shooting;
            MeleeSkillRecord.Level += attributeLevelData.Melee;
            ConstructionSkillRecord.Level += attributeLevelData.Construction;
            MiningSkillRecord.Level += attributeLevelData.Mining;
            CookingSkillRecord.Level += attributeLevelData.Cooking;
            PlantsSkillRecord.Level += attributeLevelData.Plants;
            AnimalsSkillRecord.Level += attributeLevelData.Animals;
            CraftingSkillRecord.Level += attributeLevelData.Crafting;
            ArtisticSkillRecord.Level += attributeLevelData.Artistic;
            MedicalSkillRecord.Level += attributeLevelData.Medical;
            SocialSkillRecord.Level += attributeLevelData.Social;
            IntellectualSkillRecord.Level += attributeLevelData.Intellectual;
        }
    }
}