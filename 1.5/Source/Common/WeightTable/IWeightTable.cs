using System.Collections.Generic;

namespace BA
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