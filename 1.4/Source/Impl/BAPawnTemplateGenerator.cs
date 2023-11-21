using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BlueArchiveStudents
{
    public static class PawnKindTemplateGeneratorDebug
    {
        private static IList<PawnKindTemplateDef> m_DefList;

        public static void TryLoadAllDefs()
        {
            if (m_DefList != null && m_DefList.Count > 0)
            {
                return;
            }

            m_DefList = new List<PawnKindTemplateDef>();
            foreach (var _def in DefDatabase<PawnKindTemplateDef>.AllDefs)
                m_DefList.Add(_def);
        }

        [DebugAction("BlueArchiveStudents", "Spawn Makoto",
            false, false, false, 0, false,
            allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void SpawnMakoto()
        {
            TryLoadAllDefs();
            var _targetDef = m_DefList.First(x => x.defName.EqualsIgnoreCase("makoto_template"));
            PawnKindTemplateGenerator.Generate(_targetDef, Verse.Current.Game.CurrentMap);
        }

        [DebugAction("BlueArchiveStudents", "Try Spawn Makoto",
            false, false, false, 0, false,
            allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void TrySpawnMakoto()
        {
            TryLoadAllDefs();

            var _targetDef = m_DefList.First(x => x.defName.EqualsIgnoreCase("makoto_template"));

            // 모든 map과 world 내에서 살아있는 폰들의 목록을 얻습니다.
            // todo maps와 world가 무슨 차이인지 알아보기

            var _allPawnsAlive = Current.Game.Maps
                .Select(x => x.mapPawns)
                .SelectMany(x => x.AllPawns)
                .Where(x => x.health.Dead == false)
                .ToList();

            _allPawnsAlive.AddRange(Current.Game.World.worldPawns.AllPawnsAlive);

            // 살아있는 폰들 중 동일한 pawnKind를 가진 pawn이 하나라도 있는지 검사합니다.
            bool _exist = _allPawnsAlive.Any(x => x.kindDef == _targetDef.pawnKindDef);

            if (_exist)
                Verse.Log.Message($"{_targetDef.pawnKindDef.defName}가 이미 스폰되어 있습니다.");
            else
                PawnKindTemplateGenerator.Generate(_targetDef, Verse.Current.Game.CurrentMap);
        }
    }
}