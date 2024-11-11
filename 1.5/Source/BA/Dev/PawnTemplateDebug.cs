using System;
using System.Collections.Generic;
using System.Linq;
using LudeonTK;
using Verse;

namespace BA
{
    public static class PawnKindTemplateDebug
    {
        private static IList<PawnKindTemplateDef> _defList;
        private static bool _initialized = false;

        public static void TryLoadAllDefs()
        {
            if (_initialized)
                return;

            _initialized = true;
            _defList = new List<PawnKindTemplateDef>();
            foreach (var def in DefDatabase<PawnKindTemplateDef>.AllDefs)
                _defList.Add(def);
            _defList.SortStable((a, b) => String.Compare(a.nickname, b.nickname, StringComparison.Ordinal));
        }

        private static void TrySpawn(PawnKindTemplateDef template)
        {
            var allPawnsAlive = Current.Game.Maps
                .Select(x => x.mapPawns)
                .SelectMany(x => x.AllPawns)
                .Where(x => x.health.Dead == false)
                .ToList();

            allPawnsAlive.AddRange(Current.Game.World.worldPawns.AllPawnsAlive);

            // 살아있는 폰들 중 동일한 pawnKind를 가진 pawn이 하나라도 있는지 검사합니다.
            bool exist = allPawnsAlive.Any(x => x.kindDef == template.pawnKindDef);

            if (exist)
                Verse.Log.Warning($"{template.pawnKindDef.defName}가 이미 스폰되어 있습니다.");
            else
                PawnKindTemplateGenerator.Generate(template, Verse.Current.Game.CurrentMap);
        }

        [DebugAction("BlueArchiveStudents", "Spawn BA Pawn",
            false, false, false, false,
            0, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void SpawnBaPawn()
        {
            TryLoadAllDefs();

            List<DebugMenuOption> options = new List<DebugMenuOption>();
            foreach (var x in _defList)
            {
                options.Add(new DebugMenuOption(
                    $"[{x.pawnKindDef.defName}]",
                    DebugMenuOptionMode.Action,
                    () => PawnKindTemplateGenerator.Generate(x, Verse.Current.Game.CurrentMap)));
            }

            Find.WindowStack.Add(new Dialog_DebugOptionListLister(options));
        }

        [DebugAction("BlueArchiveStudents", "Try Spawn BA Pawn",
            false, false, false, false,
            0, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void TrySpawnBaPawn()
        {
            TryLoadAllDefs();

            List<DebugMenuOption> options = new List<DebugMenuOption>();
            foreach (var x in _defList)
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