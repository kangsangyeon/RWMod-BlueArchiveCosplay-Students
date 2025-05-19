using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BA
{
    public class BAUtil
    {
        public static bool IsBAPawn(Pawn pawn, out Comp_BAPawn comp)
        {
            comp = null;
            if (pawn == null)
                return false;
            if (pawn.kindDef.race.race.Humanlike == false) // 사람이 아니면 건너뜀
                return false;
            if (pawn.kindDef.race.race.Animal) // 동물이면 건너뜀
                return false;
            if (pawn.kindDef.defName.StartsWith("BA") == false) // 우리 프로젝트에서 정의한 pawnkind 아니면 건너뜀
                return false;
            comp = pawn.GetComp<Comp_BAPawn>();
            return true;
        }

        private static bool IsPlayerCaravan(WorldObject obj)
        {
            return obj is Caravan caravan && caravan.Faction == Faction.OfPlayer;
        }

        private static IEnumerable<Thing> GetTotalEnumerableOf(ThingDef def)
        {
            var inMaps =
                Find.Maps.SelectMany(x => x.listerThings.ThingsOfDef(def));
            var inCaravans =
                Find.WorldObjects.AllWorldObjects
                    .Where(IsPlayerCaravan)
                    .SelectMany(x => (x as Caravan).AllThings.Where(y => y.def == def));
            return inMaps.Concat(inCaravans);
        }

        private static int GetTotalOf(ThingDef def)
        {
            var enumerable = GetTotalEnumerableOf(def);
            return enumerable.Sum(x => x.stackCount);
        }

        private static bool TryConsumeOf(ThingDef def, int value)
        {
            var things = GetTotalEnumerableOf(def).ToArray();
            var total = things.Sum(x => x.stackCount);
            if (total < value)
                return false;

            var remaining = value;
            foreach (var t in things)
            {
                if (remaining <= 0) break;

                if (t.stackCount <= remaining)
                {
                    remaining -= t.stackCount;
                    t.Destroy();
                }
                else
                {
                    // 일부만 떼어내서 소모함
                    var part = t.SplitOff(remaining);
                    part.Destroy();
                    remaining = 0;
                }
            }

            return true;
        }

        public static int GetTotalEligma()
        {
            return GetTotalOf(BADefOf.BA_Item_Eligma);
        }

        public static bool TryConsumeEligma(int value)
        {
            return TryConsumeOf(BADefOf.BA_Item_Eligma, value);
        }

        public static int GetTotalSilver()
        {
            return GetTotalOf(ThingDefOf.Silver);
        }

        public static bool TryConsumeSilver(int value)
        {
            return TryConsumeOf(ThingDefOf.Silver, value);
        }

        public static Pawn GetFirstPawnOf(PawnKindDef def)
        {
            return Current.Game.World.worldPawns.AllPawnsAliveOrDead
                .First(p => p.kindDef == def);
        }

        public static Pawn GetFirstPawnOf(int studentId)
        {
            return GetFirstPawnOf(BADefOfCollections.PawnKindDefs[studentId]);
        }
    }
}