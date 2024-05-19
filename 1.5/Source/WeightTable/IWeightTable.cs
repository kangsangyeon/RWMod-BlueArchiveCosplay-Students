using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BlueArchiveStudents
{
    public interface IWeightTable
    {
        List<IWeightTableItem> Items { get; }
    }

    public interface IWeightTableItem
    {
        float Weight { get; }
    }
}