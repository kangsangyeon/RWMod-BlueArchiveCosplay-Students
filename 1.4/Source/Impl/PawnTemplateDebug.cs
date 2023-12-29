using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BlueArchiveStudents
{
    public static class PawnKindTemplateDebug
    {
        private static IList<PawnKindTemplateDef> m_DefList;
        private static bool initialized = false;

        public static void TryLoadAllDefs()
        {
            if (initialized)
                return;

            initialized = true;
            m_DefList = new List<PawnKindTemplateDef>();
            foreach (var _def in DefDatabase<PawnKindTemplateDef>.AllDefs)
                m_DefList.Add(_def);
        }

        private static void TrySpawn(PawnKindTemplateDef _template)
        {
            var _allPawnsAlive = Current.Game.Maps
                .Select(x => x.mapPawns)
                .SelectMany(x => x.AllPawns)
                .Where(x => x.health.Dead == false)
                .ToList();

            _allPawnsAlive.AddRange(Current.Game.World.worldPawns.AllPawnsAlive);

            // 살아있는 폰들 중 동일한 pawnKind를 가진 pawn이 하나라도 있는지 검사합니다.
            bool _exist = _allPawnsAlive.Any(x => x.kindDef == _template.pawnKindDef);

            if (_exist)
                Verse.Log.Warning($"{_template.pawnKindDef.defName}가 이미 스폰되어 있습니다.");
            else
                PawnKindTemplateGenerator.Generate(_template, Verse.Current.Game.CurrentMap);
        }

        [DebugAction("BlueArchiveStudents", "Spawn BA Pawn", false, false, false, 0, false,
            allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void SpawnBAPawn()
        {
            TryLoadAllDefs();

            List<DebugMenuOption> options = new List<DebugMenuOption>();
            foreach (var x in m_DefList)
            {
                options.Add(new DebugMenuOption(
                    $"[{x.pawnKindDef.defName}]",
                    DebugMenuOptionMode.Action,
                    () => PawnKindTemplateGenerator.Generate(x, Verse.Current.Game.CurrentMap)));
            }

            Find.WindowStack.Add(new Dialog_DebugOptionListLister(options));
        }

        [DebugAction("BlueArchiveStudents", "Try Spawn BA Pawn", false, false, false, 0, false,
            allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void TrySpawnBAPawn()
        {
            TryLoadAllDefs();

            List<DebugMenuOption> options = new List<DebugMenuOption>();
            foreach (var x in m_DefList)
            {
                options.Add(new DebugMenuOption(
                    $"[{x.pawnKindDef.defName}]",
                    DebugMenuOptionMode.Action,
                    () => TrySpawn(x)));
            }

            Find.WindowStack.Add(new Dialog_DebugOptionListLister(options));
        }
    }
}