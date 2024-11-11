using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace BA
{
    public class WeightTables
    {
        private static readonly PropertyInfo PropInfo_DefDatabaseAllDefs;

        private static Dictionary<string, IWeightTable> _tableByNameDict;

        public static void TryInitializeTableDict()
        {
            if (_tableByNameDict != null)
                return;

            _tableByNameDict = new Dictionary<string, IWeightTable>();

            var weightTableTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    x.IsInterface == false
                    && x.IsAbstract == false
                    && typeof(IWeightTable).IsAssignableFrom(x))
                .ToList();

            Log.Message($"weight table type count: {weightTableTypes.Count}");

            foreach (var t in weightTableTypes)
            {
                Type defDbType = typeof(DefDatabase<>);
                Type defDb = defDbType.MakeGenericType(t);
                PropertyInfo propertyInfo =
                    defDb.GetProperty("AllDefs", BindingFlags.Static | BindingFlags.Public);
                var allTableDefList = (propertyInfo.GetValue(defDb) as IEnumerable<IWeightTable>).ToList();
                Log.Message($"{t.Name} table count: {allTableDefList.Count}");
                foreach (var table in allTableDefList)
                {
                    var def = table as Def;
                    _tableByNameDict.Add(def.defName, table);
                }
            }
        }

        public static IWeightTableItem GetRandomItem(string tableName)
        {
            if (_tableByNameDict == null)
                TryInitializeTableDict();

            return _tableByNameDict[tableName].Items.RandomElementByWeight(x => x.Weight);
        }
    }
}