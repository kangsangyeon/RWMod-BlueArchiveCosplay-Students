using Verse;

namespace BlueArchiveStudents
{
    public static class TestWeightTable
    {
        [DebugAction("BlueArchiveStudents", "Test WeightTable::GetRandomItem",
            false, false, false, 0, false)]
        public static void TestGetRandomItem()
        {
            var _item = WeightTables.GetRandomItem("table_character-gacha") as CharacterTableItem;
            Log.Message($"random item: {_item.pawnKindTemplateDef.defName} (weight: {_item.weight})");
        }
    }
}