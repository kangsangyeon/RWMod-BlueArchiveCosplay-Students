using System.Reflection;
using RimWorld;
using UniRx;
using UnityEngine;
using Verse;

namespace BA
{
    [StaticConstructorOnStartup]
    public static class BAStudents
    {
        public static bool DisableIMGUI = false;
        public static bool DisableCamera = false;
        public static bool DisableAudio = false;

        static BAStudents()
        {
            new HarmonyLib.Harmony("BlueArchiveStudents")
                .PatchAll(Assembly.GetExecutingAssembly());

            Application.logMessageReceived += OnUnityLogMessageReceived;

            Harmony_Pawn_SpawnSetup.OnPostfix
                .Subscribe(x => OnPawnSetup(x.instance, x.map, x.respawningAfterLoad));
        }

        private static void OnUnityLogMessageReceived(string log, string stackTrace, LogType type)
        {
            // BAU: blue archive unity
            if (log.StartsWith("BAU"))
                Log.Message($"{type.ToString()}::{log}\n{stackTrace}");
        }

        private static void OnPawnSetup(Pawn instance, Map map, bool respawningAfterLoad)
        {
            if (instance.kindDef.race.race.Humanlike == false) // 사람이 아니면 건너뜀
                return;
            if (instance.kindDef.race.race.Animal) // 동물이면 건너뜀
                return;

            if (instance.kindDef.defName.StartsWith("BA"))
            {
                Log.Message("No Tattoo 설정");
                instance.style.BodyTattoo = TattooDefOf.NoTattoo_Body;
                instance.style.FaceTattoo = TattooDefOf.NoTattoo_Face;
            }
        }
    }
}