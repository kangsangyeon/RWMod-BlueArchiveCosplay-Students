using LudeonTK;
using Verse;

namespace BA
{
    public static class WeightTableDevelopment
    {
        [DebugAction(Const.DebugActionCategory, nameof(WeightTableDevelopment) + "::" + nameof(TestGetRandomItem),
            false, false, false, false,
            0, false)]
        public static void TestGetRandomItem()
        {
            var item = WeightTables.GetRandomItem("table_character-gacha") as CharacterTableItem;
            Log.Message($"random item: {item.PawnKindTemplateDef.defName} (weight: {item.Weight})");
        }
    }
}