using RimWorld;
using Verse;

namespace BA
{
    // public class BAPawnComp : ThingComp
    // {
    //     public const int k_MaxLevel = 85;
    //     public int StudentId;
    //     public int Level = -1;
    //     public int Exp = 0;
    //     public int ExpTick = 0;
    //
    //     public StudentAttribute Attribute => GameResource.StudentTable[StudentId].Attribute;
    //
    //     public int RequiredExp
    //     {
    //         get
    //         {
    //             if (Level >= k_MaxLevel)
    //                 return 0;
    //             return GameResource.StudentLevelRequiredExpTable[Level + 1].Value;
    //         }
    //     }
    //
    //     public Pawn Owner => parent as Pawn;
    //
    //     public override void PostSpawnSetup(bool respawningAfterLoad)
    //     {
    //         base.PostSpawnSetup(respawningAfterLoad);
    //         // var label = Owner.kindDef.label;
    //         // var id = label.Substring(label.LastIndexOf('(') + 1, 4);
    //         // StudentId = int.Parse(id);
    //         Level = 1;
    //         Exp = 0;
    //         ExpTick = 0;
    //     }
    //
    //     public override void PostExposeData()
    //     {
    //         base.PostExposeData();
    //         Scribe_Values.Look(ref StudentId, "BA_StudentId", forceSave: true);
    //         Scribe_Values.Look(ref Level, "BA_Level", forceSave: true);
    //         Scribe_Values.Look(ref Exp, "BA_Exp", forceSave: true);
    //     }
    //
    //     public void LevelUp()
    //     {
    //         while (Exp > RequiredExp && Level < k_MaxLevel)
    //         {
    //             Exp -= RequiredExp;
    //             ++Level;
    //         }
    //
    //         if (Level >= k_MaxLevel)
    //         {
    //             Exp = 0;
    //         }
    //     }
    //
    //     public override void CompTick()
    //     {
    //         base.CompTick();
    //         // if (!parent.Spawned)
    //         //     return;
    //         // Log.Message(
    //         //     $"BAPawnComp::CompTick() : id: {StudentId}, ageTracker.CurLifeStage.factor: {Owner.ageTracker.CurLifeStage.meleeDamageFactor}, GetStatValue({Owner.GetStatValue(StatDefOf.MeleeDamageFactor)})");
    //     }
    // }
}