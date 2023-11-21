using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BlueArchiveStudents
{
    public class CharacterTableDef : Def, IWeightTable
    {
        public List<IWeightTableItem> Items =>
            items.Select(x => x as IWeightTableItem).ToList();

        public List<BlueArchiveStudents.CharacterTableItem> items;
    }

    public class CharacterTableItem : IWeightTableItem
    {
        public float Weight => weight;

        public PawnKindTemplateDef pawnKindTemplateDef;
        public int weight;
    }
}