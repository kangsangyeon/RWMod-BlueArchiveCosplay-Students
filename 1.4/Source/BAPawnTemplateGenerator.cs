using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BlueArchiveStudents
{
    public static class PawnKindTemplateGeneratorDebug
    {
        private static IList<PawnKindTemplateDef> m_DefList = new List<PawnKindTemplateDef>();

        public static void TryLoadAllDefs()
        {
            //if (m_DefList != null)
            //    return;

            m_DefList = new List<PawnKindTemplateDef>();
            foreach (var _def in DefDatabase<PawnKindTemplateDef>.AllDefs)
                m_DefList.Add(_def);
        }

        [DebugAction("BlueArchiveStudents", "Spawn Makoto", false, false, false, 0, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void SpawnMakoto()
        {
            TryLoadAllDefs();
            var _targetDef = m_DefList.First(x => x.firstName.EqualsIgnoreCase("makoto"));
            PawnKindTemplateGenerator.Generate(_targetDef, Verse.Current.Game.CurrentMap);
        }
    }
}
