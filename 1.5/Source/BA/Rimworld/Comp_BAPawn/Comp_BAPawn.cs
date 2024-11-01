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

        public StudentAttribute Attribute => GameResource.StudentTable[StudentId].Attribute;
        public Pawn Owner => parent as Pawn;

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
            }
        }

        public override void CompTick()
        {
            if (!parent.Spawned)
                return;
            if (!parent.IsHashIntervalTick(Const.TickPerSecond)) // 1초마다 tick 실행
                return;
            Log.Message(
                $"BAPawnComp::CompTick() : id: {StudentId}, ageTracker.CurLifeStage.factor: {Owner.ageTracker.CurLifeStage.meleeDamageFactor}, GetStatValue({Owner.GetStatValue(StatDefOf.MeleeDamageFactor)})");
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
                UpdateRequiredExp();
                Log.Message($"Level Up! {Level}");
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
    }
}