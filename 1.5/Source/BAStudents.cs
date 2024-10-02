using System.Reflection;
using UnityEngine;
using Verse;

namespace BA
{
    [StaticConstructorOnStartup]
    public static class BAStudents
    {
        public static bool DisableIMGUI = false;

        static BAStudents()
        {
            new HarmonyLib.Harmony("BlueArchiveStudents")
                .PatchAll(Assembly.GetExecutingAssembly());

            Application.logMessageReceived += (_log, _trace, _type) => { Log.Message($"{_type.ToString()}::{_log}\n{_trace}"); };
        }
    }
}