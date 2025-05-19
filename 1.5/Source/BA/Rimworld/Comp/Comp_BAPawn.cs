using System.Collections.Generic;
using System.Text.RegularExpressions;
using RimWorld;
using UnityEngine;
using Verse;

namespace BA
{
    public class CompProperties_BAPawn : CompProperties
    {
        public CompProperties_BAPawn() => this.compClass = typeof(Comp_BAPawn);
    }

    public class Comp_BAPawn : ThingComp
    {
        public static int Tick => GameResource.Const.PawnCompSettings.Tick;
        public static int GainExpPerTick => GameResource.Const.PawnCompSettings.GainExpPerTick;
        public static float GainExpBonusThreshold => GameResource.Const.PawnCompSettings.GainExpBonusThreshold;
        public static float GainExpBonusMultiplier => GameResource.Const.PawnCompSettings.GainExpBonusMultiplier;
        public static float GainExpDayLimitCount => GameResource.Const.PawnCompSettings.GainExpDayLimitCount;
        public static float GainExpDayLimitMultiplier => GameResource.Const.PawnCompSettings.GainExpDayLimitMultiplier;

        public int StudentId;
        public int Shinbi;
        public int RequiredExp = int.MaxValue;

        private int _level;
        private int _exp;
        private Dictionary<SkillDef, int> _gainExpDayCount = new();
        private int _lastDayLimitResetTimestamp = -1;

        public int Level
        {
            get => _level;
            set
            {
                if (_level == value)
                    return;
                _level = value;
                TryLevelUpTick();
                ModifySaveData();
            }
        }

        public int Exp
        {
            get => _exp;
            set
            {
                if (_exp == value)
                    return;
                _exp = value;
                TryLevelUpTick();
                ModifySaveData();
            }
        }

        public int LevelLimit =>
            GameResource.ShinbiTable[Shinbi].StudentLevelLimit;

        public StudentData StudentData => GameResource.StudentTable[StudentId];
        public Pawn Owner => parent as Pawn;
        public BA.PawnKindDef OwnerPawnKindDef => Owner.kindDef as BA.PawnKindDef;
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
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref StudentId, $"BA_{nameof(StudentId)}", forceSave: true);
            Scribe_Values.Look(ref Shinbi, $"BA_{nameof(Shinbi)}", forceSave: true);
            Scribe_Values.Look(ref RequiredExp, $"BA_{nameof(RequiredExp)}", forceSave: true);
            Scribe_Values.Look(ref _level, $"BA_{nameof(_level)}", forceSave: true);
            Scribe_Values.Look(ref _exp, $"BA_{nameof(_exp)}", forceSave: true);
            Scribe_Values.Look(ref _lastDayLimitResetTimestamp, $"BA_{nameof(_lastDayLimitResetTimestamp)}");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (respawningAfterLoad)
            {
            }
            else
            {
                StudentId = OwnerPawnKindDef.studentId;
                Shinbi = StudentData.DefaultStar;
                Level = 1;
                Exp = 0;
                UpdateRequiredExp();
                _lastDayLimitResetTimestamp = Find.TickManager.TicksGame;
                ModifySaveData();

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
            ApplySkillLevelCap();
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

        private void ApplySkillLevelCap()
        {
            // 3성 15
            // 4성 17
            // 5성 20 
            // 이하로는 3성의 제한을 따라 갈 것

            var cap = 0;
            if (StudentData.DefaultStar == 5)
                cap = 20;
            else if (StudentData.DefaultStar == 4)
                cap = 17;
            else
                cap = 15;

            ShootingSkillRecord.Level = Mathf.Clamp(ShootingSkillRecord.Level, 0, cap);
            MeleeSkillRecord.Level = Mathf.Clamp(MeleeSkillRecord.Level, 0, cap);
            ConstructionSkillRecord.Level = Mathf.Clamp(ConstructionSkillRecord.Level, 0, cap);
            MiningSkillRecord.Level = Mathf.Clamp(MiningSkillRecord.Level, 0, cap);
            CookingSkillRecord.Level = Mathf.Clamp(CookingSkillRecord.Level, 0, cap);
            PlantsSkillRecord.Level = Mathf.Clamp(PlantsSkillRecord.Level, 0, cap);
            AnimalsSkillRecord.Level = Mathf.Clamp(AnimalsSkillRecord.Level, 0, cap);
            CraftingSkillRecord.Level = Mathf.Clamp(CraftingSkillRecord.Level, 0, cap);
            ArtisticSkillRecord.Level = Mathf.Clamp(ArtisticSkillRecord.Level, 0, cap);
            MedicalSkillRecord.Level = Mathf.Clamp(MedicalSkillRecord.Level, 0, cap);
            SocialSkillRecord.Level = Mathf.Clamp(SocialSkillRecord.Level, 0, cap);
            IntellectualSkillRecord.Level = Mathf.Clamp(IntellectualSkillRecord.Level, 0, cap);
        }

        private void TryLevelUpTick()
        {
            UpdateRequiredExp();

            while (_exp >= RequiredExp && Level < LevelLimit)
            {
                _exp -= RequiredExp;
                ++Level;
                TryLevelUpSkills();
                UpdateRequiredExp();
            }

            if (Level >= LevelLimit)
            {
                _exp = 0;
            }
        }

        private void GainExpTick()
        {
            Exp += GameResource.Const.PawnCompSettings.GainExpPerTick;
            ModifySaveData();
        }

        private void UpdateRequiredExp()
        {
            RequiredExp = GetRequiredExpAt(Level);
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
            ApplySkillLevelCap();
        }

        private void ModifySaveData()
        {
            GameResource.Save.StudentSaveData[StudentId].Unlock = true;
            GameResource.Save.StudentSaveData[StudentId].Level = Level;
            GameResource.Save.StudentSaveData[StudentId].Exp = Exp;
            GameResource.Save.StudentSaveData[StudentId].Shinbi = Shinbi;
        }

        private int GetRequiredExpAt(int level)
        {
            if (level < 0 || level >= LevelLimit)
                return 0;
            return GameResource.StudentLevelRequiredExpTable[Level].Value;
        }
    }
}