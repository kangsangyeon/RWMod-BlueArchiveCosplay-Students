using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BA
{
    public class CharacterTableDef : Def, IWeightTable
    {
        List<IWeightTableItem> IWeightTable.Items =>
            Items.Select(x => x as IWeightTableItem).ToList();

        public List<CharacterTableItem> Items;
    }

    public class CharacterTableItem : IWeightTableItem
    {
        float IWeightTableItem.Weight => Weight;

        public PawnKindTemplateDef PawnKindTemplateDef;
        public int Weight;
    }
}