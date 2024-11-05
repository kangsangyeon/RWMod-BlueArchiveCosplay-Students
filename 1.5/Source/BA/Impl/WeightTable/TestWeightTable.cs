using LudeonTK;
using Verse;

namespace BA
{
    public static class TestWeightTable
    {
        [DebugAction("BlueArchiveStudents", "Test WeightTable::GetRandomItem",
            false, false, false, false,
            0, false)]
        public static void TestGetRandomItem()
        {
            var item = WeightTables.GetRandomItem("table_character-gacha") as CharacterTableItem;
            Log.Message($"random item: {item.pawnKindTemplateDef.defName} (weight: {item.weight})");
        }
    }
}