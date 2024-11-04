using System.Linq;
using System.Text.RegularExpressions;
using RimWorld;
using Verse;

namespace BA
{
    public class Comp_BAPawn : ThingComp
    {
        public int StudentId;
        public int Level = -1;
        public int Exp = 0;
        public int RequiredExp = int.MaxValue;

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

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref StudentId, "BA_StudentId", forceSave: true);
            Scribe_Values.Look(ref Level, "BA_Level", forceSave: true);
            Scribe_Values.Look(ref Exp, "BA_Exp", forceSave: true);
            Scribe_Values.Look(ref RequiredExp, "BA_RequiredExp", forceSave: true);
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
            if (!parent.IsHashIntervalTick(Const.TickPerSecond)) // 1초마다 tick 실행
                return;
            // Log.Message(
            //     $"BAPawnComp::CompTick() : id: {StudentId}, ageTracker.CurLifeStage.factor: {Owner.ageTracker.CurLifeStage.meleeDamageFactor}, GetStatValue({Owner.GetStatValue(StatDefOf.MeleeDamageFactor)})");
            GainExpTick();
            TryLevelUpTick();
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
            Exp += 1000; // temp: 임시적으로 틱당 1000 경험치 획득
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