using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace BlueArchiveStudents
{
    public class WeightTables
    {
        private static Dictionary<string, IWeightTable> tableByNameDict;

        public static void TryInitializeTableDict()
        {
            if (tableByNameDict != null)
                return;

            tableByNameDict = new Dictionary<string, IWeightTable>();

            var _weightTableTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    x.IsInterface == false
                    && x.IsAbstract == false
                    && typeof(IWeightTable).IsAssignableFrom(x))
                .ToList();

            Log.Message($"weight table type count: {_weightTableTypes.Count}");

            foreach (var t in _weightTableTypes)
            {
                Type _defDbType = typeof(DefDatabase<>);
                Type _defDb = _defDbType.MakeGenericType(t);
                PropertyInfo _propertyInfo =
                    _defDb.GetProperty("AllDefs", BindingFlags.Static | BindingFlags.Public);
                var _allTableDefList = (_propertyInfo.GetValue(_defDb) as IEnumerable<IWeightTable>).ToList();
                Log.Message($"{t.Name} table count: {_allTableDefList.Count}");
                foreach (var _table in _allTableDefList)
                {
                    var _def = _table as Def;
                    tableByNameDict.Add(_def.defName, _table);
                }
            }
        }

        public static IWeightTableItem GetRandomItem(string _tableName)
        {
            if (tableByNameDict == null)
            {
                TryInitializeTableDict();
            }

            return tableByNameDict[_tableName].Items.RandomElementByWeight(x => x.Weight);
        }
    }
}