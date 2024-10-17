using System.Reflection;
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

            Application.logMessageReceived += (_log, _trace, _type) =>
            {
                // BAU: blue archive unity
                if (_log.StartsWith("BAU"))
                    Log.Message($"{_type.ToString()}::{_log}\n{_trace}");
            };
        }
    }
}