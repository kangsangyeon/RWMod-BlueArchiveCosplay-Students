using System.Reflection;
using Verse;

namespace BA
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatcher
    {
        static HarmonyPatcher()
        {
            new HarmonyLib.Harmony("BlueArchiveStudents")
                .PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    public static class BAStudents
    {
        public static bool DisableIMGUI = false;
    }
}