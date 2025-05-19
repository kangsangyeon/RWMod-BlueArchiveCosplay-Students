using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BA
{
    [DefOf]
    public static class BADefOf
    {
        public static ThingDef BA_Item_Eligma;
        public static ThingDef BA_Item_Report_N;
        public static ThingDef BA_Item_Report_R;
        public static ThingDef BA_Item_Report_SR;
        public static ThingDef BA_Item_Report_UR;
        public static PawnKindDef BA_PawnKind_Makoto;
    }

    public static class BADefOfCollections
    {
        private static Dictionary<int, BA.PawnKindDef> _pawnKindDefs = null;

        public static Dictionary<int, BA.PawnKindDef> PawnKindDefs =>
            _pawnKindDefs ??= CreatePawnKindDefs();

        private static Dictionary<int, BA.PawnKindDef> CreatePawnKindDefs()
        {
            var result =
                DefDatabase<PawnKindDef>.AllDefsListForReading.ToDictionary(x => x.studentId);
            return result;
        }
    }
}